using System.Collections.Generic;
using Unity.VisualScripting;
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

        dropAction.started += OnDropStarted;
    }

    private void OnDisable()
    {
        dropAction.started -= OnDropStarted;

        controls.Player.Disable();
    }

    private void OnDropStarted(InputAction.CallbackContext context)
    {
        DropItem();
    }

    public GameObject currentlyHolding;
    public Transform handPosititon;

    public void TakeItem(GameObject item)
    {
        if (item.GetComponent<Rigidbody>() != null) Destroy(item.GetComponent<Rigidbody>());

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
        currentlyHolding.transform.localScale = Vector3.one;
        currentlyHolding.transform.AddComponent<Rigidbody>();
        currentlyHolding = null;

        ProximityPromptScript[] prompts = FindObjectsByType<ProximityPromptScript>(FindObjectsSortMode.None);

        foreach (ProximityPromptScript prompt in prompts)
        {
            if ((interactionLayer.value & (1 << prompt.gameObject.layer)) != 0)
            {
                prompt.IsVisible = true;
            }
        }
    }
}
