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

    // ✅ NetworkId yerine PlayerRef ile takip (en sağlam)
    private readonly Dictionary<PlayerRef, CarCheckpointState> states = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private bool TryGetPlayer(CarController car, out PlayerRef player)
    {
        player = PlayerRef.None;
        if (car == null) return false;

        var no = car.GetComponent<NetworkObject>();
        if (no == null) return false;

        player = no.InputAuthority;
        return player != PlayerRef.None;
    }

    private CarCheckpointState GetState(PlayerRef player)
    {
        if (!states.TryGetValue(player, out var s))
        {
            s = new CarCheckpointState { nextIndex = 0, lastCheckpoint = null };
            states[player] = s;
        }
        return s;
    }

    /// <summary>
    /// (Opsiyonel ama önerilir) Spawn olur olmaz ilk checkpoint'i spawnpoint yapar.
    /// Böylece checkpoint görmeden de R ile geri dönebilirsin.
    /// </summary>
    public void SetInitialCheckpoint(CarController car, Transform spawnPoint)
    {
        if (!TryGetPlayer(car, out var p)) return;
        var s = GetState(p);
        if (s.lastCheckpoint == null)
            s.lastCheckpoint = spawnPoint;
    }

    /// <summary>
    /// Checkpoint sırası: 0,1,2... şeklinde olmalı.
    /// Yanlış sıraysa false döner (kayıt etmez).
    /// </summary>
    public bool TrySetCheckpoint(CarController car, int checkpointIndex, Transform checkpointTransform)
    {
        if (!TryGetPlayer(car, out var p)) return false;

        var s = GetState(p);

        // Debug (istersen sonra silebilirsin)
        Debug.Log($"[CP-MGR] player={p} hit={checkpointIndex} expected={s.nextIndex}");

        if (checkpointIndex != s.nextIndex)
            return false;

        s.lastCheckpoint = checkpointTransform;
        s.nextIndex++;

        Debug.Log($"[CP-MGR] SET OK -> next={s.nextIndex}");
        return true;
    }

    /// <summary>
    /// Respawn sadece authority tarafında yapılmalı (Host server veya WebGL Shared master).
    /// </summary>
    public void Respawn(CarController car)
    {
        if (car == null) return;

        var runner = FindObjectOfType<NetworkRunner>();
        if (runner == null) return;

        // ✅ Host: server, WebGL Shared: master karar verir
        if (!(runner.IsServer || runner.IsSharedModeMasterClient))
            return;

        if (!TryGetPlayer(car, out var p)) return;

        var s = GetState(p);

        if (s.lastCheckpoint == null)
        {
            Debug.Log("[RESPAWN] lastCheckpoint NULL (checkpoint alınmamış)");
            return;
        }

        var rb = car.GetRigidbody();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        car.transform.SetPositionAndRotation(
            s.lastCheckpoint.position + Vector3.up * 1f,
            s.lastCheckpoint.rotation
        );

        Debug.Log("[RESPAWN] OK");
    }
}