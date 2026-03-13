using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class RaceSpawner : MonoBehaviour
{
    public NetworkPrefabRef carPrefab;
    public Transform[] spawnPoints;

    private int nextIndex = 0;
    private readonly HashSet<PlayerRef> spawnedPlayers = new();

    public void SpawnCarFor(NetworkRunner runner, PlayerRef player)
    {
        if (!carPrefab.IsValid) return;

        if (spawnedPlayers.Contains(player))
            return;

        spawnedPlayers.Add(player);

        Transform sp = spawnPoints[nextIndex % spawnPoints.Length];
        nextIndex++;

        Vector3 pos = sp.position + Vector3.up * 0.3f;
        Quaternion rot = sp.rotation;

        runner.Spawn(carPrefab, pos, rot, player, (r, obj) =>
        {
            r.SetPlayerObject(player, obj);

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        });
    }

    public void Unregister(PlayerRef player)
    {
        spawnedPlayers.Remove(player);
    }
}