using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media;

namespace LedController2Client
{
    public class MessengerWorker : IMessengerWorker
    {
        #region Ctors

        public MessengerWorker(IMessenger messenger)
        {
            _messenger = messenger;
            _messenger.ByteReceived += _messenger_ByteReceived;
            _messenger.AddMarkerCompleted += _messenger_AddMarkerCompleted;
            _messenger.RemMarkerCompleted += _messenger_RemMarkerCompleted;
            _messenger.SetMarkerCompleted += _messenger_SetMarkerCompleted;
            _messenger.SetTimeSpanCompleted += _messenger_SetTimeSpanCompleted;
            _messenger.SetTimeProgressCompleted += _messenger_SetTimeProgressCompleted;
            _messenger.ReadMarkerCompleted += _messenger_ReadMarkerCompleted;
            _messenger.ReadMarkerCountCompleted += _messenger_ReadMarkerCountCompleted;
            _messenger.ReadTimeSpanCompleted += _messageReceiver_ReadTimeSpanCompleted;
            _messenger.ReadTimeProgressCompleted += _messageReceiver_ReadTimeProgressCompleted;
            _messenger.TurnOnCompleted += _messenger_TurnOnCompleted;
            _messenger.SoundOffCompleted += _messenger_SoundOffCompleted;
            _messenger.SoundOnCompleted += _messenger_SoundOnCompleted;
            _messenger.TurnOffCompleted += _messenger_TurnOffCompleted;
            _messenger.ReadSystemStateFlagsCompleted += _messenger_ReadSystemStateFlagsCompleted;

            _lockforMessangerTasks = new object();
            _messengerTasks = new List<Action>();
        }

        #endregion

        #region Attributes

        private IMessenger _messenger;
        private Thread _workerThread;
        private bool _workerThreadStopRequested;

        #endregion

        #region Properties

        private object _lockforMessangerTasks;
        private List<Action> _messengerTasks;

        #endregion

        #region Event handling

        void _messenger_ByteReceived(IMessenger sender, byte obj)
        {
            RaiseEventAction(ByteReceived, obj);
        }

        void _messenger_AddMarkerCompleted(IMessenger obj)
        {
            RaiseEventAction(AddMarkerCompleted);
        }
        void _messenger_RemMarkerCompleted(IMessenger obj)
        {
            RaiseEventAction(RemMarkerCompleted);
        }
        void _messenger_SetMarkerCompleted(IMessenger obj)
        {
            RaiseEventAction(SetMarkerCompleted);
        }
        void _messenger_SetTimeSpanCompleted(IMessenger obj)
        {
            RaiseEventAction(SetTimeSpanCompleted);
        }
        void _messenger_SetTimeProgressCompleted(IMessenger obj)
        {
            RaiseEventAction(SetTimeProgressCompleted);
        }

        void _messenger_ReadMarkerCompleted(IMessenger sender, ColorMarkerResponseMessage obj)
        {
            RaiseEventAction(ReadMarkerCompleted, obj);
        }

        void _messenger_ReadMarkerCountCompleted(IMessenger sender, ColorMarkerCountResponseMessage obj)
        {
            RaiseEventAction(ReadMarkerCountCompleted, obj);
        }

        void _messageReceiver_ReadTimeSpanCompleted(IMessenger sender, TimeSpanResponseMessage obj)
        {
            RaiseEventAction(ReadTimeSpanCompleted, obj);
        }

        void _messageReceiver_ReadTimeProgressCompleted(IMessenger sender, TimeProgressResponseMessage obj)
        {
            RaiseEventAction(ReadTimeProgressCompleted, obj);
        }

        void _messenger_TurnOnCompleted(IMessenger obj)
        {
            RaiseEventAction(TurnOnCompleted);
        }

        void _messenger_TurnOffCompleted(IMessenger obj)
        {
            RaiseEventAction(TurnOffCompleted);
        }

        void _messenger_SoundOnCompleted(IMessenger obj)
        {
            RaiseEventAction(SoundOnCompleted);
        }

        void _messenger_SoundOffCompleted(IMessenger obj)
        {
            RaiseEventAction(SoundOffCompleted);
        }

        void _messenger_ReadSystemStateFlagsCompleted(IMessenger arg1, SystemStateFlagsResponseMessage obj)
        {
            RaiseEventAction(ReadSystemStateFlagsCompleted, obj);
        }

        #endregion

        #region Methods

        protected virtual void WorkerThreadStart()
        {
            while (!_workerThreadStopRequested)
            {
                Thread.Sleep(1);

                Action task = null;

                OperateOnLockedMessengerTasks(tasks =>
                {
                    if (tasks.Count > 0)
                    {
                        task = tasks[0];
                        tasks.RemoveAt(0);
                    }
                });

                if (task != null)
                    task();
            }
        }

        protected virtual void RaiseEventAction(Action<IMessenger> eventHandler)
        {
            if (eventHandler != null)
                eventHandler(this);
        }

        protected virtual void RaiseEventAction<TEventArgs>(Action<IMessenger, TEventArgs> eventHandler, TEventArgs eventArgs)
        {
            if (eventHandler != null)
                eventHandler(this, eventArgs);
        }

        private void OperateOnLockedMessengerTasks(Action<List<Action>> action)
        {
            lock (_lockforMessangerTasks)
            {
                action(_messengerTasks);
            }
        }

        private void CheckWorkerThreadStatus()
        {
            if (_workerThread == null || !_workerThread.IsAlive)
                throw new InvalidOperationException("Worker thread is not alive.");
        }

        #endregion

        #region IMessengerWorker Members

        public virtual bool IsRunning
        {
            get { return _workerThread.IsAlive; }
        }

        public virtual int TaskCount
        {
            get
            {
                int count = 0;
                OperateOnLockedMessengerTasks(tasks =>
                {
                    count = tasks.Count;
                });
                return count;
            }
        }

        public virtual void Start()
        {
            _workerThreadStopRequested = false;
            _workerThread = new Thread(new ThreadStart(WorkerThreadStart));
            _messengerTasks.Clear();
            _workerThread.Start();
        }

        public virtual void Stop()
        {
            _workerThreadStopRequested = true;
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

        public virtual void AddMarker()
        {
            CheckWorkerThreadStatus();
            OperateOnLockedMessengerTasks(tasks =>
            {
                tasks.Add(() => { _messenger.AddMarker(); });
            });
        }

        public virtual void RemMarker()
        {
            CheckWorkerThreadStatus();
            OperateOnLockedMessengerTasks(tasks =>
            {
                tasks.Add(() => { _messenger.RemMarker(); });
            });
        }

        public virtual void SetMarker(byte markerIndex, byte timePoint, Color color)
        {
            CheckWorkerThreadStatus();
            OperateOnLockedMessengerTasks(tasks =>
            {
                tasks.Add(() => { _messenger.SetMarker(markerIndex, timePoint, color); });
            });
        }

        public virtual void SetTimeSpan(UInt16 timeSpan)
        {
            CheckWorkerThreadStatus();
            OperateOnLockedMessengerTasks(tasks =>
            {
                tasks.Add(() => { _messenger.SetTimeSpan(timeSpan); });
            });
        }

        public virtual void SetTimeProgress(UInt16 timeProgress)
        {
            CheckWorkerThreadStatus();
            OperateOnLockedMessengerTasks(tasks =>
            {
                tasks.Add(() => { _messenger.SetTimeProgress(timeProgress); });
            });
        }

        public virtual void LoadMarker(byte markerIndex)
        {
            CheckWorkerThreadStatus();
            OperateOnLockedMessengerTasks(tasks =>
            {
                tasks.Add(() => { _messenger.LoadMarker(markerIndex); });
            });
        }

        public virtual void LoadMarkerCount()
        {
            CheckWorkerThreadStatus();
            OperateOnLockedMessengerTasks(tasks =>
            {
                tasks.Add(() => { _messenger.LoadMarkerCount(); });
            });
        }

        public virtual void LoadTimeSpan()
        {
            CheckWorkerThreadStatus();
            OperateOnLockedMessengerTasks(tasks =>
            {
                tasks.Add(() => { _messenger.LoadTimeSpan(); });
            });
        }

        public virtual void LoadTimeProgress()
        {
            CheckWorkerThreadStatus();
            OperateOnLockedMessengerTasks(tasks =>
            {
                tasks.Add(() => { _messenger.LoadTimeProgress(); });
            });
        }

        public virtual void TurnOn()
        {
            CheckWorkerThreadStatus();
            OperateOnLockedMessengerTasks(tasks =>
            {
                tasks.Add(() => { _messenger.TurnOn(); });
            });
        }

        public virtual void TurnOff()
        {
            CheckWorkerThreadStatus();
            OperateOnLockedMessengerTasks(tasks =>
            {
                tasks.Add(() => { _messenger.TurnOff(); });
            });
        }

        public virtual void SoundOn()
        {
            CheckWorkerThreadStatus();
            OperateOnLockedMessengerTasks(tasks =>
            {
                tasks.Add(() => { _messenger.SoundOn(); });
            });
        }

        public virtual void SoundOff()
        {
            CheckWorkerThreadStatus();
            OperateOnLockedMessengerTasks(tasks =>
            {
                tasks.Add(() => { _messenger.SoundOff(); });
            });
        }

        public virtual void LoadSystemStateFlags()
        {
            CheckWorkerThreadStatus();
            OperateOnLockedMessengerTasks(tasks =>
            {
                tasks.Add(() => { _messenger.LoadSystemStateFlags(); });
            });
        }

        #endregion

        #endregion
    }
}
