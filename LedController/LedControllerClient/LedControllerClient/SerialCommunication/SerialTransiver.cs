using System.IO.Ports;

namespace LedControllerClient.SerialCommunication
{
    /// <summary>
    /// Serial communication transmitter/receiver
    /// </summary>
    public class SerialTransiver
    {
        #region Constants

        private const int __BYTE_BIT_COUNT = 8;

        #endregion

        #region Ctors

        /// <summary>
        /// Creates new instance of <see cref="SerialTransiver"/>.
        /// </summary>
        public SerialTransiver()
        {
            //
        }

        #endregion

        #region Attributes

        private SerialPort _port;

        #endregion

        #region Events

        public event SerialDataReceivedEventHandler DataReceived;

        #endregion

        #region Event handling

        void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (DataReceived != null)
                DataReceived(sender, e);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Send data buffer.
        /// </summary>
        /// <param name="buffer"></param>
        public virtual void Send(byte[] buffer)
        {
            if (!_port.IsOpen)
                _port.Open();

            _port.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Inits serial port.
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        public virtual void Init(string portName, int baudRate, Parity parity = Parity.None, int dataBits = __BYTE_BIT_COUNT, StopBits stopBits = StopBits.One)
        {
            _port = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            _port.DataReceived += _port_DataReceived;
        }

        #endregion
    }
}
