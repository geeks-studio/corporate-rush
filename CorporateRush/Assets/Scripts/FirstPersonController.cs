using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float maxLookAngle = 80f;
    [SerializeField] private float crouchCameraHeight = 0.5f;
    [SerializeField] private float crouchTransitionSpeed = 5f;

    [Header("Physics")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.3f;

    private float currentSpeed;
    private float xRotation = 0f;
    private bool isGrounded = false;
    private bool isCrouching = false;
    private float defaultCameraHeight;
    private float targetCameraHeight;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        defaultCameraHeight = cameraTransform.localPosition.y;
        targetCameraHeight = defaultCameraHeight;

        // Заморозка вращения Rigidbody, чтобы капсула не падала.
        rb.freezeRotation = true;
    }

    private void Update()
    {
        HandleCamera();
        HandleMovement();
        HandleJump();
        HandleCrouch();
        SmoothCameraTransition();
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void HandleCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX); // Вращение капсулы вбок
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        currentSpeed = moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            currentSpeed *= sprintMultiplier;
        }

        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        moveDirection.Normalize();

        Vector3 velocity = new Vector3(moveDirection.x * currentSpeed, rb.linearVelocity.y, moveDirection.z * currentSpeed);
        rb.linearVelocity = velocity; // Прямое управление Rigidbody
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;

            targetCameraHeight = isCrouching ? crouchCameraHeight : defaultCameraHeight;
        }
    }

    private void SmoothCameraTransition()
    {
        Vector3 cameraPosition = cameraTransform.localPosition;
        cameraPosition.y = Mathf.Lerp(cameraPosition.y, targetCameraHeight, Time.deltaTime * crouchTransitionSpeed);
        cameraTransform.localPosition = cameraPosition;
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
