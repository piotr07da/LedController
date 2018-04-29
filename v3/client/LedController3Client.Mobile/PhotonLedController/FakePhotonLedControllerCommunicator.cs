
using System;

namespace LedController3Client.Mobile.PhotonLedController
{
    public class FakePhotonLedControllerCommunicator : IPhotonLedControllerCommunicator
    {
        public event EventHandler<EventArgs<ulong>> CycleTimeRead;
        public event EventHandler<EventArgs<float>> TimeProgressRead;
        public event EventHandler<EventArgs<ColorTimePoint[]>> ColorTimePointsRead;

        public void ReadCycleTime()
        {
            CycleTimeRead?.Invoke(this, new EventArgs<ulong>(60000));
        }

        public void ReadTimeProgress()
        {
            var now = DateTime.Now;
            var milisOfCurrentMinute = now.Second * 1000f + now.Millisecond;
            var progress = milisOfCurrentMinute / 60000f;
            TimeProgressRead?.Invoke(this, new EventArgs<float>(progress));
        }

        public void ReadColorTimePoints()
        {
            ColorTimePointsRead?.Invoke(this, new EventArgs<ColorTimePoint[]>(new[]
            {
                new ColorTimePoint(0, new Color(180, 0, 180), 0f, true),
                new ColorTimePoint(1, new Color(255, 0, 0), .15f, false),
                new ColorTimePoint(2, new Color(0, 255, 0), .50f, false),
                new ColorTimePoint(3, new Color(0, 0, 255), .85f, false),
                new ColorTimePoint(0, new Color(180, 0, 180), 1f, true),
            }));
        }
    }
}
