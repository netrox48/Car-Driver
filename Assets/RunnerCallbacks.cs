using Fusion;
using Fusion.Sockets;
using UnityEngine;
using System.Collections.Generic;

public class RunnerCallbacks : MonoBehaviour, INetworkRunnerCallbacks
{
    private RaceSpawner raceSpawner;

    void Start()
    {
        raceSpawner = FindObjectOfType<RaceSpawner>();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!(runner.IsServer || runner.IsSharedModeMasterClient))
            return;

        SpawnPlayer(runner, player);
    }

    void SpawnPlayer(NetworkRunner runner, PlayerRef player)
    {
        if (raceSpawner == null)
            raceSpawner = FindObjectOfType<RaceSpawner>();

        raceSpawner.SpawnCarFor(runner, player);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        raceSpawner?.Unregister(player);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new CarInputData
        {
            Horizontal = Input.GetAxis("Horizontal"),
            Vertical = Input.GetAxis("Vertical"),
            Brake = Input.GetKey(KeyCode.Space),
            RespawnHeld = Input.GetKey(KeyCode.R)
        };

        input.Set(data);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}