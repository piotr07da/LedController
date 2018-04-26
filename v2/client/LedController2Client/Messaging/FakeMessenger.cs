using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media;

namespace LedController2Client
{
    public class FakeMessenger : IMessenger
    {
        #region Constants

        public const ushort ctl_TIME_SPAN_SCALE = 5;

        #endregion

        #region Ctors

        public FakeMessenger()
        {
            _timerThread = new Thread(TimerThreadStart);
            _timerThread.Start();
            _randomizer = new Random();
            _markerColors = new List<Color>() { Colors.Red, Color.FromRgb(0, 255, 0), Colors.Blue, Colors.Red };
            TimeSpan = 100;
        }

        #endregion

        #region Attributes

        private Thread _timerThread;
        private ushort _timerCounter;
        private ushort _timerCounterDelta;
        private readonly Random _randomizer;
        private List<Color> _markerColors;
        private ushort _timeSpan;

        #endregion

        #region Properties

        private ushort TimeSpan
        {
            get { return _timeSpan; }
            set
            {
                _timeSpan = value;
                _timerCounterDelta = (ushort)((UInt16.MaxValue * ctl_TIME_SPAN_SCALE) / (double)(_timeSpan * 1000.0));
            }
        }

        #endregion

        #region Methods

        private void TimerThreadStart()
        {
            while (true)
            {
                Thread.Sleep(1);
                if (_timerCounter < UInt16.MaxValue - _timerCounterDelta)
                    _timerCounter += _timerCounterDelta;
                else
                    _timerCounter = 0;
            }
        }

        protected virtual void RaiseOneWayRequestCompletedEvent(Action<IMessenger> eventHandler)
        {
            if (eventHandler != null)
                eventHandler(this);
        }

        #endregion

        #region IMessenger Members

        #region IMessenger Events

        public event Action<IMessenger, byte> ByteReceived;
        public event Action<IMessenger> AddMarkerCompleted;
        public event Action<IMessenger> RemMarkerCompleted;
        public event Action<IMessenger> SetMarkerCompleted;
        public event Action<IMessenger> SetTimeSpanCompleted;
        public event Action<IMessenger> SetTimeProgressCompleted;
        public event Action<IMessenger, ColorMarkerResponseMessage> ReadMarkerCompleted;
        public event Action<IMessenger, ColorMarkerCountResponseMessage> ReadMarkerCountCompleted;
        public event Action<IMessenger, TimeSpanResponseMessage> ReadTimeSpanCompleted;
        public event Action<IMessenger, TimeProgressResponseMessage> ReadTimeProgressCompleted;
        public event Action<IMessenger> TurnOnCompleted;
        public event Action<IMessenger> TurnOffCompleted;
        public event Action<IMessenger> SoundOnCompleted;
        public event Action<IMessenger> SoundOffCompleted;
        public event Action<IMessenger, SystemStateFlagsResponseMessage> ReadSystemStateFlagsCompleted;

        #endregion

        #region IMessenger Methods

        public void AddMarker()
        {
            _markerColors.Insert(_markerColors.Count - 1, Color.FromRgb((byte)_randomizer.Next(256), (byte)_randomizer.Next(256), (byte)_randomizer.Next(256)));
            RaiseOneWayRequestCompletedEvent(AddMarkerCompleted);
        }

        public void RemMarker()
        {
            int cnt = _markerColors.Count;
            if (cnt > 4)
                _markerColors.RemoveAt(cnt - 2);
            RaiseOneWayRequestCompletedEvent(RemMarkerCompleted);
        }

        public void SetMarker(byte markerIndex, byte timePoint, Color color)
        {
            int indexOfLastColor = _markerColors.Count - 1;

            _markerColors[markerIndex] = color;
            if (markerIndex == 0)
                _markerColors[indexOfLastColor] = color;
            else if (markerIndex == indexOfLastColor)
                _markerColors[0] = color;

            RaiseOneWayRequestCompletedEvent(SetMarkerCompleted);
        }

        public void SetTimeSpan(ushort timeSpan)
        {
            TimeSpan = timeSpan;

            RaiseOneWayRequestCompletedEvent(SetTimeSpanCompleted);
        }

        public void SetTimeProgress(ushort timeProgress)
        {
            _timerCounter = timeProgress;

            RaiseOneWayRequestCompletedEvent(SetTimeProgressCompleted);
        }

        public void LoadMarker(byte markerIndex)
        {
            if (ReadMarkerCompleted != null)
            {
                ColorMarkerResponseMessage respMsg = new ColorMarkerResponseMessage() { MarkerIndex = markerIndex, TimePoint = (byte)(255 * markerIndex / (_markerColors.Count - 1)) };
                respMsg.R = _markerColors[markerIndex].R;
                respMsg.G = _markerColors[markerIndex].G;
                respMsg.B = _markerColors[markerIndex].B;
                ReadMarkerCompleted(this, respMsg);
            }
        }

        public void LoadMarkerCount()
        {
            if (ReadMarkerCountCompleted != null)
                ReadMarkerCountCompleted(this, new ColorMarkerCountResponseMessage() { MarkerCount = (byte)_markerColors.Count });
        }

        public void LoadTimeSpan()
        {
            if (ReadTimeSpanCompleted != null)
                ReadTimeSpanCompleted(this, new TimeSpanResponseMessage() { TimeSpan = _timeSpan });
        }

        public void LoadTimeProgress()
        {
            if (ReadTimeProgressCompleted != null)
            {
                ReadTimeProgressCompleted(this, new TimeProgressResponseMessage() { TimeProgress = _timerCounter });
            }
        }

        public virtual void TurnOn()
        {

        }

        public virtual void TurnOff()
        {

        }

        public virtual void SoundOn()
        {

        }

        public virtual void SoundOff()
        {

        }

        public virtual void LoadSystemStateFlags()
        {

        }

        #endregion

        #endregion
    }
}
