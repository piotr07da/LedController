using System;

namespace LedController3Client.Mobile.PhotonLedController
{
    public interface IPhotonLedControllerCommunicator
    {
        event EventHandler<EventArgs<int>> CycleTimeRead;
        event EventHandler<EventArgs<float>> TimeProgressRead;
        event EventHandler<EventArgs<ColorTimePoint[]>> ColorTimePointsRead;

        void ReadCycleTime();
        void ReadTimeProgress();
        void ReadColorTimePoints();

        void WriteCycleTime(int cycleTime);
        void WriteTimeProgress(float timeProgress);
        void WriteColorTimePointColor(int id, ColorTimePointColor color);
        void WriteColorTimePointTime(int id, float time);
    }
}
