using Fusion;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayGameListener : MonoBehaviour
{
    public NetworkRunner _runner;

    private const string PlaySceneName = "PlayStage(MVP)";
    private const string SessionName = "HostGame";

    public bool IsStarted = false;
    public bool IsHost = false;

    [Networked]
    private int IsReadyCount { get; set; } = 0; 

    public void Start()
    {
        _runner = gameObject.AddGetComponent<NetworkRunner>();
    }

    public void Update()
    {
        if (IsStarted)
        {
            StartCoroutine(SwitchToHostMode());
            IsStarted = false;
        }
    }

    public IEnumerator BroadcastPlayGame()
    {
        RPC_RequestReady();

        while (IsReadyCount < _runner.SessionInfo.PlayerCount)
            yield return null;

        _runner.LoadScene(SessionName);
    } 

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_RequestReady()
    {
        RPC_AcceptReady();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AcceptReady()
    {
        IsReadyCount++;
        PlayerPrefsManager.SetGameMode(_runner.GameMode);
    }

    public IEnumerator SwitchToHostMode()
    {
        // 씬 로드
        var asyncLoad = SceneManager.LoadSceneAsync(PlaySceneName);
        while (!asyncLoad.isDone)
            yield return null;

        // 새로운 Runner 생성
        var runnerGO = new GameObject("Runner (Host)");
        var newRunner = runnerGO.AddComponent<NetworkRunner>();
        newRunner.ProvideInput = true;

        // 씬 매니저 필요
        var sceneManager = runnerGO.AddComponent<NetworkSceneManagerDefault>();

        //if(IsHost)
        //{
            yield return newRunner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.AutoHostOrClient,
                SessionName = SessionName,
                Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
                SceneManager = sceneManager
            });
        //}
        //else
        //{
        //    yield return ClientTryConnect(newRunner, sceneManager);

        //}

        yield return _runner.Shutdown();
    }

    public IEnumerator SwitchToStage()
    {
        // 씬 로드
        var asyncLoad = SceneManager.LoadSceneAsync(PlaySceneName);
        while (!asyncLoad.isDone)
            yield return null;

        // 새로운 Runner 생성
        var runnerGO = new GameObject("Runner (Host)");
        var newRunner = runnerGO.AddComponent<NetworkRunner>();
        newRunner.ProvideInput = true;

        // 씬 매니저 필요
        var sceneManager = runnerGO.AddComponent<NetworkSceneManagerDefault>();

        //if(IsHost)
        //{
        yield return newRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = SessionName,
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = sceneManager
        });
        //}
        //else
        //{
        //    yield return ClientTryConnect(newRunner, sceneManager);

        //}

        yield return _runner.Shutdown();
    }

    //public async Task ClientTryConnect(NetworkRunner newRunner, NetworkSceneManagerDefault sceneManager)
    //{

    //    StartGameResult result;
    //    do
    //    {
    //        result = await newRunner.StartGame(new StartGameArgs
    //        {
    //            GameMode = GameMode.Client,
    //            SessionName = SessionName,
    //            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
    //            SceneManager = sceneManager
    //        });

    //    } while (!result.Ok);
    //}
}
