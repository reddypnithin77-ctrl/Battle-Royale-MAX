using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BRVehicleController : MonoBehaviour
{
    [Header("Driving")]
    public float acceleration = 18f;
    public float reverseAcceleration = 9f;
    public float maxForwardSpeed = 24f;
    public float maxReverseSpeed = 8f;
    public float steeringStrength = 85f;
    public float lateralGrip = 7f;
    public float brakeStrength = 16f;
    [Header("Stability")]
    public Transform centerOfMass;
    public float downforce = 20f;
    public float airControl = 0.25f;

    private Rigidbody body;
    private float throttle;
    private float steering;
    private bool braking;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        if (centerOfMass != null)
            body.centerOfMass = transform.InverseTransformPoint(centerOfMass.position);
    }

    // Connect mobile UI buttons/axes to these public methods.
    public void SetDriveInput(float value) { throttle = Mathf.Clamp(value, -1f, 1f); }
    public void SetSteeringInput(float value) { steering = Mathf.Clamp(value, -1f, 1f); }
    public void SetBrake(bool value) { braking = value; }

    private void FixedUpdate()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(body.velocity);
        bool grounded = Physics.Raycast(transform.position + Vector3.up * 0.25f, Vector3.down, 1.2f);
        float control = grounded ? 1f : airControl;

        if (throttle > 0f && localVelocity.z < maxForwardSpeed)
            body.AddForce(transform.forward * (throttle * acceleration), ForceMode.Acceleration);
        else if (throttle < 0f && localVelocity.z > -maxReverseSpeed)
            body.AddForce(transform.forward * (throttle * reverseAcceleration), ForceMode.Acceleration);

        float speedFactor = Mathf.Clamp01(body.velocity.magnitude / 3f);
        if (grounded && body.velocity.sqrMagnitude > 0.5f)
            body.AddTorque(Vector3.up * steering * steeringStrength * speedFactor * control, ForceMode.Acceleration);

        Vector3 lateralVelocity = transform.right * Vector3.Dot(body.velocity, transform.right);
        body.AddForce(-lateralVelocity * lateralGrip * control, ForceMode.Acceleration);

        if (braking)
            body.AddForce(-body.velocity * brakeStrength, ForceMode.Acceleration);
        if (grounded)
            body.AddForce(-transform.up * downforce, ForceMode.Acceleration);
    }
}
