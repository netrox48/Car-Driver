using UnityEngine;
using Fusion;

public class FusionLauncher : MonoBehaviour
{
    [Header("Runner Prefab")]
    [SerializeField] private NetworkRunner runnerPrefab;

    private NetworkRunner runner;

    [Header("Session Name")]
    [SerializeField] private string sessionName = "RaceRoom";

    [Header("Race Scene Index")]
    [SerializeField] private int sceneBuildIndex = 1;

    void Start()
    {
        EnsureRunner();
    }

    void EnsureRunner()
    {
        if (runner != null) return;

        runner = FindObjectOfType<NetworkRunner>();

        if (runner == null)
        {
            runner = Instantiate(runnerPrefab);
            runner.name = "NetworkRunner";
        }

        DontDestroyOnLoad(runner.gameObject);

        if (runner.GetComponent<NetworkSceneManagerDefault>() == null)
            runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        runner.ProvideInput = true;

        RunnerCallbacks callbacks = runner.GetComponent<RunnerCallbacks>();
        if (callbacks != null)
            runner.AddCallbacks(callbacks);
    }

    public void StartRace()
    {
        StartGame(GameMode.Shared);
    }

    async void StartGame(GameMode mode)
    {
        EnsureRunner();

        var args = new StartGameArgs
        {
            GameMode = mode,
            SessionName = sessionName,
            Scene = SceneRef.FromIndex(sceneBuildIndex),
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
        };

        var result = await runner.StartGame(args);

        if (!result.Ok)
            Debug.LogError("[Fusion] StartGame Failed: " + result.ShutdownReason);
        else
            Debug.Log("[Fusion] Game Started");
    }
}