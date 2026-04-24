using UnityEngine;

public class BallSwing : MonoBehaviour
{
    public enum BowlType
    {
        Swing,
        Spin
    }

    [Header("Mode")]
    public BowlType currentMode = BowlType.Swing;

    [Header("References")]
    public Transform bounceMarker;

    [Header("Delivery Settings")]
    public float flightTime = 0.9f;

    [Header("Swing Settings")]
    [Range(-1, 1)]
    public int swingDirection = 1;

    [Range(0f, 1f)]
    public float swingStrength = 0.5f;

    public float maxSwingForce = 8f;

    [Header("Spin Settings")]
    [Range(-1, 1)]
    public int spinDirection = 1;

    [Range(0f, 1f)]
    public float spinStrength = 0.5f;

    public float maxSpinForce = 6f;

    private Rigidbody rb;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private Vector3 forwardDir;
    private Vector3 lateralDir;

    private bool hasBounced = false;
    private bool isMoving = false;

    private float elapsedTime = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        startPosition = transform.position;
    }

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
    }

    public void Bowl()
    {
        if (isMoving) return;

        hasBounced = false;
        isMoving = true;
        elapsedTime = 0f;

        Vector3 displacement = targetPosition - transform.position;

        // Projectile velocity calculation
        Vector3 velocity = new Vector3(
            displacement.x / flightTime,
            (displacement.y + 0.5f * Mathf.Abs(Physics.gravity.y) * flightTime * flightTime * 1.2f) / flightTime,
            displacement.z / flightTime
        );

        rb.isKinematic = false;
        rb.linearVelocity = velocity;

        forwardDir = new Vector3(velocity.x, 0f, velocity.z).normalized;
        lateralDir = Vector3.Cross(Vector3.up, forwardDir).normalized;
    }

    public void ResetBall()
    {
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = startPosition;

        hasBounced = false;
        isMoving = false;
    }

    private void FixedUpdate()
    {
        if (!isMoving || hasBounced) return;

        if (currentMode == BowlType.Swing)
        {
            ApplySwing();
        }
    }

    private void ApplySwing()
    {
        elapsedTime += Time.fixedDeltaTime;

        float progress = Mathf.Clamp01(elapsedTime / flightTime);

        float ramp = progress * progress;

        float force = ramp * swingStrength * maxSwingForce * swingDirection;

        rb.AddForce(lateralDir * force, ForceMode.Force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasBounced) return;

        hasBounced = true;

        Vector3 vel = rb.linearVelocity;

        // --- SPIN LOGIC ---
        if (currentMode == BowlType.Spin)
        {
            Vector3 flatForward = new Vector3(vel.x, 0f, vel.z).normalized;
            Vector3 side = Vector3.Cross(Vector3.up, flatForward).normalized;

            float spinForce = spinStrength * maxSpinForce * spinDirection;

            vel += side * spinForce;
        }

        // --- BOUNCE ---
        float minBounce = 4f;
        float bounceMultiplier = 0.6f;

        vel.y = Mathf.Max(Mathf.Abs(vel.y) * bounceMultiplier, minBounce);

        rb.linearVelocity = vel;
        rb.angularVelocity = Vector3.zero;
    }
}