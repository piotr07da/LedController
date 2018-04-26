using System.IO.Ports;

namespace LedController2Client.SerialCommunication
{
    public class SerialTransiverFactory : ITransiverFactory
    {
        #region Constants

        private const int __BYTE_BIT_COUNT = 8;

        #endregion

        #region Constructors

        public SerialTransiverFactory(string portName, int baudRate, Parity parity = Parity.None, int dataBits = __BYTE_BIT_COUNT, StopBits stopBits = StopBits.One)
        {
            _portName = portName;
            _baudRate = baudRate;
            _parity = parity;
            _dataBits = dataBits;
            _stopBits = stopBits;
        }

        #endregion

        #region Attributes

        private string _portName;
        private int _baudRate;
        private Parity _parity;
        private int _dataBits;
        private StopBits _stopBits;

        #endregion

        #region ITransiverFactory members

        public ITransiver CreateTransiver()
        {
            SerialTransiver transiver = new SerialTransiver();
            transiver.Init(_portName, _baudRate, _parity, _dataBits, _stopBits);
            return transiver;
        }

        #endregion
    }
}
