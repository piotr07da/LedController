using System;

namespace LedController2Client.SerialCommunication
{
    public class FakeTransiver : ITransiver
    {
        private bool _isOpen;

        #region ITransiver members

        public bool IsOpen
        {
            get { return _isOpen; }
        }

        public void Open()
        {
            _isOpen = true;
        }

        public bool TryOpen()
        {
            _isOpen = true;
            return true;
        }

        public void Close()
        {
            _isOpen = false;
        }

        public void Send(byte b)
        {
            throw new NotImplementedException();
        }

        public bool SyncSend(byte b, out byte bout)
        {
            throw new NotImplementedException();
        }

        public event Action<TransiverDataReceivedEventArgs> DataReceived;

        public event Action SyncSendFailed;

        public event Action Opened;

        public event Action Closed;

        #endregion



    }
}
