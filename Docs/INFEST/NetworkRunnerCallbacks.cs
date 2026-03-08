using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace INFEST.Game
{
    public class NetworkRunnerCallbacks : MonoBehaviour, INetworkRunnerCallbacks
    {
        [Header("Input Related")]
        [SerializeField] private PlayerInputActionHandler _playerInputActionHandler;
        [SerializeField] private InputManager _InputManager;

        public void Start()
        {
            //Cursor.lockState = CursorLockMode.Locked;
        }

        public void Update()
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            //    Cursor.lockState = CursorLockMode.Locked;
            //}
            //if (Input.GetKeyDown(KeyCode.Escape))
            //{
            //    Cursor.lockState = CursorLockMode.None;
            //}
            //if (Input.GetKeyDown(KeyCode.F))
            //{
            //    Screen.fullScreen = true;
            //}
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Global.Instance.UIManager.Hide<UILoading>();

            if (runner.LocalPlayer == player)
            {
                _InputManager.Init();
                _playerInputActionHandler.Init();
            }

            if (runner.IsServer)
            {
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                NetworkGameManager.Instance.gamePlayers.RemovePlayer(player);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            var data = _playerInputActionHandler.GetNetworkInput();

            if (data != null)
            {
                input.Set(data.Value);
            }
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    }
}
