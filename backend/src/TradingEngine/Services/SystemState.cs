namespace TradingEngine.Services;

public class SystemState : ISystemState
{
    private volatile bool _isRunning = false;

    public bool IsRunning => _isRunning;

    public void Start() => _isRunning = true;

    public void Stop() => _isRunning = false;
}
