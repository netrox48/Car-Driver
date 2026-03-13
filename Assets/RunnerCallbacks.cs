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

    private RaceSpawner raceSpawner;

    void Start()
    {
        raceSpawner = FindObjectOfType<RaceSpawner>();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player Joined: " + player);

        if (!(runner.IsServer || runner.IsSharedModeMasterClient))
            return;

        SpawnPlayer(runner, player);
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        raceSpawner = FindObjectOfType<RaceSpawner>();

        if (!(runner.IsServer || runner.IsSharedModeMasterClient))
            return;

        foreach (var p in runner.ActivePlayers)
            SpawnPlayer(runner, p);
    }

    void SpawnPlayer(NetworkRunner runner, PlayerRef player)
    {
        if (raceSpawner == null)
        {
            raceSpawner = FindObjectOfType<RaceSpawner>();
            if (raceSpawner == null)
            {
                Debug.LogError("RaceSpawner bulunamadı!");
                return;
            }
        }

        raceSpawner.SpawnCarFor(runner, player);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (raceSpawner != null)
            raceSpawner.Unregister(player);
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

    void ReturnToMenu()
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