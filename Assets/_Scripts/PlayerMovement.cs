using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerSettings settings;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform mainCamera;

    float currentSpeed;
    float yaw;
    float pitch;
    float shakeTimer;

    Vector3 defaultCameraPosition;

    PlayerControls controls;
    InputAction moveAction;
    InputAction sprintAction;
    InputAction lookAction;

    private void Awake()
    {
        controls = new PlayerControls();
        moveAction = controls.Player.Move;
        sprintAction = controls.Player.Sprint;
        lookAction = controls.Player.Look;
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;

        currentSpeed = settings.normalSpeed;

        defaultCameraPosition = mainCamera.localPosition;
    }

    private void OnEnable()
    {
        controls.Player.Enable();

        sprintAction.started += OnSprintStarted;
        sprintAction.canceled += OnSprintCanceled;
    }

    private void OnDisable()
    {
        sprintAction.started -= OnSprintStarted;
        sprintAction.canceled -= OnSprintCanceled;

        controls.Player.Disable();
    }

    private void OnSprintStarted(InputAction.CallbackContext context)
    {
        currentSpeed = settings.runningSpeed;
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        currentSpeed = settings.normalSpeed;
    }

    private void Update()
    {
        HandleMovement();
        HandleCamera();
        HandleHeadbobbing();
    }

    void HandleMovement()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 velocity = new Vector3(moveInput.x, 0f, moveInput.y);
        velocity.Normalize();
        velocity = velocity * currentSpeed * Time.deltaTime;
        velocity = Quaternion.Euler(0f, yaw, 0f) * velocity;

        controller.Move(velocity);
    }

    void HandleCamera()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        yaw += lookInput.x * settings.cameraSensitivity;
        pitch -= lookInput.y * settings.cameraSensitivity;
        pitch = Mathf.Clamp(pitch, -80f, 80f);
        mainCamera.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void HandleHeadbobbing()
    {
        float inputMagnitude = moveAction.ReadValue<Vector2>().magnitude;
        Vector3 targetOffset = defaultCameraPosition;

        if (inputMagnitude > 0.01f)
        {
            shakeTimer += Time.deltaTime * settings.cameraShakeFrequency * inputMagnitude;
            float verticalOffset = Mathf.Sin(shakeTimer) * settings.cameraShakeVerticalAmplitude;
            float horizontalOffset = Mathf.Cos(shakeTimer * 0.5f) * settings.cameraShakeHorizontalAmplitude;
            targetOffset += new Vector3(horizontalOffset, verticalOffset, 0f);
        }
        else
        {
            shakeTimer = 0f;
        }

        mainCamera.localPosition = Vector3.Lerp(mainCamera.localPosition, targetOffset, Time.deltaTime * settings.cameraShakeSmoothing);
    }
}
