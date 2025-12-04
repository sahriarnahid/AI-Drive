using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CarAgent : Agent
{
    private CarDriver driver;
    private Rigidbody rb;
    private CheckpointManager checkpointManager;
    private Transform spawnPoint;
    private Transform env;

    private void Awake()
    {
        driver = GetComponent<CarDriver>();
        rb = GetComponent<Rigidbody>();

        env = transform.parent;

        // Auto-find manager
        checkpointManager = env.GetComponentInChildren<CheckpointManager>();

        // Auto-find or create spawnpoint
        spawnPoint = env.Find("SpawnPoint");
        if (spawnPoint == null)
        {
            var sp = new GameObject("SpawnPoint").transform;
            sp.SetParent(env, false);
            spawnPoint = sp;
        }
    }

    public override void OnEpisodeBegin()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        driver.StopCompletely();

        // reset local position
        transform.localPosition = spawnPoint.localPosition;
        transform.localRotation = spawnPoint.localRotation;

        checkpointManager.ResetCheckpoints();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Normalized speed
        sensor.AddObservation(rb.velocity.magnitude / driver.maxSpeed);

        // Local velocity (helps turning)
        Vector3 lv = transform.InverseTransformDirection(rb.velocity);
        sensor.AddObservation(lv.x);
        sensor.AddObservation(lv.z);

        // Car forward direction
        sensor.AddObservation(transform.forward);
    }

    public override void OnActionReceived(ActionBuffers a)
    {
        float forward = a.ContinuousActions[0];
        float turn    = a.ContinuousActions[1];

        driver.SetInputs(forward, turn);

        // Small time penalty
        AddReward(-0.0003f);

        // Reward forward movement alignment
        if (rb.velocity.sqrMagnitude > 0.1f)
        {
            float align = Vector3.Dot(transform.forward, rb.velocity.normalized);
            if (align > 0f) AddReward(align * 0.002f);
            if (align < -0.2f) AddReward(-0.002f); // backward penalty
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var c = actionsOut.ContinuousActions;
        c[0] = Input.GetAxis("Vertical");
        c[1] = Input.GetAxis("Horizontal");
    }

    // Called when checkpoint hit
    public void OnCheckpointReached(bool correct)
    {
        AddReward(correct ? +1f : -0.5f);
    }

    // Called on lap finish
    public void OnLapCompleted()
    {
        AddReward(+3f);
        EndEpisode();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            Debug.Log("hit something");
            AddReward(-1f);
            EndEpisode();
        }
    }
}
