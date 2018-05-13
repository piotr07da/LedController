using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace LedController3Client.Communication
{
    public class TcpPhotonLedControllerCommunicator : IPhotonLedControllerCommunicator
    {
        private const double MinActionInterval = 50;

        private readonly LedServiceHostProvider _ledServiceHostProvider;
        private readonly ConcurrentQueue<Action<NetworkStream>> _actionsQueue;
        private readonly ConcurrentDictionary<string, double> _actionEnqueueTimestamps;

        private TcpClient _tcpClient;

        public TcpPhotonLedControllerCommunicator()
        {
            _ledServiceHostProvider = new LedServiceHostProvider();
            _actionsQueue = new ConcurrentQueue<Action<NetworkStream>>();
            _actionEnqueueTimestamps = new ConcurrentDictionary<string, double>();
        }

        public event EventHandler<EventArgs<int>> CycleTimeRead;
        public event EventHandler<EventArgs<float>> TimeProgressRead;
        public event EventHandler<EventArgs<ColorTimePoint[]>> ColorTimePointsRead;

        public void Start()
        {
            var t = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if (_actionsQueue.IsEmpty)
                        continue;

                    if (!_actionsQueue.TryDequeue(out Action<NetworkStream> action))
                        continue;

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
            }));
            t.Start();
        }

        public void ReadCycleTime()
        {
            EnqueueCallOnNetworkStream(nameof(ReadCycleTime), netStream =>
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
            EnqueueCallOnNetworkStream(nameof(ReadTimeProgress), netStream =>
            {
                netStream.WriteByte(0x02);

                var data = new byte[4];
                var len = netStream.Read(data, 0, 4);
                var timeProgress = BitConverter.ToSingle(data, 0);
                TimeProgressRead?.Invoke(this, new EventArgs<float>(timeProgress));
            });
        }

        public void ReadColorTimePoints()
        {
            EnqueueCallOnNetworkStream(nameof(ReadColorTimePoints), netStream =>
            {
                netStream.WriteByte(0x03);

                var byteList = new List<byte>();
                var buffer = new byte[1024];

                do
                {
                    var bytesRead = netStream.Read(buffer, 0, 4);
                    for (var i = 0; i < bytesRead; ++i)
                        byteList.Add(buffer[i]);
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
            EnqueueCallOnNetworkStream(nameof(WriteCycleTime), netStream =>
            {
                netStream.WriteByte(0x71);
                netStream.Write(BitConverter.GetBytes(cycleTime), 0, 4);
            });
        }

        public void WriteTimeProgress(float timeProgress)
        {
            EnqueueCallOnNetworkStream(nameof(WriteTimeProgress), netStream =>
            {
                netStream.WriteByte(0x72);
                netStream.Write(BitConverter.GetBytes(timeProgress), 0, 4);
            });
        }

        public void WriteColorTimePointColor(byte id, ColorTimePointColor color)
        {
            EnqueueCallOnNetworkStream(nameof(WriteColorTimePointColor), netStream =>
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
            EnqueueCallOnNetworkStream(nameof(WriteColorTimePointTime), netStream =>
            {
                netStream.WriteByte(0x73);
                netStream.WriteByte(id);
                netStream.Write(BitConverter.GetBytes(time), 0, 4);
            });
        }

        private void EnqueueCallOnNetworkStream(string actionName, Action<NetworkStream> action)
        {
            var cts = CurrentTimestamp();
            _actionEnqueueTimestamps.TryGetValue(actionName, out double lts);

            if (cts - lts > MinActionInterval)
            {
                _actionsQueue.Enqueue(action);
                _actionEnqueueTimestamps.AddOrUpdate(actionName, cts, (k, v) => cts);
            }
        }

        private double CurrentTimestamp()
        {
            return (DateTime.Now - DateTime.MinValue).TotalMilliseconds;
        }
    }
}
