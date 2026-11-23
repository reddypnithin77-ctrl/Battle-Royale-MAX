using UnityEngine;

public class BRThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2.1f, -4.2f);
    public float sensitivity = 0.12f;
    public float followSmooth = 14f;
    private float yaw;
    private float pitch = 12f;

    private void LateUpdate()
    {
        if (target == null) return;
        Vector2 look = MobileInputBridge.Instance != null ? MobileInputBridge.Instance.LookDelta : Vector2.zero;
        yaw += look.x * sensitivity;
        pitch = Mathf.Clamp(pitch - look.y * sensitivity, -25f, 65f);
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desired = target.position + rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desired, followSmooth * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(target.position + Vector3.up * 1.4f - transform.position);
    }
}
