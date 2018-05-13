using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace LedController3Client.Communication
{
    // Bunch of code from previous version of cloud communication

    public class CloudCommunicator 
    {
        private const string DeviceId = "";
        private const string AccessToken = "";
        public const string GetVariableUrlPattern = "https://api.particle.io/v1/devices/{deviceId}/{variableName}/?access_token={token}";
        public const string GetFunctionUrlPattern = "https://api.particle.io/v1/devices/{deviceId}/{functionName}";

        public event EventHandler<EventArgs<int>> CycleTimeRead;
        public event EventHandler<EventArgs<float>> TimeProgressRead;
        public event EventHandler<EventArgs<ColorTimePoint[]>> ColorTimePointsRead;

        public void ReadCycleTime()
        {
            CallForFreshData();

            var rawVar = GetRawVariable("Cycle");
            var varVal = Int32.Parse(rawVar);
            CycleTimeRead?.Invoke(this, new EventArgs<int>(varVal));
        }

        public void ReadTimeProgress()
        {
            CallForFreshData();

            var rawVar = GetRawVariable("Progress");
            var varVal = Single.Parse(rawVar);
            TimeProgressRead?.Invoke(this, new EventArgs<float>(varVal));
        }

        public void ReadColorTimePoints()
        {
            CallForFreshData();

            var rawVar = GetRawVariable("Points");
            var bytes = StringToByteArray(rawVar);

            var ctps = new List<ColorTimePoint>();
            for (int pIx = 0; pIx < bytes.Length / 8; ++pIx)
            {
                var bIxOffset = pIx * 8;
                var id = bytes[bIxOffset];
                var color = new ColorTimePointColor(bytes[bIxOffset + 1], bytes[bIxOffset + 2], bytes[bIxOffset + 3]);
                var time = BitConverter.ToSingle(bytes, bIxOffset + 4);
                ctps.Add(new ColorTimePoint(id, color, time));
            }

            ColorTimePointsRead?.Invoke(this, new EventArgs<ColorTimePoint[]>(ctps.ToArray()));
        }

        public void WriteCycleTime(int cycleTime)
        {
            throw new NotImplementedException();
        }

        public void WriteTimeProgress(float timeProgress)
        {
            throw new NotImplementedException();
        }

        public void WriteColorTimePointColor(byte id, ColorTimePointColor color)
        {
            throw new NotImplementedException();
        }

        public void WriteColorTimePointTime(byte id, float time)
        {
            throw new NotImplementedException();
        }

        private void CallForFreshData()
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(
                GetFunctionUrlPattern
                .Replace("{deviceId}", DeviceId)
                .Replace("{functionName}", "FreshData"));
            request.Method = "POST";
            var parameters = $"access_token={AccessToken}";
            var parametersBytes = Encoding.UTF8.GetBytes(parameters);
            request.ContentLength = parameters.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            using (var dataStream = request.GetRequestStream())
            {
                dataStream.Write(parametersBytes, 0, parametersBytes.Length);
            }
            var response = request.GetResponse();
            response.Dispose();
        }

        private string GetRawVariable(string variableName)
        {
            string responseText;

            var request = (HttpWebRequest)HttpWebRequest.Create(
                GetVariableUrlPattern
                .Replace("{deviceId}", DeviceId)
                .Replace("{variableName}", variableName)
                .Replace("{token}", AccessToken));
            request.Method = "GET";
            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    var sr = new StreamReader(responseStream);
                    responseText = sr.ReadToEnd();
                }
            }

            var jsonObj = JsonConvert.DeserializeObject<JObject>(responseText);
            return jsonObj["result"].ToString();
        }

        private byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
