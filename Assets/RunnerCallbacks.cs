using Fusion;
using Fusion.Sockets;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RunnerCallbacks : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private int menuSceneBuildIndex = 0;

    private RaceSpawner raceSpawner;

    private void FindSpawner()
    {
        if (raceSpawner == null)
            raceSpawner = FindObjectOfType<RaceSpawner>();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[Fusion] Player Joined: {player}");

        if (!(runner.IsServer || runner.IsSharedModeMasterClient))
            return;

        FindSpawner();

        if (raceSpawner == null)
        {
            Debug.LogWarning("[Fusion] RaceSpawner henüz yok, sahne yüklenince spawn denenecek.");
            return;
        }

        raceSpawner.SpawnCarFor(runner, player);
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("[Fusion] SceneLoadDone");

        if (!(runner.IsServer || runner.IsSharedModeMasterClient))
            return;

        FindSpawner();

        if (raceSpawner == null)
        {
            Debug.LogError("[Fusion] RaceSpawner bulunamadı!");
            return;
        }

        foreach (var player in runner.ActivePlayers)
        {
            raceSpawner.SpawnCarFor(runner, player);
        }
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

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        if (SceneManager.GetActiveScene().buildIndex != menuSceneBuildIndex)
            SceneManager.LoadScene(menuSceneBuildIndex);
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        if (SceneManager.GetActiveScene().buildIndex != menuSceneBuildIndex)
            SceneManager.LoadScene(menuSceneBuildIndex);
    }

    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}