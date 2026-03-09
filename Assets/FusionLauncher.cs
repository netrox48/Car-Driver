using UnityEngine;
using Fusion;
using System.Threading.Tasks;

public class FusionLauncher : MonoBehaviour
{
    [Header("Runner Prefab (NetworkRunner içeren prefab)")]
    [SerializeField] private NetworkRunner runnerPrefab;

    private NetworkRunner runner;
    private bool callbacksAdded;

    [Header("Session")]
    [SerializeField] private string sessionName = "RaceRoom";

    [Header("Scene (RaceArena build index)")]
    [SerializeField] private int sceneBuildIndex = 1;

    private void Awake()
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
                Debug.LogError("FusionLauncher: runnerPrefab atanmadı!");
                return;
            }
            runner = Instantiate(runnerPrefab);
        }

        runner.name = "NetworkRunner";
        DontDestroyOnLoad(runner.gameObject);

        if (runner.GetComponent<NetworkSceneManagerDefault>() == null)
            runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        runner.ProvideInput = true;

        if (!callbacksAdded)
        {
            var cb = runner.GetComponent<RunnerCallbacks>();
            if (cb == null) cb = runner.gameObject.AddComponent<RunnerCallbacks>();
            runner.AddCallbacks(cb);
            callbacksAdded = true;
        }
    }

    public void StartHost()
    {
#if UNITY_WEBGL
        _ = StartGame(GameMode.Shared);
#else
        _ = StartGame(GameMode.Host);
#endif
    }

    public void StartClient()
    {
#if UNITY_WEBGL
        _ = StartGame(GameMode.Shared);
#else
        _ = StartGame(GameMode.Client);
#endif
    }

    private async Task StartGame(GameMode mode)
    {
        EnsureRunner();
        if (runner == null) return;

        var args = new StartGameArgs
        {
            GameMode = mode,
            SessionName = sessionName,
            Scene = SceneRef.FromIndex(sceneBuildIndex),
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
        };

        var result = await runner.StartGame(args);
        if (!result.Ok)
            Debug.LogError($"[Fusion] StartGame failed: {result.ShutdownReason}");
        else
            Debug.Log($"[Fusion] StartGame OK. mode={mode} session={sessionName}");
    }
}