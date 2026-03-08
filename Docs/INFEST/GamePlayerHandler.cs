using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace INFEST.Game
{
    public struct PlayerGameProfile : INetworkStruct
    {
        public NetworkString<_16> nickname;
        public JOB job;
    }

    public class GamePlayerHandler : NetworkBehaviour
    {
        [Header("Prefab For Runner.Spawn()")]
        [SerializeField] private NetworkPrefabRef _playerPrefab;

        public Action OnValueChanged;

        public List<Transform> PlayerSpawnPoints;

        [Networked, Capacity(16), OnChangedRender(nameof(OnValueChangedInvoke)), UnitySerializeField]
        private NetworkDictionary<PlayerRef, NetworkId> PlayerObjs => default;

        [Networked, Capacity(16), OnChangedRender(nameof(OnValueChangedInvoke)), UnitySerializeField]
        private NetworkDictionary<PlayerRef, PlayerGameProfile> PlayerGameProfiles => default;

        [Networked, Capacity(16), OnChangedRender(nameof(OnValueChangedInvoke)), UnitySerializeField]
        private NetworkDictionary<PlayerRef, int> PlayerGameKillCounts => default;

        [Networked, Capacity(16), OnChangedRender(nameof(OnValueChangedInvoke)), UnitySerializeField]
        private NetworkDictionary<PlayerRef, int> PlayerGameDeathCounts => default;

        [Networked, Capacity(16), OnChangedRender(nameof(OnValueChangedInvoke)), UnitySerializeField]
        private NetworkDictionary<PlayerRef, int> PlayerGameGoldCounts => default;


        public override void Spawned()
        {
            base.Spawned();
            string nickname = PlayerPrefsManager.GetNickname();
            JOB job = PlayerPrefsManager.GetJob();

            RPC_RequestSpawnPlayer(Runner.LocalPlayer, nickname, job);
        }

        private void OnValueChangedInvoke() => OnValueChanged?.Invoke();

        public bool IsValid(PlayerRef player)
        {
            return PlayerObjs.ContainsKey(player);
        }

        public void AddPlayerObj(PlayerRef player, NetworkId playerObjId)
        {
            PlayerObjs.Add(player, playerObjId);
        }

        public void AddKillCount(PlayerRef player, int count)
        {
            PlayerGameKillCounts.Set(player, PlayerGameKillCounts.Get(player) + count);
        }

        public void AddDeathCount(PlayerRef player, int count)
        {
            PlayerGameDeathCounts.Set(player, PlayerGameDeathCounts.Get(player) + count);
        }

        public void AddGoldCount(PlayerRef player, int count)
        {
            PlayerGameGoldCounts.Set(player, PlayerGameGoldCounts.Get(player) + count);
        }
        public void SetGoldCount(PlayerRef player, int amount)
        {
            PlayerGameGoldCounts.Set(player, amount);
        }

        public int GetPlayerCount() => PlayerObjs.Count;
        public Player GetPlayerObj(PlayerRef player) => Runner.FindObject(PlayerObjs.Get(player)).GetComponent<Player>();
        public PlayerGameProfile GetProfile(PlayerRef player) => PlayerGameProfiles.Get(player);
        public int GetKillCount(PlayerRef player) => PlayerGameKillCounts.Get(player);
        public int GetDeathCount(PlayerRef player) => PlayerGameDeathCounts.Get(player);
        public int GetGoldCount(PlayerRef player) => PlayerGameGoldCounts.Get(player);

        public List<PlayerRef> GetPlayerRefs()
        {
            List<PlayerRef> refs = new();
            foreach (var playerObj in PlayerObjs)
            {
                refs.Add(playerObj.Key);                
            }
            return refs;
        }

        public void RemovePlayer(PlayerRef player)
        {
            if (Runner.IsServer)
            {
                PlayerObjs.Remove(player);
                PlayerGameProfiles.Remove(player);
                PlayerGameKillCounts.Remove(player);
                PlayerGameDeathCounts.Remove(player);
                PlayerGameGoldCounts.Remove(player);

                Debug.Log("Removed");
            }
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_RequestSpawnPlayer(PlayerRef player, string nickname, JOB job)
        {
            PlayerGameProfiles.Add(player, new PlayerGameProfile());
            PlayerGameKillCounts.Add(player, 0);
            PlayerGameDeathCounts.Add(player, 0);
            PlayerGameGoldCounts.Add(player, 0);
            PlayerGameProfiles.Set(player, new PlayerGameProfile()
            {
                nickname = nickname,
                job = job
            });

            Runner.Spawn(_playerPrefab, NetworkGameManager.Instance.gamePlayers.PlayerSpawnPoints[0].position, inputAuthority: player);
        }
    }
}