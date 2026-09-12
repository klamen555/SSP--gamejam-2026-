using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class InteractorSystem : MonoBehaviour
{
    [SerializeField] PlayerSettings settings;
    [SerializeField] LayerMask interactionLayer;



    PlayerControls controls;
    InputAction dropAction;

    private void Awake()
    {
        controls = new PlayerControls();
        dropAction = controls.Player.Drop;
    }

    private void OnEnable()
    {
        controls.Player.Enable();

        dropAction.started += OnInteractionStarted;
    }

    private void OnDisable()
    {
        dropAction.started -= OnInteractionStarted;

        controls.Player.Disable();
    }

    private void OnInteractionStarted(InputAction.CallbackContext context)
    {
        
    }

    public GameObject currentlyHolding;
    public Transform handPosititon;

    public void TakeItem(GameObject item)
    {
        if (currentlyHolding == null)
        {
            item.transform.SetParent(handPosititon);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            item.transform.localRotation = Quaternion.identity;
            currentlyHolding = item;
        }

        ProximityPromptScript[] prompts = FindObjectsByType<ProximityPromptScript>(FindObjectsSortMode.None);

        foreach (ProximityPromptScript prompt in prompts)
        {
            if ((interactionLayer.value & (1 << prompt.gameObject.layer)) != 0)
            {
                prompt.IsVisible = false;
            }
        }
    }

    public void DropItem()
    {
        currentlyHolding.transform.SetParent(null);
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, settings.interactionRange))
        {
            currentlyHolding.transform.position = hit.point;
        }

        currentlyHolding.GetComponent<ProximityPromptScript>().enabled = true;
        currentlyHolding = null;
    }
}
