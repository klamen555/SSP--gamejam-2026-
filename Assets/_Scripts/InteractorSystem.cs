using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class InteractorSystem : MonoBehaviour
{
    [SerializeField] PlayerSettings settings;
    [SerializeField] LayerMask interactionLayer;



    //PlayerControls controls;
    //InputAction interactAction;

    //private void Awake()
    //{
    //    controls = new PlayerControls();
    //    interactAction = controls.Player.Interact;
    //}

    //private void OnEnable()
    //{
    //    controls.Player.Enable();

    //    interactAction.started += OnInteractionStarted;
    //}

    //private void OnDisable()
    //{
    //    interactAction.started -= OnInteractionStarted;

    //    controls.Player.Disable();
    //}

    //private void OnInteractionStarted(InputAction.CallbackContext context)
    //{
    //    RaycastHit hit;
    //    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, settings.interactionRange, interactionLayer.value))
    //    {
    //        if (hit.transform.TryGetComponent<IInteractable>(out IInteractable interactable))
    //        {
    //            interactable.OnInteract();
    //        }
    //    }
    //}

    public GameObject currentlyHolding;
    public Transform handPosititon;

    public void TakeItem(GameObject item)
    {
        if (currentlyHolding == null)
        {
            item.transform.SetParent(handPosititon);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            currentlyHolding = item;
        }

        ProximityPromptScript[] prompts = FindObjectsByType<ProximityPromptScript>(FindObjectsSortMode.None);

        foreach (ProximityPromptScript prompt in prompts)
        {
            if (prompt.transform.gameObject.layer == interactionLayer.value)
            {
                prompt.GetComponent<ProximityPromptScript>().IsVisible = false;
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

        currentlyHolding = null;
    }
}
