using GameVanilla.Game.Popups;
using NUnit.Framework;
using System.Collections.Generic;
using TS.Game.Life;
using TS.Game.Result;
using TS.Game.Reward;
using UnityEngine;

namespace TS.Core
{
    public class PreparedMiniGame : BasePreparedGame
    {
        public SMiniGame Info { get; private set; }

        public PreparedMiniGame(SMiniGame info, GameMode mode, List<GameReward> preRewards = null, List<BGameResult> preResults = null)
            : base(info.Name, mode,
                  new DefaultGameReward(preRewards),
                  new DefaultGameLife(info.MaxLifeCount),
                  new BGameResult(preResults))
        {
            _gameType = GameType.Mini;
            Info = info;
        }

        protected override bool TryStartGame()
        {
            base.TryStartGame();

            switch (GameMode)
            {
                case GameMode.Infinity:
                    return StartGameFromAlbum();
                case GameMode.ReachGoal:
                    return StartGameFromMain();
                case GameMode.None:
                    break;
            }

            return false;
        }

        protected override void TryShowPlayPopup()
        {
            base.TryShowPlayPopup();
            UIManager.Instance.OpenPopupImmediate<PlayMiniGamePopup>("PlayMiniGamePopup", (System.Action<PlayMiniGamePopup>)((onOpen) =>
            {
                onOpen.Set(this);
            }));
        }

        private bool StartGameFromAlbum()
        {
            if (PlayerManager.Instance.Energy.CanStartMiniGame())
            {
                PlayerManager.Instance.Energy.RemoveTicket();

                SceneLoader.Instance.LoadMyAsyncSceneWithFade(Info.SceneName);

                return true;
            }
            else
            {
                UIManager.Instance.OpenPopupImmediate<BuyTicketsPopup>("Popups/BuyTicketsPopup");

                return false;
            }
        }

        private bool StartGameFromMain()
        {
            if (PlayerManager.Instance.Energy.CanStartGame())
            {
                PlayerManager.Instance.Energy.RemoveLife();

#if UNITY_EDITOR
                Utility.DebugLog("Test용으로 150스테이지 클리어 유무를 초기화 하였습니다. 나중에 지워주세요.");
                PlayerPrefs.SetInt(Info.Name, 0);
#endif

                if (PlayerManager.Instance.Stage.CurTargetStage == Info.RequiredStage && PlayerPrefs.GetInt(Info.Name, 0) == 0)
                {
                    UIManager.Instance.OpenPopupImmediate<UI_WebtoonPlayer>("UI_WebtoonPlayer", (popup) =>
                    {
                        popup.SetWebtoon(Info.Webtoon);
                        popup.OnComplete += () => SceneLoader.Instance.LoadMyAsyncSceneWithFade(Info.SceneName);
                    });
                    PlayerPrefs.SetInt(Info.Name, 1);
                    return true;
                }

                SceneLoader.Instance.LoadMyAsyncSceneWithFade(Info.SceneName);
                return true;
            }
            else
            {
                UIManager.Instance.OpenPopupImmediate<BuyLivesPopup>("Popups/BuyLivesPopup");

                return false;
            }
        }
    }
}
