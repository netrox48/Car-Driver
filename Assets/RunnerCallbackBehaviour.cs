#pragma warning disable UNT0006

using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunnerCallbackBehaviour : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("Return To Menu")]
    [SerializeField] private int menuSceneBuildIndex = 0; // Scenes/UI = 0

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return;

        var rs = FindFirstObjectByType<RaceSpawner>();
        if (rs != null)
            rs.SpawnCarFor(player);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return;

        var rs = FindFirstObjectByType<RaceSpawner>();
        if (rs != null)
            rs.Unregister(player);
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

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"[Fusion] OnShutdown: {shutdownReason}");
        ReturnToMenu();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log($"[Fusion] OnDisconnectedFromServer: {reason}");
        ReturnToMenu();
    }

    private void ReturnToMenu()
    {
        // Zaten UI sahnesindeysek yapma
        if (SceneManager.GetActiveScene().buildIndex == menuSceneBuildIndex)
            return;

        // UI sahnesine dön
        SceneManager.LoadScene(menuSceneBuildIndex);
    }

    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}

#pragma warning restore UNT0006
