namespace TradingEngine.Services;

public interface ISystemState
{
    bool IsRunning { get; }
    void Start();
    void Stop();
}
