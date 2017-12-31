using LedController2Client.SerialCommunication;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Media;

namespace LedController2Client
{
    public class Messenger : IMessenger
    {
        #region Constants

        private const int __RECEIVE_FRAME_TIMEOUT = 2500;

        #endregion

        #region Ctors

        public Messenger(ITransiver transiver)
        {
            _transiver = transiver;
            _transiver.DataReceived += _transiver_DataReceived;



            _rs = new FrameReceiverState();
            _rs.Init();
            _rs.FrameReceived += _rs_FrameReceived;
            _rs.FrameError += _rs_FrameError;
        }

        #endregion

        #region Attributes

        private ITransiver _transiver;
        private int _echoBytesLeft;

        private MessageType _messageType;
        private byte _markerIndex;


        private FrameReceiverState _rs;
        private bool _pendingResponse;

        private ColorMarkerResponseMessage _colorMarkerResponseMessage;
        private ColorMarkerCountResponseMessage _colorMarkerCountResponseMessage;
        private TimeSpanResponseMessage _timeSpanResponseMessage;
        private TimeProgressResponseMessage _timeProgressResponseMessage;
        private SystemStateFlagsResponseMessage _systemStateFlagsResponseMessage;

        #endregion

        #region Properties

        public MessageType MessageType
        {
            get { return _messageType; }
        }

        #endregion

        #region Event handling

        void _transiver_DataReceived(TransiverDataReceivedEventArgs ea)
        {
            byte b = ea.Byte;
            ReadByte(b);

            if (ByteReceived != null)
                ByteReceived(this, b);
        }

        void _rs_FrameReceived(FrameReceiverState obj)
        {
            switch (_messageType)
            {
                case MessageType.GetMarker:
                    if (ReadMarkerCompleted != null)
                    {
                        _colorMarkerResponseMessage = new ColorMarkerResponseMessage();
                        _colorMarkerResponseMessage.TimePoint = obj.PayloadBuff[0];
                        _colorMarkerResponseMessage.R = obj.PayloadBuff[1];
                        _colorMarkerResponseMessage.G = obj.PayloadBuff[2];
                        _colorMarkerResponseMessage.B = obj.PayloadBuff[3];
                        _colorMarkerResponseMessage.MarkerIndex = _markerIndex;
                        ReadMarkerCompleted(this, _colorMarkerResponseMessage);
                    }
                    break;

                case MessageType.GetMarkerCount:
                    if (ReadMarkerCountCompleted != null)
                    {
                        _colorMarkerCountResponseMessage = new ColorMarkerCountResponseMessage() { MarkerCount = obj.PayloadBuff[0] };
                        ReadMarkerCountCompleted(this, _colorMarkerCountResponseMessage);
                    }
                    break;

                case MessageType.GetTimeSpan:
                    if (ReadTimeSpanCompleted != null)
                    {
                        _timeSpanResponseMessage = new TimeSpanResponseMessage();
                        _timeSpanResponseMessage.TimeSpan = obj.PayloadBuff[0];
                        _timeSpanResponseMessage.TimeSpan |= (UInt16)((UInt16)obj.PayloadBuff[1] << 8);
                        ReadTimeSpanCompleted(this, _timeSpanResponseMessage);
                    }
                    break;

                case MessageType.GetTimeProgress:
                    if (ReadTimeProgressCompleted != null)
                    {
                        _timeProgressResponseMessage = new TimeProgressResponseMessage();
                        _timeProgressResponseMessage.TimeProgress = obj.PayloadBuff[0];
                        _timeProgressResponseMessage.TimeProgress |= (UInt16)((UInt16)obj.PayloadBuff[1] << 8);
                        ReadTimeProgressCompleted(this, _timeProgressResponseMessage);
                    }
                    break;

                case MessageType.GetSystemStateFlags:
                    if (ReadSystemStateFlagsCompleted != null)
                    {
                        _systemStateFlagsResponseMessage = new SystemStateFlagsResponseMessage();
                        _systemStateFlagsResponseMessage.Data = obj.PayloadBuff;
                        ReadSystemStateFlagsCompleted(this, _systemStateFlagsResponseMessage);
                    }
                    break;
            }

            _messageType = MessageType.Undefined;
            _pendingResponse = false;
        }

        void _rs_FrameError(FrameReceiverState obj)
        {
            _messageType = MessageType.Undefined;
            _pendingResponse = false;
        }

        #endregion

        #region Methods

        protected virtual void InitMessageContext(MessageType messageType)
        {
            _messageType = messageType;

            if (MessageTypeChanged != null)
                MessageTypeChanged(this, _messageType);
        }

        protected virtual bool SyncSendData(byte[] data, bool wrapIntoFrame = true)
        {
            if (wrapIntoFrame)
            {
                data = PrependSyncword(data);
                data = AppendCRC4(data);
            }

            _echoBytesLeft = data.Length;

            for (int bIx = 0; bIx < data.Length; ++bIx)
            {
                byte bout;
                if (!_transiver.SyncSend(data[bIx], out bout))
                {
                    _messageType = MessageType.Undefined;
                    _pendingResponse = false;

                    return false;
                }
            }

            return true;
        }

        private byte[] PrependSyncword(byte[] requestFrame)
        {
            byte[] frame = new byte[requestFrame.Length + 3];
            frame[0] = 0xAA;
            frame[1] = 0xAA;
            frame[2] = 0xAB;
            for (int bIx = 0; bIx < requestFrame.Length; ++bIx)
                frame[3 + bIx] = requestFrame[bIx];
            return frame;
        }

        private byte[] AppendCRC4(byte[] requestFrame)
        {
            byte[] frame = new byte[requestFrame.Length + 1];
            Array.Copy(requestFrame, frame, requestFrame.Length);

            // TODO
            frame[frame.Length - 1] = 5;

            return frame;
        }

        // READING RESPONSE BYTES

        protected virtual void ReadByte(byte @byte)
        {
            if (_echoBytesLeft > 0)
                --_echoBytesLeft;
            else
                _rs.ReceiveByte(@byte);
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
        public event Action<IMessenger, MessageType> MessageTypeChanged;
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
            byte[] requestFrame = new byte[] { (byte)MessageType.AddMarker, 1, 0 };
            InitMessageContext(MessageType.AddMarker);

            _pendingResponse = true;
            SyncSendData(requestFrame);
            _pendingResponse = false;

            RaiseOneWayRequestCompletedEvent(AddMarkerCompleted);
        }

        public void RemMarker()
        {
            byte[] requestFrame = new byte[] { (byte)MessageType.RemMarker, 1, 0 };
            InitMessageContext(MessageType.RemMarker);

            _pendingResponse = true;
            SyncSendData(requestFrame);
            _pendingResponse = false;

            RaiseOneWayRequestCompletedEvent(RemMarkerCompleted);
        }

        public virtual void SetMarker(byte markerIndex, byte timePoint, Color color)
        {
            byte[] requestFrame = new byte[] { (byte)MessageType.SetMarker, 5, markerIndex, timePoint, color.R, color.G, color.B };
            InitMessageContext(MessageType.SetMarker);

            _pendingResponse = true;
            SyncSendData(requestFrame);
            _pendingResponse = false;

            RaiseOneWayRequestCompletedEvent(SetMarkerCompleted);
        }

        public void SetTimeSpan(ushort timeSpan)
        {
            byte l = (byte)(timeSpan & 255);
            byte h = (byte)(timeSpan >> 8);
            byte[] requestFrame = new byte[] { (byte)MessageType.SetTimeSpan, 2, l, h };
            InitMessageContext(MessageType.SetTimeSpan);

            _pendingResponse = true;
            SyncSendData(requestFrame);
            _pendingResponse = false;

            RaiseOneWayRequestCompletedEvent(SetTimeSpanCompleted);
        }

        public void SetTimeProgress(ushort timeProgress)
        {
            byte l = (byte)(timeProgress & 255);
            byte h = (byte)(timeProgress >> 8);
            byte[] requestFrame = new byte[] { (byte)MessageType.SetTimeProgress, 2, l, h };
            InitMessageContext(MessageType.SetTimeProgress);

            _pendingResponse = true;
            SyncSendData(requestFrame);
            _pendingResponse = false;

            RaiseOneWayRequestCompletedEvent(SetTimeProgressCompleted);
        }

        public virtual void LoadMarker(byte markerIndex)
        {
            _markerIndex = markerIndex;
            byte[] requestFrame = new byte[] { (byte)MessageType.GetMarker, 1, markerIndex };
            InitMessageContext(MessageType.GetMarker);

            _pendingResponse = true;
            int responseOverflow = 0;
            SyncSendData(requestFrame);
            while (_pendingResponse && responseOverflow++ < __RECEIVE_FRAME_TIMEOUT) { Thread.Sleep(1); }
            _pendingResponse = false;
        }

        public virtual void LoadMarkerCount()
        {
            byte[] requestFrame = new byte[] { (byte)MessageType.GetMarkerCount, 1, 0 };
            InitMessageContext(MessageType.GetMarkerCount);

            _pendingResponse = true;
            int responseOverflow = 0;
            SyncSendData(requestFrame);
            while (_pendingResponse && responseOverflow++ < __RECEIVE_FRAME_TIMEOUT) { Thread.Sleep(1); }
            _pendingResponse = false;
        }

        //public virtual async Task<ColorMarkerCountResponseMessage> LoadMarkerCount()
        //{
        //    return await new Task<ColorMarkerCountResponseMessage>(() =>
        //    {
        //        byte[] requestFrame = new byte[] { (byte)MessageType.GetMarkerCount, 1, 0 };
        //        InitMessageContext(MessageType.GetMarkerCount);

        //        _pendingResponse = true;
        //        int responseOverflow = 0;
        //        SyncSendData(requestFrame);
        //        while (_pendingResponse && responseOverflow++ < __RECEIVE_FRAME_TIMEOUT) { Thread.Sleep(1); }
        //        _pendingResponse = false;

        //        return new ColorMarkerCountResponseMessage();
        //    });
        //}

        public virtual void LoadTimeSpan()
        {
            byte[] requestFrame = new byte[] { (byte)MessageType.GetTimeSpan, 1, 0 };
            InitMessageContext(MessageType.GetTimeSpan);

            _pendingResponse = true;
            int responseOverflow = 0;
            SyncSendData(requestFrame);
            while (_pendingResponse && responseOverflow++ < __RECEIVE_FRAME_TIMEOUT) { Thread.Sleep(1); }
            _pendingResponse = false;
        }

        public virtual void LoadTimeProgress()
        {
            byte[] requestFrame = new byte[] { (byte)MessageType.GetTimeProgress, 1, 0 };
            InitMessageContext(MessageType.GetTimeProgress);

            _pendingResponse = true;
            int responseOverflow = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            SyncSendData(requestFrame);
            sw.Stop();
            long ms = sw.ElapsedMilliseconds;
            while (_pendingResponse && responseOverflow++ < __RECEIVE_FRAME_TIMEOUT) { Thread.Sleep(1); }
            _pendingResponse = false;
        }

        public virtual void TurnOn()
        {
            byte[] requestFrame = new byte[] { (byte)MessageType.TurnOn, 1, 0 };
            InitMessageContext(MessageType.TurnOn);

            _pendingResponse = true;
            SyncSendData(requestFrame);
            _pendingResponse = false;

            RaiseOneWayRequestCompletedEvent(TurnOnCompleted);
        }

        public virtual void TurnOff()
        {
            byte[] requestFrame = new byte[] { (byte)MessageType.TurnOff, 1, 0 };
            InitMessageContext(MessageType.TurnOff);

            _pendingResponse = true;
            SyncSendData(requestFrame);
            _pendingResponse = false;

            RaiseOneWayRequestCompletedEvent(TurnOffCompleted);
        }

        public virtual void SoundOn()
        {
            byte[] requestFrame = new byte[] { (byte)MessageType.SoundOn, 1, 0 };
            InitMessageContext(MessageType.SoundOn);

            _pendingResponse = true;
            SyncSendData(requestFrame);
            _pendingResponse = false;

            RaiseOneWayRequestCompletedEvent(SoundOnCompleted);
        }

        public virtual void SoundOff()
        {
            byte[] requestFrame = new byte[] { (byte)MessageType.SoundOff, 1, 0 };
            InitMessageContext(MessageType.SoundOff);

            _pendingResponse = true;
            SyncSendData(requestFrame);
            _pendingResponse = false;

            RaiseOneWayRequestCompletedEvent(SoundOffCompleted);
        }

        public virtual void LoadSystemStateFlags()
        {
            byte[] requestFrame = new byte[] { (byte)MessageType.GetSystemStateFlags, 1, 0 };
            InitMessageContext(MessageType.GetSystemStateFlags);

            _pendingResponse = true;
            int responseOverflow = 0;
            SyncSendData(requestFrame);
            while (_pendingResponse && responseOverflow++ < __RECEIVE_FRAME_TIMEOUT) { Thread.Sleep(1); }
            _pendingResponse = false;
        }

        #endregion

        #endregion
    }
}
