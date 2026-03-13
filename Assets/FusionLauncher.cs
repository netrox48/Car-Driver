using UnityEngine;
using Fusion;

public class FusionLauncher : MonoBehaviour
{
    [Header("Runner Prefab")]
    [SerializeField] private NetworkRunner runnerPrefab;

    private NetworkRunner runner;
    private bool isStartingGame = false;

    [Header("Session Name")]
    [SerializeField] private string sessionName = "RaceRoom";

    [Header("Race Scene Index")]
    [SerializeField] private int sceneBuildIndex = 1;

    private void Start()
    {
        EnsureRunner();
    }

    private void EnsureRunner()
    {
        if (runner != null) return;

        runner = FindObjectOfType<NetworkRunner>();

        if (runner == null)
        {
            if (runnerPrefab == null)
            {
                Debug.LogError("[FusionLauncher] runnerPrefab atanmadı!");
                return;
            }

            runner = Instantiate(runnerPrefab);
            runner.name = "NetworkRunner";
        }

        DontDestroyOnLoad(runner.gameObject);

        if (runner.GetComponent<NetworkSceneManagerDefault>() == null)
            runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        runner.ProvideInput = true;

        RunnerCallbacks callbacks = runner.GetComponent<RunnerCallbacks>();
        if (callbacks != null)
        {
            runner.AddCallbacks(callbacks);
        }
        else
        {
            Debug.LogError("[FusionLauncher] RunnerPrefab üzerinde RunnerCallbacks yok!");
        }
    }

    public void StartRace()
    {
        if (isStartingGame) return;

        StartGame(GameMode.Shared);
    }

    private async void StartGame(GameMode mode)
    {
        EnsureRunner();

        if (runner == null)
            return;

        if (runner.IsRunning)
        {
            Debug.LogWarning("[FusionLauncher] Runner zaten çalışıyor.");
            return;
        }

        isStartingGame = true;

        var args = new StartGameArgs
        {
            GameMode = mode,
            SessionName = sessionName,
            Scene = SceneRef.FromIndex(sceneBuildIndex),
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
        };

        var result = await runner.StartGame(args);

        isStartingGame = false;

        if (!result.Ok)
            Debug.LogError("[Fusion] StartGame Failed: " + result.ShutdownReason);
        else
            Debug.Log("[Fusion] Game Started");
    }
}