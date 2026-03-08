public abstract class GameReward
{
    protected readonly List<IReward> rewards = new();
    protected readonly List<IReward> rewardsLog = new();
    protected readonly List<GameReward> _previousGameRewards = new();

    public GameReward(List<GameReward> previousGameRewards = null)
    {
        _previousGameRewards = previousGameRewards;
    }

    public void ResetCurrent()
    {
        foreach (var reward in rewards)
        {
            reward.Reward = 0;
        }

        rewardsLog.Clear();
    }

    public void ResetPrevious()
    {
        _previousGameRewards?.Clear();
    }

    public virtual List<GameReward> GetAllGameRewardIncludeThisAndPrevious()
    {
        List<GameReward> gameRewards = new List<GameReward>();

        if (_previousGameRewards != null)
        {
            foreach (var previous in _previousGameRewards)
            {
                gameRewards.Add(previous);
            }
        }

        gameRewards.Add(this);

        return gameRewards;
    }

    public virtual List<IReward> GetAllRewardsLogs()
    {
        List<IReward> allRewardsLogs = new List<IReward>();

        foreach (var reward in rewardsLog)
        {
            allRewardsLogs.Add(reward);
        }

        if(_previousGameRewards != null)
        {
            foreach (var previous in _previousGameRewards)
            {
                foreach (var reward in previous.GetRewardLogs())
                {
                    allRewardsLogs.Add(reward);
                }
            }
        }

        return allRewardsLogs;
    }

    public virtual void SendRewardToServerWithClearAndSaveLog()
    {
        foreach (var reward in rewards)
        {
            rewardsLog.Add(reward);

            if(reward is RewardGold gold)
            {
                PlayerManager.Instance.Currency.AddUserGold(gold.Reward, goldSource:"GameReward");
            }
        }

        rewards.Clear();
    }

    public virtual int GetReward<T>() where T : class, IReward
    {
        foreach(var reward in rewards)
        {
            if(reward is T t)
            {
                return t.Reward;
            }
        }

        return 0;
    }

    public virtual int GetRewardLog<T>() where T : class, IReward
    {
        foreach (var reward in rewardsLog)
        {
            if (reward is T t)
            {
                return t.Reward;
            }
        }

        return 0;
    }

    public virtual List<IReward> GetRewardLogs()
    {
        return rewardsLog;
    }

    public virtual void UpdateReward<T>(int amount) where T : class, IReward
    {
        foreach (var reward in rewards)
        {
            if (reward is T t)
            {
                t.Reward += amount;
            }
        }
    }
}