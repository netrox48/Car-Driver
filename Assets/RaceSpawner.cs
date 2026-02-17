// RaceSpawner.cs
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class RaceSpawner : NetworkBehaviour
{
    public NetworkPrefabRef carPrefab;
    public Transform[] spawnPoints;
    public float lockSeconds = 5f;

    private int nextIndex = 0;

    // Ayný PlayerRef’e birden fazla araba spawn edilmesini engeller
    private readonly HashSet<PlayerRef> _spawnedPlayers = new HashSet<PlayerRef>();

    public void SpawnCarFor(PlayerRef player)
    {
        if (!Object.HasStateAuthority) return;

        if (!carPrefab.IsValid)
        {
            Debug.LogError("RaceSpawner: carPrefab boþ!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("RaceSpawner: spawnPoints boþ!");
            return;
        }

        // Duplicate spawn korumasý
        if (_spawnedPlayers.Contains(player))
            return;

        _spawnedPlayers.Add(player);

        Transform sp = spawnPoints[nextIndex % spawnPoints.Length];
        nextIndex++;

        Runner.Spawn(
            carPrefab,
            sp.position,
            sp.rotation,
            player, // InputAuthority bu player olur
            (runner, obj) =>
            {
                // PlayerRef -> spawned car eþlemesi (çok faydalý)
                runner.SetPlayerObject(player, obj);

                // Sürücüyü ata + baþlangýç lock
                var seat = obj.GetComponent<VehicleSeat>();
                if (seat != null)
                    seat.Server_AssignDriver(player, lockSeconds);

                // Güvenli set (bazý prefab/physics durumlarýnda iþe yarar)
                obj.transform.SetPositionAndRotation(sp.position, sp.rotation);
            }
        );
    }

    // (Opsiyonel) Player çýkýnca temizlemek istersen çaðýrýrsýn
    public void Unregister(PlayerRef player)
    {
        _spawnedPlayers.Remove(player);
    }
}
