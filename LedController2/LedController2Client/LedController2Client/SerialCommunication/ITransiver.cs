using System;

namespace LedController2Client.SerialCommunication
{
    public interface ITransiver
    {
        bool IsOpen { get; }

        void Open();

        bool TryOpen();

        void Close();

        void Send(byte b);

        bool SyncSend(byte b, out byte bout);

        event Action<TransiverDataReceivedEventArgs> DataReceived;

        event Action SyncSendFailed;

        event Action Opened;

        event Action Closed;
    }
}
