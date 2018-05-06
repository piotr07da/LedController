using System;
using System.Collections.Generic;
using System.Linq;

namespace LedController3Client.Communication
{
    public class FakePhotonLedControllerCommunicator : IPhotonLedControllerCommunicator
    {
        private DateTime _t0 = DateTime.Now;
        private DateTime _t1 = DateTime.Now;
        private int _cycleTime = 60000;
        private int _currentTime;
        private List<ColorTimePoint> _points = new List<ColorTimePoint>()
        {
            new ColorTimePoint(1, new ColorTimePointColor(255, 0, 0), .10f),
            new ColorTimePoint(2, new ColorTimePointColor(0, 255, 0), .43f),
            new ColorTimePoint(3, new ColorTimePointColor(0, 0, 255), .76f),
        };

        public event EventHandler<EventArgs<int>> CycleTimeRead;
        public event EventHandler<EventArgs<float>> TimeProgressRead;
        public event EventHandler<EventArgs<ColorTimePoint[]>> ColorTimePointsRead;

        public void ReadCycleTime()
        {
            CycleTimeRead?.Invoke(this, new EventArgs<int>(_cycleTime));
        }

        public void ReadTimeProgress()
        {
            _t0 = _t1;
            _t1 = DateTime.Now;
            var dt = _t1 - _t0;
            var dtms = (int)dt.TotalMilliseconds;
            _currentTime += dtms;
            int diff = _currentTime - _cycleTime;
            if (diff >= 0)
            {
                _currentTime = diff;
            }
            TimeProgressRead?.Invoke(this, new EventArgs<float>(_currentTime / (float)_cycleTime));
        }

        public void ReadColorTimePoints()
        {
            ColorTimePointsRead?.Invoke(this, new EventArgs<ColorTimePoint[]>(_points.ToArray()));
        }

        public void WriteCycleTime(int cycleTime)
        {
            _currentTime = _currentTime * cycleTime / _cycleTime;
            _cycleTime = cycleTime;
        }

        public void WriteTimeProgress(float timeProgress)
        {
            _currentTime = (int)(_cycleTime * timeProgress);
        }

        public void WriteColorTimePointColor(byte id, ColorTimePointColor color)
        {
            var updPoint = _points.Find(p => p.Id == id);
            updPoint.Color = color;
        }

        public void WriteColorTimePointTime(byte id, float time)
        {
            var updPoint = _points.Find(p => p.Id == id);
            updPoint.Time = time;
            _points = _points.OrderBy(p => p.Time).ToList();
        }
    }
}
