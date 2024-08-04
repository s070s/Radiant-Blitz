using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpHeight = 2.0f;
    public float gravity = 9.81f;
    public float rotationSpeed = 0.2f;

    [Header("Camera Projection Vectors")]
    [Tooltip("Main camera object for projection calculations")]
    public GameObject cam;

    private CharacterController characterController;
    private Vector2 moveInput;
    private bool isRunning;
    private bool isJumping;
    private bool isAiming;
    private Vector3 moveDirection;
    private float verticalVelocity;
    private Vector3 forwardProjectionVector;
    private Vector3 rightProjectionVector;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void LateUpdate()
    {
        UpdateProjectionVectors();
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
        HandleOrientation();
    }

    /// <summary>
    /// Handles the input from the old input manager.
    /// </summary>
    private void HandleInput()
    {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        isRunning = Input.GetKey(KeyCode.LeftShift);
        isJumping = Input.GetButtonDown("Jump");
        isAiming = Input.GetButton("Fire2"); // Assuming right mouse button for aiming
    }

    /// <summary>
    /// Updates the forward and right projection vectors based on the camera's orientation.
    /// </summary>
    private void UpdateProjectionVectors()
    {
        forwardProjectionVector = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
        rightProjectionVector = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;
    }

    /// <summary>
    /// Handles the character's orientation based on movement and aiming.
    /// </summary>
    private void HandleOrientation()
    {
        if (isAiming)
        {
            RotateTowards(forwardProjectionVector);
        }
        else
        {
            Vector3 moveVector = new Vector3(moveDirection.x, 0, moveDirection.z);
            if (moveVector != Vector3.zero)
            {
                RotateTowards(moveVector);
            }
        }
    }

    /// <summary>
    /// Rotates the character towards the given direction.
    /// </summary>
    /// <param name="direction">Direction to rotate towards</param>
    private void RotateTowards(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / rotationSpeed);
    }

    /// <summary>
    /// Handles the character's movement based on input and environment.
    /// </summary>
    private void HandleMovement()
    {
        float speed = isRunning && characterController.isGrounded ? runSpeed : walkSpeed;

        if (characterController.isGrounded)
        {
            Vector3 direction = rightProjectionVector * moveInput.x + forwardProjectionVector * moveInput.y;
            moveDirection = direction.normalized * speed;

            if (isJumping)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        moveDirection.y = verticalVelocity;
        characterController.Move(moveDirection * Time.deltaTime);
    }
}
