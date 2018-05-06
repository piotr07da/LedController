using System;

namespace LedController3Client.Communication
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
        void WriteColorTimePointColor(byte id, ColorTimePointColor color);
        void WriteColorTimePointTime(byte id, float time);
    }
}
