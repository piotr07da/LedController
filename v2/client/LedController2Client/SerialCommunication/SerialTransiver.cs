using System;
using System.IO.Ports;
using System.Threading;

namespace LedController2Client.SerialCommunication
{
    /// <summary>
    /// Serial communication transmitter/receiver
    /// </summary>
    public class SerialTransiver : ITransiver
    {
        #region Constants

        public const int __RECEIVE_BYTE_TIMEOUT = 8;

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

        #region Event handling

        void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port = (sender as SerialPort);

            bool firstByteStored = false;
            while (port.BytesToRead > 0)
            {
                byte b = (byte)port.ReadByte();

                if (!firstByteStored)
                {
                    _response = b;
                    firstByteStored = true;
                }

                if (DataReceived != null)
                    DataReceived(new TransiverDataReceivedEventArgs() { Byte = b });
            }

            _pendingResponse = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inits serial port.
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        public virtual void Init(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            _port = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            _port.DataReceived += _port_DataReceived;
        }

        protected virtual void RaiseOpened()
        {
            Action handler = Opened;
            if (handler != null)
                handler();
        }

        protected virtual void RaiseClosed()
        {
            Action handler = Closed;
            if (handler != null)
                handler();
        }

        #endregion

        #region ITransiver members

        public virtual bool IsOpen
        {
            get { return _port.IsOpen; }
        }

        public virtual void Open()
        {
            if (!_port.IsOpen)
            {
                _port.Open();
                RaiseOpened();
            }
        }

        public virtual bool TryOpen()
        {
            try
            {
                if (!_port.IsOpen)
                {
                    _port.Open();
                    RaiseOpened();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            return true;
        }

        public virtual void Close()
        {
            if (_port.IsOpen)
            {
                _port.Close();
                RaiseClosed();
            }
        }

        /// <summary>
        /// Send data buffer.
        /// </summary>
        /// <param name="buffer"></param>
        public virtual void Send(byte b)
        {
            try
            {
                _port.Write(new byte[] { b }, 0, 1);
            }
            catch (Exception ex)
            {
                if (_port.IsOpen)
                    Close();
                else
                    RaiseClosed();
                Console.WriteLine(ex.ToString());
            }
        }

        private bool _pendingResponse;
        private int _pendingResponseCounter;
        private byte _response;
        public virtual bool SyncSend(byte b, out byte bout)
        {
            _pendingResponse = true;
            _pendingResponseCounter = 0;
            Send(b);

            while (_pendingResponse && _pendingResponseCounter < __RECEIVE_BYTE_TIMEOUT)
            {
                Thread.Sleep(1);
                ++_pendingResponseCounter;
            }

            Thread.Sleep(1);

            if (_pendingResponseCounter == __RECEIVE_BYTE_TIMEOUT)
            {
                bout = 0;

                if (SyncSendFailed != null)
                    SyncSendFailed();

                return false;
            }
            else
            {
                bout = _response;
                return true;
            }
        }

        public event Action<TransiverDataReceivedEventArgs> DataReceived;

        public event Action SyncSendFailed;

        public event Action Opened;

        public event Action Closed;

        #endregion



    }
}
