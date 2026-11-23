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
    public float groundProbeRadius = 0.35f;
    public float groundProbeDistance = 1.2f;
    public LayerMask groundMask = ~0;

    private Rigidbody body;
    private float throttle;
    private float steering;
    private bool braking;

    private void Awake()
    {
        acceleration = SafeNonNegative(acceleration, 18f);
        reverseAcceleration = SafeNonNegative(reverseAcceleration, 9f);
        maxForwardSpeed = SafeNonNegative(maxForwardSpeed, 24f);
        maxReverseSpeed = SafeNonNegative(maxReverseSpeed, 8f);
        steeringStrength = SafeNonNegative(steeringStrength, 85f);
        lateralGrip = SafeNonNegative(lateralGrip, 7f);
        brakeStrength = SafeNonNegative(brakeStrength, 16f);
        downforce = SafeNonNegative(downforce, 20f);
        groundProbeRadius = Mathf.Max(0.01f, SafeNonNegative(groundProbeRadius, 0.35f));
        groundProbeDistance = Mathf.Max(0.01f, SafeNonNegative(groundProbeDistance, 1.2f));
        airControl = Mathf.Clamp(SafeNonNegative(airControl, 0.25f), 0f, 1f);
        body = GetComponent<Rigidbody>();
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        if (centerOfMass != null)
            body.centerOfMass = transform.InverseTransformPoint(centerOfMass.position);
    }

    public void SetDriveInput(float value) { throttle = IsFinite(value) ? Mathf.Clamp(value, -1f, 1f) : 0f; }
    public void SetSteeringInput(float value) { steering = IsFinite(value) ? Mathf.Clamp(value, -1f, 1f) : 0f; }
    public void SetBrake(bool value) { braking = value; }

    private static bool IsFinite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
    private static float SafeNonNegative(float value, float fallback) =>
        IsFinite(value) ? Mathf.Max(0f, value) : fallback;

    private void FixedUpdate()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(body.velocity);
        bool grounded = Physics.SphereCast(transform.position + transform.up * 0.25f,
            groundProbeRadius, -transform.up, out RaycastHit groundHit,
            groundProbeDistance, groundMask, QueryTriggerInteraction.Ignore);
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

        // Clamp only the vehicle's forward/reverse component; preserve vertical and lateral motion.
        localVelocity = transform.InverseTransformDirection(body.velocity);
        localVelocity.z = Mathf.Clamp(localVelocity.z, -maxReverseSpeed, maxForwardSpeed);
        body.velocity = transform.TransformDirection(localVelocity);
    }
}
