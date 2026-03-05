using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class RaceSpawner : NetworkBehaviour
{
    [Header("Cars (index = CarId)")]
    public NetworkPrefabRef[] carPrefabs;

    public Transform[] spawnPoints;
    public float lockSeconds = 5f;

    private int nextIndex = 0;
    private readonly HashSet<PlayerRef> _spawnedPlayers = new HashSet<PlayerRef>();

    public void SpawnCarFor(PlayerRef player, int carId)
    {
        if (!Object.HasStateAuthority) return;

        if (carPrefabs == null || carPrefabs.Length == 0)
        {
            Debug.LogError("RaceSpawner: carPrefabs boþ!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("RaceSpawner: spawnPoints boþ!");
            return;
        }

        if (_spawnedPlayers.Contains(player))
            return;

        carId = Mathf.Clamp(carId, 0, carPrefabs.Length - 1);

        NetworkPrefabRef selectedPrefab = carPrefabs[carId];
        if (!selectedPrefab.IsValid)
        {
            Debug.LogError($"RaceSpawner: carPrefabs[{carId}] geçersiz!");
            return;
        }

        _spawnedPlayers.Add(player);

        Transform sp = spawnPoints[nextIndex % spawnPoints.Length];
        nextIndex++;

        Runner.Spawn(
            selectedPrefab,
            sp.position,
            sp.rotation,
            player,
            (runner, obj) =>
            {
                runner.SetPlayerObject(player, obj);

                var seat = obj.GetComponent<VehicleSeat>();
                if (seat != null)
                    seat.Server_AssignDriver(player, lockSeconds);

                obj.transform.SetPositionAndRotation(sp.position, sp.rotation);
            }
        );
    }

    public void Unregister(PlayerRef player)
    {
        _spawnedPlayers.Remove(player);
    }
}