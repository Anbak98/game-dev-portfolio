using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : NetworkBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _profilePrefab;

    public PlayerProfile MyProfile;
    [SerializeField] private List<PlayerProfile> _teamProfiles = new();

    [Networked] public bool Lock { get; private set; } = false;

    //private bool _isPrivate = false;

    public override void Spawned()
    {
        MatchManager.Instance.RoomUI.Room = this;
        MatchManager.Instance.Room = this;

        Runner.AddCallbacks(this);

        Runner.Spawn(_profilePrefab, inputAuthority: Runner.LocalPlayer).GetComponent<PlayerProfile>();
        PlayerPrefs.SetString("RoomCode", Runner.SessionInfo.Name);

        Global.Instance.UIManager.Hide<UILoadingPopup>();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        MatchManager.Instance.RoomUI.SetVisualablePlayPartyButtonOnHost(runner.IsSharedModeMasterClient);

        Global.Instance.UIManager.Hide<UILoadingPopup>();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        MatchManager.Instance.RoomUI.SetVisualablePlayPartyButtonOnHost(runner.IsSharedModeMasterClient);
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_BroadcastUpdatePlayerProfile()
    {
        if (!Lock)
        {
            MyProfile.SetInfo();
            MatchManager.Instance.RoomUI.UpdateUI(_teamProfiles);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SendProfileToAll(PlayerProfile playerProfile)
    {
        if (!Lock)
        {
            if (MyProfile != playerProfile && !_teamProfiles.Contains(playerProfile))
                _teamProfiles.Add(playerProfile);

            MatchManager.Instance.RoomUI.UpdateUIWhenJoinRoom();
            MatchManager.Instance.RoomUI.UpdateUI(_teamProfiles);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_RemoveProfileToAll(PlayerProfile playerProfile)
    {
        if (!Lock)
        {
            if (_teamProfiles.Contains(playerProfile))
                _teamProfiles.Remove(playerProfile);

            if (MatchManager.Instance != null && MatchManager.Instance.RoomUI != null)
                MatchManager.Instance.RoomUI.UpdateUI(_teamProfiles);
        }
    }

    [Networked]
    private int IsReadyCount { get; set; } = 0;

    public IEnumerator BroadcastPlayGame()
    {
        RPC_RequestReady();

        yield return new WaitForSeconds(0.2F);

        while (IsReadyCount < Runner.SessionInfo.PlayerCount)
            yield return null;

        var load = Runner.LoadScene("RuinedCity");

        while(!load.IsDone)
            yield return null;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_RequestReady()
    {
        AudioManager.instance.StopBgm();
        Global.Instance.UIManager.Show<UILoadingPopup>();
        Debug.Log($"{IsReadyCount}/{Runner.SessionInfo.PlayerCount}");

        if (Runner.IsSharedModeMasterClient)
        {
            PlayerPrefsManager.SetGameMode(GameMode.Host);
        }
        else
        {
            PlayerPrefsManager.SetGameMode(GameMode.Client);
        }

        RPC_AcceptReady();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AcceptReady()
    {
        IsReadyCount++;
        Debug.Log($"{IsReadyCount}/{Runner.SessionInfo.PlayerCount}");
    }

    #region NOT USED
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    #endregion
}