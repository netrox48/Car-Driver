using UnityEngine;
using System.Collections;
using Fusion;

public class Checkpoint : NetworkBehaviour
{
    public int checkpointIndex;
    private bool used = false;

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (Runner == null) return;
        if (!Runner.IsServer) return;
        if (CheckpointManager.Instance == null) return;

        CarController car = other.GetComponentInParent<CarController>();
        if (car == null) return;

        bool accepted = CheckpointManager.Instance.TrySetCheckpoint(car, checkpointIndex, transform);

        if (accepted)
        {
            used = true;
            StartCoroutine(ResetUsedFlag());
        }
    }

    private IEnumerator ResetUsedFlag()
    {
        yield return new WaitForSeconds(0.25f);
        used = false;
    }
}
