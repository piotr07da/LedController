using System.Threading;

namespace LedController3Client.Mobile.PhotonLedController
{
    public class PhotonLedControllerPollingService : IPhotonLedControllerPollingService
    {
        private readonly IPhotonLedControllerCommunicator _communicator;

        private bool _run;

        public PhotonLedControllerPollingService(IPhotonLedControllerCommunicator communicator)
        {
            _communicator = communicator;
        }

        public void Start()
        {
            _run = true;
            var t = new Thread(new ThreadStart(() =>
            {
                while(_run)
                {
                    _communicator.ReadTimeProgress();
                    Thread.Sleep(20);
                }
            }));
            t.Start();
        }

        public void Stop()
        {
            _run = false;
        }
    }
}
