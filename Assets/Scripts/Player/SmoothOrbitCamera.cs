using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SmoothOrbitCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    [Tooltip("The focus point the camera will orbit around")]
    public Transform focus;
    [Tooltip("The distance of the camera from the focus point")]
    public float distance = 6;
    [Tooltip("Clamp for the pitch rotation")]
    public Vector2 pitchClamp = new Vector2(-30f, 60f);

    [Header("Input Settings")]
    [Tooltip("Rotation speed of the camera")]
    public float rotationSpeed = 1000f;

    [Header("Collision Settings")]
    [Tooltip("Layer mask for collision detection")]
    public LayerMask collisionMask;
    [Tooltip("Offset to prevent clipping when colliding")]
    public float collisionOffset = 0.2f;

    public float yaw { get; private set; }
    public float pitch { get; private set; }
    private Vector2 lookInput;

    private void Awake()
    {
        ValidateFocus();
        InitializeCameraRotation();
    }

    private void Start()
    {
        ValidateFocus();
        InitializeCameraRotation();
    }

    private void LateUpdate()
    {
        if (!focus) return;

        HandleCameraRotation();
        UpdateCameraPosition();
    }

    private void OnDrawGizmosSelected()
    {
        if (!focus) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(focus.position, transform.position);
        Gizmos.DrawWireSphere(focus.position, 0.5f);
    }

    private void ValidateFocus()
    {
        if (!focus)
        {
            Debug.LogError("Focus (target) is not assigned in the SmoothOrbitCamera script.");
            enabled = false;
        }
    }

    private void InitializeCameraRotation()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    private void HandleCameraRotation()
    {
        lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        yaw += lookInput.x * rotationSpeed * Time.deltaTime;
        pitch -= lookInput.y * rotationSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, pitchClamp.x, pitchClamp.y);
    }

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = CalculateDesiredPosition(rotation);

        if (Physics.Raycast(focus.position, desiredPosition - focus.position, out RaycastHit hit, distance, collisionMask))
        {
            desiredPosition = hit.point + (hit.normal * collisionOffset);
        }

        transform.position = desiredPosition;
        transform.rotation = rotation;
    }

    private Vector3 CalculateDesiredPosition(Quaternion rotation)
    {
        return focus.position - (rotation * Vector3.forward * distance);
    }
}
