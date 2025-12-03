using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CarAgent : Agent
{
    public CarDriver driver;

    public override void OnEpisodeBegin()
    {
        driver.StopCompletely();
        // Reset position & rotation here
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forward = actions.ContinuousActions[0];
        float turn    = actions.ContinuousActions[1];

        driver.SetInputs(forward, turn);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(driver.GetComponent<Rigidbody>().velocity.magnitude);
        sensor.AddObservation(transform.forward);
    }
}
