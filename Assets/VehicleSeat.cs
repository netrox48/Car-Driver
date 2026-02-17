// VehicleSeat.cs
using Fusion;
using UnityEngine;

public class VehicleSeat : NetworkBehaviour
{
    [Networked] public PlayerRef Driver { get; private set; }
    [Networked] public TickTimer DriveLockTimer { get; private set; }

    public bool IsOccupied => Driver != PlayerRef.None;

    //  Race: lock yoksa veya lock bittiyse sürüþe izin ver
    public bool CanDrive(NetworkRunner runner)
    {
        return DriveLockTimer.ExpiredOrNotRunning(runner);
    }

    //  OPENWORLD (compile uyumu): InputAuthority sahibi "binmek" isteyince server'a bildirir
    // (Openworld kullanmýyorsan çaðrýlmaz; ama EnterExitVehicle compile hatasýný çözer.)
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestEnter(PlayerRef player)
    {
        if (Driver != PlayerRef.None) return;

        Driver = player;
        Object.AssignInputAuthority(player);

        DriveLockTimer = default;
    }

    //  OPENWORLD (compile uyumu): InputAuthority sahibi "inmek" isteyince server'a bildirir
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestExit(PlayerRef player)
    {
        if (Driver != player) return;

        Driver = PlayerRef.None;
        Object.RemoveInputAuthority();

        DriveLockTimer = default;
    }

    //  RACE: Host/Server direkt sürücü atar ve lock baþlatýr
    public void Server_AssignDriver(PlayerRef player, float lockSeconds)
    {
        if (!Object.HasStateAuthority) return;

        Driver = player;
        Object.AssignInputAuthority(player);

        DriveLockTimer = TickTimer.CreateFromSeconds(Runner, lockSeconds);
    }

    //  Opsiyonel: yarýþ bittiðinde / oyuncu ayrýlýnca temizlemek için
    public void Server_ClearDriver()
    {
        if (!Object.HasStateAuthority) return;

        Driver = PlayerRef.None;
        Object.RemoveInputAuthority();

        DriveLockTimer = default;
    }
}
