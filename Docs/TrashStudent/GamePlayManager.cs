namespace TS.Core
{
    public static class GamePlayManager
    {
        public static IPreparedGame Prepared { get; private set; }

        public static IPreparedGame PrepareNextGame()
        {

            IPreparedGame nextPreparedGame = null;

            if (MiniGameStageMap.Instance.TryGetMiniGameByIdx(PlayerManager.Instance.Stage.CurTargetStage, out var stageInfo))
            {
                if (PrepareMiniGame(out var mini, game: stageInfo.EMiniGame, mode: GameMode.ReachGoal))
                {
                    nextPreparedGame = mini;
                }
            }
            else
            {
                if (PrepareMainGame(out var main))
                {
                    nextPreparedGame = main;
                }
            }

            return nextPreparedGame;
        }

        public static bool PrepareMiniGame(out PreparedMiniGame preparedMiniGame, EMiniGame game = EMiniGame.None, GameMode mode = GameMode.None)
        {
            preparedMiniGame = null;

            if (MiniGameStageMap.Instance.TryGetMiniGameByType(game, out var stageInfo))
            {
                List<GameReward> gameRewards = null;
                List<GameResult> gameResults = null ;

                if (Prepared != null)
                {
                    gameRewards = Prepared.GameReward.GetAllGameRewardIncludeThisAndPrevious();
                    gameResults = Prepared.GameResult.GetAllGameRewardIncludeThisAndPrevious();
                }

                preparedMiniGame = new(stageInfo, mode, gameRewards, gameResults);
                Prepared = preparedMiniGame;
                return true;
            }

            return false;
        }

        public static bool PrepareMainGame(out PreparedMainGame preparedMainGame, int stageIdx = -1)
        {

            if (stageIdx == -1)
            {
                stageIdx = PlayerManager.Instance.Stage.CurTargetStage;
            }

            preparedMainGame = null;

            if (!MiniGameStageMap.Instance.TryGetMiniGameByIdx(stageIdx, out var stageInfo))
            {
                List<GameReward> gameRewards = null;
                List<GameResult> gameResults = null;

                if (Prepared != null)
                {
                    gameRewards = Prepared.GameReward.GetAllGameRewardIncludeThisAndPrevious();
                    gameResults = Prepared.GameResult.GetAllGameRewardIncludeThisAndPrevious();
                }

                preparedMainGame = new PreparedMainGame(stageIdx, gameRewards, gameResults);
                Prepared = preparedMainGame;
                return true;
            }

            return false;
        }

        public static void ClearGame()
        {
            Prepared.ResetGame();
            Prepared.ResetPrevious();
        }
    }
}
