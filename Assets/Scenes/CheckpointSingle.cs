using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    [HideInInspector] public CheckpointManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Car")) return;

        manager?.PlayerThroughCheckpoint(this);
    }
}
