using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class InteractorSystem : MonoBehaviour
{
    [SerializeField] PlayerSettings settings;
    [SerializeField] LayerMask interactionLayer;

    public GameObject currentlyHolding;

    PlayerControls controls;
    InputAction interactAction;

    private void Awake()
    {
        controls = new PlayerControls();
        interactAction = controls.Player.Interact;
    }

    private void OnEnable()
    {
        controls.Player.Enable();

        interactAction.started += OnInteractionStarted;
    }

    private void OnDisable()
    {
        interactAction.started -= OnInteractionStarted;

        controls.Player.Disable();
    }

    private void OnInteractionStarted(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, settings.interactionRange, interactionLayer.value))
        {
            if (hit.transform.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                interactable.OnInteract();
            }
        }
    }
}
