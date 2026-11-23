using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BRPlayerMotor : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5.5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2.8f;
    public float acceleration = 22f;
    [Range(0f, 1f)] public float airControl = 0.45f;
    public float jumpHeight = 1.2f;
    public float gravity = -22f;
    [Header("Crouch")]
    public float crouchHeight = 1.15f;
    public float standingHeight = 1.9f;
    public float stanceLerpSpeed = 12f;
    [Header("References")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 verticalVelocity;
    private Vector3 planarVelocity;
    private float defaultCenterY;
    private bool crouching;
    private bool sprintRequested;
    private bool crouchRequested;

    // Public controls allow mobile UI and automated Play Mode tests to drive stance.
    public void SetSprint(bool active) { sprintRequested = active; }
    public void SetCrouch(bool active) { crouchRequested = active; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        standingHeight = Mathf.Max(standingHeight, controller.height);
        defaultCenterY = controller.center.y;
    }

    private void Update()
    {
        MobileInputBridge bridge = MobileInputBridge.Instance;
        Vector2 input = bridge != null ? bridge.Move : Vector2.zero;
        if (input.sqrMagnitude < 0.001f)
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        bool sprint = (sprintRequested || Input.GetKey(KeyCode.LeftShift)) && input.y > 0.1f;
        crouching = crouchRequested || Input.GetKey(KeyCode.C);
        float targetSpeed = crouching ? crouchSpeed : (sprint ? sprintSpeed : walkSpeed);

        Transform cam = cameraTransform != null ? cameraTransform : (Camera.main != null ? Camera.main.transform : transform);
        Vector3 forward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;
        Vector3 desiredDirection = Vector3.ClampMagnitude(forward * input.y + right * input.x, 1f);
        Vector3 desiredVelocity = desiredDirection * targetSpeed;
        float control = controller.isGrounded ? 1f : airControl;
        planarVelocity = Vector3.MoveTowards(planarVelocity, desiredVelocity, acceleration * control * Time.deltaTime);

        if (controller.isGrounded && verticalVelocity.y < 0f)
            verticalVelocity.y = -2f;

        bool jump = bridge != null && bridge.ConsumeJump();
        if (Input.GetButtonDown("Jump")) jump = true;
        if (jump && controller.isGrounded && !crouching)
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move((planarVelocity + verticalVelocity) * Time.deltaTime);

        float wantedHeight = crouching ? crouchHeight : standingHeight;
        controller.height = Mathf.Lerp(controller.height, wantedHeight, stanceLerpSpeed * Time.deltaTime);
        Vector3 center = controller.center;
        center.y = defaultCenterY + (controller.height - standingHeight) * 0.5f;
        controller.center = center;

        if (desiredDirection.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredDirection), 12f * Time.deltaTime);
    }
}
