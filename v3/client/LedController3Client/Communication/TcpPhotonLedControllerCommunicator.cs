using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Tmds.MDns;

namespace LedController3Client.Communication
{
    public class TcpPhotonLedControllerCommunicator : IPhotonLedControllerCommunicator
    {
        private LedServiceHostProvider _ledServiceHostProvider;
        private TcpClient _tcpClient;

        public TcpPhotonLedControllerCommunicator()
        {
            _ledServiceHostProvider = new LedServiceHostProvider();
        }

        public event EventHandler<EventArgs<int>> CycleTimeRead;
        public event EventHandler<EventArgs<float>> TimeProgressRead;
        public event EventHandler<EventArgs<ColorTimePoint[]>> ColorTimePointsRead;

        public void ReadCycleTime()
        {
            CallOnNetworkStream(netStream =>
            {
                netStream.WriteByte(0x01);

                var data = new byte[4];
                var len = netStream.Read(data, 0, 4);
                var cycleTime = BitConverter.ToInt32(data, 0);
                CycleTimeRead?.Invoke(this, new EventArgs<int>(cycleTime));
            });
        }

        public void ReadTimeProgress()
        {
            CallOnNetworkStream(netStream =>
            {
                netStream.WriteByte(0x02);

                var data = new byte[1024];
                var len = netStream.Read(data, 0, 1024);
                var timeProgress = BitConverter.ToSingle(data, 0);
                TimeProgressRead?.Invoke(this, new EventArgs<float>(timeProgress));
            });
        }

        public void ReadColorTimePoints()
        {
            CallOnNetworkStream(netStream =>
            {
                netStream.WriteByte(0x03);

                var byteList = new List<byte>();
                var buffer = new byte[4];

                do
                {
                    netStream.Read(buffer, 0, 4);
                    byteList.AddRange(buffer);
                }
                while (netStream.DataAvailable);

                var data = byteList.ToArray();

                var pointCount = data[0];
                var offsetIx = 1;

                var points = new List<ColorTimePoint>();
                for (int i = 0; i < pointCount; ++i)
                {
                    var id = data[offsetIx];
                    var r = data[offsetIx + 1];
                    var g = data[offsetIx + 2];
                    var b = data[offsetIx + 3];
                    var t = BitConverter.ToSingle(data, offsetIx + 4);

                    var ctp = new ColorTimePoint(id, new ColorTimePointColor(r, g, b), t);
                    points.Add(ctp);

                    offsetIx += 8;
                }

                ColorTimePointsRead?.Invoke(this, new EventArgs<ColorTimePoint[]>(points.ToArray()));
            });
        }

        public void WriteCycleTime(int cycleTime)
        {
            CallOnNetworkStream(netStream =>
            {
                netStream.WriteByte(0x71);
                netStream.Write(BitConverter.GetBytes(cycleTime), 0, 4);
            });
        }

        public void WriteTimeProgress(float timeProgress)
        {
            CallOnNetworkStream(netStream =>
            {
                netStream.WriteByte(0x72);
                netStream.Write(BitConverter.GetBytes(timeProgress), 0, 4);
            });
        }

        public void WriteColorTimePointColor(byte id, ColorTimePointColor color)
        {
            CallOnNetworkStream(netStream =>
            {
                netStream.WriteByte(0x73);
                netStream.WriteByte(id);
                netStream.WriteByte(color.R);
                netStream.WriteByte(color.G);
                netStream.WriteByte(color.B);
            });
        }

        public void WriteColorTimePointTime(byte id, float time)
        {
            CallOnNetworkStream(netStream =>
            {
                netStream.WriteByte(0x73);
                netStream.WriteByte(id);
                netStream.Write(BitConverter.GetBytes(time), 0, 4);
            });
        }

        private void CallOnNetworkStream(Action<NetworkStream> action)
        {
            var ipAddress = _ledServiceHostProvider.HostIpAddress;
            var port = _ledServiceHostProvider.HostPort;

            if (_tcpClient == null || !_tcpClient.Connected)
            {
                _tcpClient = new TcpClient();
                _tcpClient.Connect(ipAddress, port);
            }

            var stream = _tcpClient.GetStream();
            action(stream);
        }
    }
}
