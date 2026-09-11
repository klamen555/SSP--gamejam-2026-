using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class InteractorSystem : MonoBehaviour
{
    [SerializeField] PlayerSettings settings;
    [SerializeField] LayerMask interactionLayer;

    PlayerControls controls;
    InputAction interactAction;

    private void Update()
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
