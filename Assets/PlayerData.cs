using Fusion;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{
    [Networked] private int CarId { get; set; }
    [Networked] private bool SpawnedCar { get; set; }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetCarId(int id)
    {
        CarId = id;
        SpawnedCar = false;
    }

    public override void Spawned()
    {
        // Local oyuncu ise seçtiði arabayý server’a bildir
        if (Object.HasInputAuthority)
            RPC_SetCarId(SelectionStore.SelectedCarId);
    }

    public override void FixedUpdateNetwork()
    {
        // Sadece server araba spawn eder
        if (!Object.HasStateAuthority) return;

        if (SpawnedCar) return;

        var rs = FindFirstObjectByType<RaceSpawner>();
        if (rs == null) return;

        rs.SpawnCarFor(Object.InputAuthority, CarId);
        SpawnedCar = true;
    }
}