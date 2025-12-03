using UnityEngine;

public class CarDriver : MonoBehaviour
{
    [Header("Car Settings")]
    public float maxSpeed = 20f;
    public float acceleration = 10f;
    public float turnSpeed = 80f;
    public float turnAcceleration = 200f;

    private float currentSpeed = 0f;
    private float currentTurnSpeed = 0f;

    private float forwardInput = 0f;
    private float turnInput = 0f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Keyboard control (for testing)
        float forward = Input.GetAxis("Vertical");
        float turn    = Input.GetAxis("Horizontal");
        SetInputs(forward, turn);
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void SetInputs(float forward, float turn)
    {
        forwardInput = Mathf.Clamp(forward, -1f, 1f);
        turnInput    = Mathf.Clamp(turn, -1f, 1f);
    }

    private void Move()
    {
        // Smooth acceleration
        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            forwardInput * maxSpeed,
            acceleration * Time.fixedDeltaTime
        );

        // Apply velocity
        Vector3 forwardVel = transform.forward * currentSpeed;
        rb.velocity = new Vector3(forwardVel.x, rb.velocity.y, forwardVel.z);

        // Smooth turning
        currentTurnSpeed = Mathf.MoveTowards(
            currentTurnSpeed,
            turnInput * turnSpeed,
            turnAcceleration * Time.fixedDeltaTime
        );

        transform.Rotate(0, currentTurnSpeed * Time.fixedDeltaTime, 0);
    }

    public void StopCompletely()
    {
        rb.velocity        = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        currentSpeed       = 0;
        currentTurnSpeed   = 0;
    }
}
