using UnityEngine;

public class CarDriver : MonoBehaviour
{
    public float maxSpeed = 20f;
    public float acceleration = 10f;
    public float turnSpeed = 80f;
    public float turnAcceleration = 200f;

    private float currentSpeed = 0f;
    private float currentTurnSpeed = 0f;

    private float forwardInput = 0f;
    private float turnInput = 0f;

    private Rigidbody rb;

    public bool keyboardControl = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!keyboardControl) return;

        SetInputs(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
    }

    public void SetInputs(float forward, float turn)
    {
        forward = Mathf.Max(0f, forward);

        forwardInput = Mathf.Clamp(forward, 0f, 1f);
        turnInput = Mathf.Clamp(turn, -1f, 1f);
    }

    private void FixedUpdate()
    {
        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            forwardInput * maxSpeed,
            acceleration * Time.fixedDeltaTime
        );

        Vector3 forwardVel = transform.forward * currentSpeed;
        rb.velocity = new Vector3(forwardVel.x, rb.velocity.y, forwardVel.z);

        currentTurnSpeed = Mathf.MoveTowards(
            currentTurnSpeed,
            turnInput * turnSpeed,
            turnAcceleration * Time.fixedDeltaTime
        );

        transform.Rotate(0, currentTurnSpeed * Time.fixedDeltaTime, 0);
    }

    public void StopCompletely()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        currentSpeed = 0;
        currentTurnSpeed = 0;
    }
}
