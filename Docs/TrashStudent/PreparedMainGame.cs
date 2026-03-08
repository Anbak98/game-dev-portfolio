using FullSerializer;
using GameVanilla.Core;
using GameVanilla.Game.Common;
using GameVanilla.Game.Popups;
using System.Collections.Generic;
using TS.Game.Life;
using TS.Game.Result;
using TS.Game.Reward;
using TS.UI;

namespace TS.Core
{
    public class PreparedMainGame : BasePreparedGame
    {
        public Level Level { get; private set; }

        public PreparedMainGame(int stageIdx = -1, List<GameReward> preRewards = null, List<BGameResult> preResults = null) :
            base("Can't bring StageName from Level",
                GameMode.ReachGoal,
                new DefaultGameReward(preRewards),
                new DefaultGameLife(1),
                new BGameResult(preResults))
        {
            _gameType = GameType.Main;
            SetLevel(stageIdx);
            _stageName = Level.IDString;
        }

        private int maxRandomLevel = 5;

        protected override bool TryStartGame()
        {
            base.TryStartGame();

            bool flag = false;

            if (PlayerManager.Instance.Energy.CanStartGame())
            {
                PlayerManager.Instance.Energy.RemoveLife();

                PlayerManager.Instance.Stage.UpdateCurrentLevelTry();

                SceneLoader.Instance.LoadMyAsyncSceneWithFade("GameScene");

                //MiniGameContextHolder.Set(new MiniGameContext(GameMode.ReachGoal, _level));
                //_transition.PerformTransition();
                flag = true;
            }
            else
            {
                UIManager.Instance.OpenPopupImmediate<BuyLivesPopup>("Popups/BuyLivesPopup");

                flag = false;
            }

            return flag;
        }

        protected override bool TryEndGame(bool isWin)
        {
            if (isWin)
            {
                PlayerManager.Instance.Stage.UpdateWinStreak();
            }
            else
            {
                PlayerManager.Instance.Stage.ResetWinStreak();
            }

            return base.TryEndGame(isWin);
        }

        protected override void TryShowPlayPopup()
        {
            base.TryShowPlayPopup();
            UIManager.Instance.OpenPopupImmediate<PlayMainGamePopup>("PlayMainGamePopup", (onOpen) =>
            {
                onOpen.Set(this);
            });
        }

        public void ShowPlayPopup(SMiniGame miniGame)
        {
            UIManager.Instance.OpenPopupImmediate<PlayMainGamePopup>("PlayMainGamePopup", (onOpen) =>
            {
                onOpen.SetIamage(miniGame);
                onOpen.Set(this);
            });
        }

        #region Helper
        private void SetLevel(int stageIdx)
        {
            if (stageIdx == -1)
            {
                stageIdx = PlayerManager.Instance.Stage.CurTargetStage;
            }

            string path = "Levels/" + stageIdx;
            if (FileUtils.FileExists(path) && stageIdx <= PlayerManager.Instance.Stage.MaxLevel)
            {
                Level = FileUtils.LoadJsonFile<Level>(new fsSerializer(), path);
            }
            else
            {
                //임시 랜덤 맵 코드라 나중에 변경 필요
                Level = FileUtils.LoadJsonFile<Level>(new fsSerializer(), "Levels/R_" + UnityEngine.Random.Range(1, maxRandomLevel + 1));
                Level.id = stageIdx;
                Level.SetRandomColorBlockGoals(150);
            }

            Level.SetRandomObstacleByStage();
        }
        #endregion
    }
}
