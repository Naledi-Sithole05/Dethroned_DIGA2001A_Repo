using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    private CheckpointManager manager;
    private CheckpointManager.Checkpoint checkpoint;
    private bool isStartTrigger;

    public void Setup(CheckpointManager mgr, CheckpointManager.Checkpoint cp, bool start)
    {
        manager = mgr;
        checkpoint = cp;
        isStartTrigger = start;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.TriggerCheckpoint(checkpoint, isStartTrigger);
        }
    }
}

