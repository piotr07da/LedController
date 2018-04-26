
namespace LedController2Client
{
    public interface IMessengerWorker : IMessenger
    {
        bool IsRunning { get; }
        int TaskCount { get; }
        void Start();
        void Stop();
    }
}
