using System;

namespace LedController3Client.Mobile.PhotonLedController
{
    public class PhotonLedControllerCommunicator : IPhotonLedControllerCommunicator
    {
        public event EventHandler<EventArgs<ulong>> CycleTimeRead;
        public event EventHandler<EventArgs<float>> TimeProgressRead;
        public event EventHandler<EventArgs<ColorTimePoint[]>> ColorTimePointsRead;

        public void ReadCycleTime()
        {
            throw new NotImplementedException();
        }

        public void ReadTimeProgress()
        {
            throw new NotImplementedException();
        }

        public void ReadColorTimePoints()
        {
            throw new NotImplementedException();
        }
    }
}
