using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float ladderClimbSpeed = 3f;
    private Vector3 moveDirection = Vector3.zero;

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 85f;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchTransitionSpeed = 8f;
    [SerializeField] private float originalCameraY = 8f;
    private bool isCrouching = false;
    private float originalHeight;
    private Vector3 originalCenter;
    private bool crouchRequested = false;

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private float xRotation = 0f;
    private bool isGrounded;
    private bool isClimbing = false;
    private bool onLadder = false;

    void Start()
    {
        originalHeight = capsuleCollider.height;
        originalCenter = capsuleCollider.center;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleCameraMovement();
        HandleMovement();
        HandleJump();
        HandleCrouch();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            onLadder = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            onLadder = false;
            isClimbing = false;
            rb.useGravity = true; // Restore gravity when leaving ladder
            rb.linearVelocity = Vector3.zero; // Prevent unwanted movement
        }
    }

    private void HandleCameraMovement()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && !isCrouching;
        float currentSpeed = isRunning ? runSpeed : (isCrouching ? crouchSpeed : walkSpeed);

        Vector3 targetDirection = transform.right * moveX + transform.forward * moveZ;
        targetDirection = targetDirection.normalized * currentSpeed;

        if (onLadder)
        {
            HandleLadderMovement();
            return;
        }

        if (isGrounded)
        {
            if (rb.linearVelocity.y <= 0f && moveX == 0 && moveZ == 0)
            {
                moveDirection = Vector3.zero;
            }
            else
            {
                moveDirection = Vector3.Lerp(moveDirection, targetDirection, Time.deltaTime * acceleration);
            }
        }
        else
        {
            moveDirection = Vector3.Lerp(moveDirection, targetDirection * 0.5f, Time.deltaTime * acceleration);
        }

        Vector3 velocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
        rb.linearVelocity = velocity;
    }

    private void HandleLadderMovement()
    {
        if (!isClimbing)
        {
            isClimbing = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero; // Reset velocity
        }

        float verticalInput = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(verticalInput) > 0.1f)
        {
            rb.linearVelocity = new Vector3(0, verticalInput * ladderClimbSpeed, 0);
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isClimbing = false;
            onLadder = false;
            rb.useGravity = true;
            rb.linearVelocity = new Vector3(moveDirection.x, jumpForce, moveDirection.z);
        }
    }

    private void HandleJump()
    {
        Collider[] groundCollisions = new Collider[1];
        isGrounded = Physics.OverlapSphereNonAlloc(groundCheck.position, 0.2f, groundCollisions, groundLayer) > 0;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrouching)
        {
            rb.linearVelocity = new Vector3(moveDirection.x, 0f, moveDirection.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            crouchRequested = true;
            Crouch();
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            crouchRequested = false;
            TryStandUp();
        }

        if (isCrouching && !crouchRequested)
        {
            TryStandUp();
        }

        float targetHeight = isCrouching ? crouchHeight : originalHeight;
        Vector3 targetCenter = isCrouching ? new Vector3(0, crouchHeight / 2f, 0) : originalCenter;

        capsuleCollider.height = targetHeight;
        capsuleCollider.center = targetCenter;

        Vector3 targetCameraPos = cameraHolder.localPosition;
        targetCameraPos.y = isCrouching ? crouchHeight : originalCameraY;
        cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, targetCameraPos, Time.deltaTime * crouchTransitionSpeed);
    }

    private void Crouch()
    {
        isCrouching = true;
        capsuleCollider.height = crouchHeight;
        capsuleCollider.center = new Vector3(0, crouchHeight / 2f, 0);
    }

    private void TryStandUp()
    {
        bool canStand = !Physics.CheckCapsule(
            transform.position + Vector3.up * crouchHeight,
            transform.position + Vector3.up * originalHeight,
            capsuleCollider.radius * 0.9f,
            groundLayer
        );

        if (canStand)
        {
            isCrouching = false;
            capsuleCollider.height = originalHeight;
            capsuleCollider.center = originalCenter;
        }
    }
}
