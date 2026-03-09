using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class RaceSpawner : MonoBehaviour
{
    public NetworkPrefabRef carPrefab;
    public Transform[] spawnPoints;
    public float lockSeconds = 3f;

    private int nextIndex = 0;
    private readonly HashSet<PlayerRef> spawnedPlayers = new HashSet<PlayerRef>();

    public void SpawnCarFor(NetworkRunner runner, PlayerRef player)
    {
        if (runner == null) return;

        if (!carPrefab.IsValid)
        {
            Debug.LogError("[RaceSpawner] carPrefab boþ/geçersiz! (NetworkProjectConfig->Prefabs + Inspector atamasý)");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[RaceSpawner] spawnPoints boþ!");
            return;
        }

        if (spawnedPlayers.Contains(player))
            return;

        spawnedPlayers.Add(player);

        Transform sp = spawnPoints[nextIndex % spawnPoints.Length];
        nextIndex++;

        Vector3 pos = sp.position + Vector3.up * 0.3f;
        Quaternion rot = sp.rotation;

        Debug.Log($"[RaceSpawner] Spawn {player} at {sp.name}");

        runner.Spawn(carPrefab, pos, rot, player, (r, obj) =>
        {
            r.SetPlayerObject(player, obj);

            var seat = obj.GetComponent<VehicleSeat>();
            if (seat != null && (r.IsServer || r.IsSharedModeMasterClient))
                seat.Server_AssignDriver(player, lockSeconds);

            var rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            obj.transform.SetPositionAndRotation(pos, rot);
        });
    }

    public void Unregister(PlayerRef player)
    {
        spawnedPlayers.Remove(player);
    }
}