using UnityEngine;
using Fusion;

public class CheckpointTrigger : MonoBehaviour
{
    [Header("0'dan başlayacak! (0,1,2,3...)")]
    public int checkpointIndex = 0;

    private void OnTriggerEnter(Collider other)
    {
        var car = other.GetComponentInParent<CarController>();
        if (car == null) return;

        var runner = FindObjectOfType<NetworkRunner>();
        if (runner == null) return;

        // ✅ Sadece server/master checkpoint'i sayar
        if (!(runner.IsServer || runner.IsSharedModeMasterClient))
            return;

        bool ok = CheckpointManager.Instance != null &&
                  CheckpointManager.Instance.TrySetCheckpoint(car, checkpointIndex, transform);

        Debug.Log($"[CP] {name} index={checkpointIndex} ok={ok}");
    }
}