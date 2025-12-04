using UnityEngine;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour
{
    private List<CheckpointSingle> checkpoints = new List<CheckpointSingle>();
    private CarAgent agent;
    private int nextIndex = 0;

    private void Awake()
    {
        Transform env = transform.parent;

        agent = env.GetComponentInChildren<CarAgent>();

        Transform checkpointRoot = env.Find("Checkpoints");

        if (checkpointRoot == null)
        {
            Debug.LogError("Environment missing 'Checkpoints' object!");
            return;
        }

        checkpoints = new List<CheckpointSingle>(checkpointRoot.GetComponentsInChildren<CheckpointSingle>());

        foreach (var cp in checkpoints)
            cp.manager = this;

        Debug.Log("Checkpoints found: " + checkpoints.Count);
    }

    public void PlayerThroughCheckpoint(CheckpointSingle cp)
    {
        int hitIndex = checkpoints.IndexOf(cp);

        Debug.Log($"Hit CP {hitIndex}, expected {nextIndex}");

        if (hitIndex == nextIndex)
        {
            agent.OnCheckpointReached(true);
            nextIndex++;

            if (nextIndex >= checkpoints.Count)
            {
                nextIndex = 0;
                agent.OnLapCompleted();
            }
        }
        else
        {
            agent.OnCheckpointReached(false);
        }
    }

    public void ResetCheckpoints()
    {
        nextIndex = 0;
    }
}
