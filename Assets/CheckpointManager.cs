using UnityEngine;
using Fusion;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private class CarCheckpointState
    {
        public int nextIndex;
        public Transform lastCheckpoint;
    }

    private Dictionary<NetworkId, CarCheckpointState> carStates = new Dictionary<NetworkId, CarCheckpointState>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private CarCheckpointState GetState(CarController car)
    {
        NetworkObject no = car.GetComponent<NetworkObject>();
        NetworkId id = no != null ? no.Id : default;

        if (!carStates.TryGetValue(id, out var state))
        {
            state = new CarCheckpointState { nextIndex = 0, lastCheckpoint = null };
            carStates[id] = state;
        }

        return state;
    }

    public bool TrySetCheckpoint(CarController car, int checkpointIndex, Transform checkpointTransform)
    {
        if (car == null) return false;

        var state = GetState(car);

        if (checkpointIndex != state.nextIndex)
            return false;

        state.lastCheckpoint = checkpointTransform;
        state.nextIndex++;
        return true;
    }

    public void Respawn(CarController car)
    {
        if (car == null) return;

        NetworkObject no = car.GetComponent<NetworkObject>();
        if (no != null && !no.HasStateAuthority) return;

        var state = GetState(car);
        if (state.lastCheckpoint == null) return;

        Rigidbody rb = car.GetRigidbody();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        car.transform.position = state.lastCheckpoint.position + Vector3.up * 1f;
        car.transform.rotation = state.lastCheckpoint.rotation;
    }
}
