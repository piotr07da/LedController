using System;

namespace LedController3Client.Mobile.PhotonLedController
{
    public class PhotonLedControllerCommunicator : IPhotonLedControllerCommunicator
    {
        public event EventHandler<EventArgs<int>> CycleTimeRead;
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

        public void WriteCycleTime(int cycleTime)
        {
            throw new NotImplementedException();
        }

        public void WriteTimeProgress(float timeProgress)
        {
            throw new NotImplementedException();
        }

        public void WriteColorTimePointColor(int id, ColorTimePointColor color)
        {
            throw new NotImplementedException();
        }

        public void WriteColorTimePointTime(int id, float time)
        {
            throw new NotImplementedException();
        }
    }
}
