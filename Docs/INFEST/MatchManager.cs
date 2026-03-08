using Fusion;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchManager : SingletonBehaviour<MatchManager>
{
    public Room Room;
    public ScreenRoom RoomUI;
    public NetworkRunner Runner;
    public NetworkObject RoomPrefab;

    public GameType SelectedGameType = GameType.BossHunt;
    public GameMap SelectedGameMap = GameMap.MVP;

    public enum Game
    {
        Old,
        New
    }

    public enum GameType
    {
        BossHunt,
        WaveBattleOneVSOne,
        WaveBattleFourVSFour,
    }

    public enum GameMap
    {
        MVP,
        RuinedCity,
        Underground
    }

    public async void QuickMatch()
    {
        Global.Instance.UIManager.Show<UILoadingPopup>();

        await Task.Delay(1000);
        foreach (var runner in FindObjectsOfType<NetworkRunner>())
        {
            await runner.Shutdown();
        }

        if (Runner != null)
            await Runner.Shutdown();

        // 货肺款 Runner 积己
        var runnerGO = new GameObject("Runner (Shared)");
        Runner = runnerGO.AddComponent<NetworkRunner>();
        runnerGO.AddComponent<PlayGameListener>();

        var customProps = new Dictionary<string, SessionProperty>();

        customProps["map"] = (int)SelectedGameMap;
        customProps["type"] = (int)SelectedGameType;

        var result = await Runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionProperties = customProps,
            SceneManager = gameObject.AddGetComponent<NetworkSceneManagerDefault>(),
            EnableClientSessionCreation = false,
        });

        if (!result.Ok)
            CreateNewSession(true);
        else
        {
            RoomUI.UpdateUIWhenJoinRoom();
        }
    }

    public async void CreateNewSession(bool IsPublic, string code = "")
    {
        Global.Instance.UIManager.Show<UILoadingPopup>();

        await Task.Delay(1000);
        foreach (var runner in FindObjectsOfType<NetworkRunner>())
        {
            await runner.Shutdown();
        }

        do {
            if (Runner != null)
                await Runner.Shutdown();

            // 货肺款 Runner 积己
            var runnerGO = new GameObject("Runner (Shared)");
            Runner = runnerGO.AddComponent<NetworkRunner>();
            runnerGO.AddComponent<PlayGameListener>();


            var customProps = new Dictionary<string, SessionProperty>();

            customProps["map"] = (int)SelectedGameMap;
            customProps["type"] = (int)SelectedGameType;

            if (code == "")
                code = GenerateSessionCode();

            await Runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Shared,
                SessionName = code,
                IsVisible = IsPublic,
                SessionProperties = customProps,
                SceneManager = gameObject.AddGetComponent<NetworkSceneManagerDefault>()
            });
        } while (Runner.SessionInfo.PlayerCount > 1);

        if (Runner.IsSharedModeMasterClient)
        {
            Runner.Spawn(RoomPrefab);
        }
    }

    public async void PlayerTutorial()
    {
        Global.Instance.UIManager.Show<UILoadingPopup>();

        foreach (var runner in FindObjectsOfType<NetworkRunner>())
        {
            await runner.Shutdown();
        }

        do
        {
            if (Runner != null)
                await Runner.Shutdown();

            // 货肺款 Runner 积己
            var runnerGO = new GameObject("Runner (Single)");
            Runner = runnerGO.AddComponent<NetworkRunner>();
            runnerGO.AddComponent<PlayGameListener>();


            var customProps = new Dictionary<string, SessionProperty>();

            customProps["map"] = (int)SelectedGameMap;
            customProps["type"] = (int)SelectedGameType;

            await Runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Single,
                SessionName = GenerateSessionCode(),
                IsVisible = false,
                IsOpen = false,
                SessionProperties = customProps,
                SceneManager = gameObject.AddGetComponent<NetworkSceneManagerDefault>()
            });
        } while (Runner.SessionInfo.PlayerCount > 1);

        PlayerPrefs.SetInt("GameMode", (int)GameMode.Single);
        await Runner.LoadScene("Tutorial");
        AudioManager.instance.StopBgm();
    }

    public async void PlayerSoloGame()
    {
        AudioManager.instance.StopBgm();
        Global.Instance.UIManager.Show<UILoadingPopup>();

        await Task.Delay(1000);
        foreach (var runner in FindObjectsOfType<NetworkRunner>())
        {
            await runner.Shutdown();
        }
        do
        {
            if (Runner != null)
                await Runner.Shutdown();

            // 货肺款 Runner 积己
            var runnerGO = new GameObject("Runner (Single)");
            Runner = runnerGO.AddComponent<NetworkRunner>();
            runnerGO.AddComponent<PlayGameListener>();


            var customProps = new Dictionary<string, SessionProperty>();

            customProps["map"] = (int)SelectedGameMap;
            customProps["type"] = (int)SelectedGameType;

            await Runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Single,
                SessionName = GenerateSessionCode(),
                IsVisible = false,
                IsOpen = false,
                SessionProperties = customProps,
                SceneManager = gameObject.AddGetComponent<NetworkSceneManagerDefault>()
            });
        } while (Runner.SessionInfo.PlayerCount > 1);

        PlayerPrefs.SetInt("GameMode", (int)GameMode.Single);
        await Runner.LoadScene("RuinedCity");
    }

    public void PlayPartyGame()
    {
        if (Runner.IsSharedModeMasterClient)
        {
            StartCoroutine(Room.BroadcastPlayGame());
            AnalyticsManager.analyticsBeforeInGame(Runner.SessionInfo.PlayerCount * 10 + 0, 1);
        }
    }

    public bool JoinSession(string code)
    {
        JoinSessionRunner(code);

        if (Runner == null) 
            return false;

        return true;
    }

    private async void JoinSessionRunner(string code)
    {
        await Task.Delay(1000);

        if (Runner != null)
            await Runner.Shutdown();

        // 货肺款 Runner 积己
        var runnerGO = new GameObject("Runner (Shared)");
        Runner = runnerGO.AddComponent<NetworkRunner>();

        await Runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = code,
            SceneManager = gameObject.AddGetComponent<NetworkSceneManagerDefault>()
        });

        if (Runner.SessionInfo.PlayerCount == 1)
            await Runner.Shutdown();
    }

    private string GenerateSessionCode(int length = 6)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ123456789";
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            sb.Append(chars[Random.Range(0, chars.Length)]);
        }

        return sb.ToString();
    }
}
