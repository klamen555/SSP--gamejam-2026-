using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public bool CanWalk = true;

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Camera playerCamera;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float lookSpeed = 2f;

    [Header("Restricted Look Limits")]
    [SerializeField] public bool applyLookLimits = false;
    [SerializeField] private float maxHorizontalAngle = 45f;
    [SerializeField] private float maxVerticalAngle = 45f;

    [Header("Camera Shake / Bob Settings")]
    [SerializeField] private float bobSpeed = 14f;
    [SerializeField] private float bobAmount = 0.05f;
    
    private float rotationX = 0f;
    private float rotationY = 0f;
    private float defaultYPos = 0f;
    private float timer = 0f;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        if (playerCamera != null)
        {
            defaultYPos = playerCamera.transform.localPosition.y;
        }

        rotationY = transform.localEulerAngles.y;
    }

    void Update()
    {
        HandleLook();
        HandleCameraBob();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (!CanWalk)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = (transform.forward * moveZ + transform.right * moveX).normalized;

        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
    }

    private void HandleLook()
    {
        if (playerCamera == null) return;

        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        rotationX -= mouseY;
        rotationY += mouseX;

        if (applyLookLimits)
        {
            rotationX = Mathf.Clamp(rotationX, -maxVerticalAngle, maxVerticalAngle);
            rotationY = Mathf.Clamp(rotationY, -maxHorizontalAngle, maxHorizontalAngle);
        }
        else
        {
            rotationX = Mathf.Clamp(rotationX, -89f, 89f);
        }

        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        rb.MoveRotation(Quaternion.Euler(0f, rotationY, 0f));
    }

    private void HandleCameraBob()
    {
        if (playerCamera == null) return;

        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        
        if (horizontalVelocity.magnitude > 0.1f)
        {
            timer += Time.deltaTime * bobSpeed;

            float noiseX = (Mathf.PerlinNoise(timer, 0f) - 0.5f) * bobAmount;
            float noiseY = (Mathf.PerlinNoise(0f, timer) - 0.5f) * bobAmount;

            playerCamera.transform.localPosition = new Vector3(
                noiseX,
                defaultYPos + noiseY,
                playerCamera.transform.localPosition.z
            );
        }
        else
        {
            timer = 0f;
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition, 
                new Vector3(0, defaultYPos, playerCamera.transform.localPosition.z), 
                Time.deltaTime * 10f
            );
        }
    }
}