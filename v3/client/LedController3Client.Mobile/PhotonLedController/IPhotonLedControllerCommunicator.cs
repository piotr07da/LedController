
using System;

namespace LedController3Client.Mobile.PhotonLedController
{
    public interface IPhotonLedControllerCommunicator
    {
        event EventHandler<EventArgs<ulong>> CycleTimeRead;
        event EventHandler<EventArgs<float>> TimeProgressRead;
        event EventHandler<EventArgs<ColorTimePoint[]>> ColorTimePointsRead;

        void ReadCycleTime();
        void ReadTimeProgress();
        void ReadColorTimePoints();
    }
}
