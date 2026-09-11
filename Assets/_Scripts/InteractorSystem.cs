using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class InteractorSystem : MonoBehaviour
{
    [SerializeField] PlayerSettings settings;
    [SerializeField] LayerMask interactionLayer;

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

        interactAction.started += OnSprintStarted;
    }

    private void OnDisable()
    {
        interactAction.started -= OnSprintStarted;

        controls.Player.Disable();
    }

    private void OnSprintStarted(InputAction.CallbackContext context)
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
