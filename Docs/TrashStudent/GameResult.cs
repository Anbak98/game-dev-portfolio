public interface IBGameResult
{
    void UpdateValue(PointSource pointSource, int amount);
    void SetValue(PointSource pointSource, int value);
    void AddNew<T>(PointSource pointSource, int initValue) where T : class, IResultCallTiming, new();
    void Reset();
    void ResetPrevious();
    void SettleGiveUp();
    void SettleEnd(bool isWin);
    void SettleShutDown();
    Dictionary<PointSource, Result> GetAllResultLog();
    Dictionary<PointSource, Result> GetResultLog();
    List<BGameResult> GetResultHistorys();
}

public class BGameResult : IBGameResult
{
    private readonly Dictionary<PointSource, List<Result>> _resultMap = new();
    private readonly Dictionary<PointSource, Result> _finalizeLog = new();
    private readonly List<BGameResult> _resultHistory;

    private readonly IGameResultFinalizer _finalizer;
    private readonly IGameResultRecorder _recorder;

    public BGameResult(List<BGameResult> resultHistory = null)
    {
        resultHistory ??= new List<BGameResult>();
        _resultHistory = resultHistory;

        _finalizer = new GameResultFinalizer(_resultMap, _finalizeLog);
        _recorder = new GameResultRecorder(_resultMap, _finalizeLog, _resultHistory);
    } 

    public void UpdateValue(PointSource pointSource, int amount) => _recorder.Add(pointSource, amount);
    public void SetValue(PointSource pointSource, int value) => _recorder.Set(pointSource, value);
    public void AddNew<T>(PointSource pointSource, int initValue) where T : class, IResultCallTiming, new()
        => _recorder.AddNew<T>(pointSource, initValue); 
    public void SettleGiveUp() => _finalizer.OnGiveUp();
    public void SettleEnd(bool isWin) => _finalizer.OnEnd(isWin);
    public void SettleShutDown() => _finalizer.OnShutDown();
    public Dictionary<PointSource, Result> GetResultLog() => _recorder.GetResultLog();
    public Dictionary<PointSource, Result> GetAllResultLog()
    {
        Dictionary<PointSource, Result> allResultLog = _recorder.GetHistorysResultLog();

        foreach(var kvp in GetResultLog())
        {
            allResultLog.TryAdd(kvp.Key, kvp.Value);
        }

        return allResultLog;
    }

    public List<BGameResult> GetResultHistorys()
    {
        List<BGameResult> history = _recorder.GetResultHistorys();
        history.Add(this);
        return history;
    }
    public void Reset() => _recorder.Reset();
    public void ResetPrevious() => _recorder.ResetHistory();
}