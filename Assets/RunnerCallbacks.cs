#pragma warning disable UNT0006

using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunnerCallbacks : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private int menuSceneBuildIndex = 0;

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!(runner.IsServer || runner.IsSharedModeMasterClient))
            return;

        TrySpawnAll(runner);
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        if (!(runner.IsServer || runner.IsSharedModeMasterClient))
            return;

        TrySpawnAll(runner);
    }

    private void TrySpawnAll(NetworkRunner runner)
    {
        var rs = FindFirstObjectByType<RaceSpawner>();
        if (rs == null)
        {
            Debug.LogError("[Spawn] RaceSpawner yok!");
            return;
        }

        foreach (var p in runner.ActivePlayers)
            rs.SpawnCarFor(runner, p);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        var rs = FindFirstObjectByType<RaceSpawner>();
        if (rs != null) rs.Unregister(player);
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

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        ReturnToMenu();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        ReturnToMenu();
    }

    private void ReturnToMenu()
    {
        if (SceneManager.GetActiveScene().buildIndex == menuSceneBuildIndex)
            return;

        SceneManager.LoadScene(menuSceneBuildIndex);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}

#pragma warning restore UNT0006