using System;

namespace LedController2Client.SerialCommunication
{
    public class TransiverDataReceivedEventArgs : EventArgs, ITransiverDataReceivedEventArgs
    {
        #region ITransiverDataReceivedEventArgs Members

        public byte Byte { get; set; }

        #endregion
    }
}
