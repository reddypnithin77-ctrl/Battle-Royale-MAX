using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BRPlayerMotor : MonoBehaviour
{
    public float moveSpeed = 5.5f;
    public float jumpHeight = 1.2f;
    public float gravity = -22f;
    public Transform cameraTransform;
    private CharacterController controller;
    private Vector3 velocity;

    private void Awake() { controller = GetComponent<CharacterController>(); }

    private void Update()
    {
        Vector2 input = MobileInputBridge.Instance != null ? MobileInputBridge.Instance.Move : Vector2.zero;
        if (input.sqrMagnitude < 0.001f) input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Transform cam = cameraTransform != null ? cameraTransform : Camera.main.transform;
        Vector3 forward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;
        Vector3 direction = Vector3.ClampMagnitude(forward * input.y + right * input.x, 1f);
        controller.Move(direction * moveSpeed * Time.deltaTime);
        if (controller.isGrounded && velocity.y < 0f) velocity.y = -2f;
        bool jump = MobileInputBridge.Instance != null && MobileInputBridge.Instance.ConsumeJump();
        if (Input.GetButtonDown("Jump")) jump = true;
        if (jump && controller.isGrounded) velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        if (direction.sqrMagnitude > 0.01f) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 12f * Time.deltaTime);
    }
}
