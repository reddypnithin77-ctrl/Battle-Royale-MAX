using UnityEngine;

// Simple non-networked practice dummy that walks between nearby patrol points.
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(BRTargetHealth))]
public class TrainingBotPatrol : MonoBehaviour
{
    public float moveSpeed = 1.8f;
    public float turnSpeed = 5f;
    public float pointRadius = 7f;
    public float waitAtPoint = 1.2f;
    public bool patrol = true;

    private CharacterController controller;
    private Vector3 origin;
    private Vector3 destination;
    private float waitTimer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        origin = transform.position;
        ChooseDestination();
    }

    private void Update()
    {
        if (!patrol) return;
        Vector3 delta = destination - transform.position;
        delta.y = 0f;
        if (delta.magnitude <= 0.6f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitAtPoint) { waitTimer = 0f; ChooseDestination(); }
            return;
        }
        Vector3 direction = delta.normalized;
        controller.SimpleMove(direction * moveSpeed);
        Quaternion target = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, turnSpeed * Time.deltaTime);
    }

    private void ChooseDestination()
    {
        Vector2 offset = Random.insideUnitCircle * pointRadius;
        destination = origin + new Vector3(offset.x, 0f, offset.y);
    }
}
