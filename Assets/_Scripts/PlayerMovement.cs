using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerSettings settings;
    [SerializeField] private CharacterController controller;

    float currentSpeed;

    PlayerControls controls;
    InputAction moveAction;
    InputAction shiftAction;

    private void Awake()
    {
        controls = new PlayerControls();
        moveAction = controls.Player.Move;
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void Update()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        moveInput.Normalize();
        moveInput *= currentSpeed;


    }
}
