using UnityEngine;
using Fusion;
using System.Threading.Tasks;

public class FusionLauncher : MonoBehaviour
{
    [Header("Runner")]
    public NetworkRunner runnerPrefab;
    private NetworkRunner runner;

    [Header("Session")]
    public string sessionName = "RaceRoom";

    [Header("Scene")]
    public int sceneBuildIndex = 0;

    private RunnerCallbacks callbacks;
    private FusionInputProvider inputProvider;

    private void Awake()
    {
        EnsureRunner();
    }

    private void Start()
    {
    }

    private void EnsureRunner()
    {
        if (runner != null) return;

        runner = FindObjectOfType<NetworkRunner>();
        if (runner == null)
        {
            runner = runnerPrefab != null
                ? Instantiate(runnerPrefab)
                : new GameObject("NetworkRunner").AddComponent<NetworkRunner>();
        }

        runner.name = "NetworkRunner";

        if (runner.GetComponent<NetworkSceneManagerDefault>() == null)
            runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        runner.ProvideInput = true;

        if (callbacks == null)
            callbacks = new RunnerCallbacks();

        runner.AddCallbacks(callbacks);

        if (inputProvider == null)
            inputProvider = new FusionInputProvider();

        runner.AddCallbacks(inputProvider);
    }

    public void StartHost()
    {
        _ = StartGame(GameMode.Host);
    }

    public void StartClient()
    {
        _ = StartGame(GameMode.Client);
    }

    private async Task StartGame(GameMode mode)
    {
        EnsureRunner();

        var args = new StartGameArgs
        {
            GameMode = mode,
            SessionName = sessionName,
            Scene = SceneRef.FromIndex(sceneBuildIndex),
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
        };

        await runner.StartGame(args);
    }
}
