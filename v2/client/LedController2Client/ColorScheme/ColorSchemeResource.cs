using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace LedController2Client
{
    public class ColorSchemeResource : ColorSchemeResourceBase
    {
        #region Constants


        #endregion


        #region ColorSchemeResourceBase Members

        private ColorSchemeConfiguration _config;
        protected override ColorSchemeConfiguration Config
        {
            get { return _config; }
        }

        protected override void LoadConfig()
        {
            string appPath = Assembly.GetExecutingAssembly().Location;
            appPath = appPath.Substring(0, appPath.LastIndexOf('\\'));
            string filePath = appPath + "\\" + ConfigurationManager.AppSettings["ColorSchemeConfigurationFilePath"];

            if (File.Exists(filePath))
            {
                byte[] serializedConfiguration = File.ReadAllBytes(filePath);
                if (serializedConfiguration != null && serializedConfiguration.Any())
                {
                    _config = DeserializeObjectFromBinary<ColorSchemeConfiguration>(serializedConfiguration);
                }
            }
            else
            {
                _config = new ColorSchemeConfiguration() { ColorSchemeGroups = new List<ColorSchemeGroup>() };
                SaveConfig();
            }
        }

        protected override void SaveConfig()
        {
            string appPath = Assembly.GetExecutingAssembly().Location;
            appPath = appPath.Substring(0, appPath.LastIndexOf('\\'));
            string filePath = appPath + "\\" + ConfigurationManager.AppSettings["ColorSchemeConfigurationFilePath"];

            MemoryStream stream = new MemoryStream();
            byte[] serializedConfig = SerializeObjectToBinary(Config, stream);
            stream.Position = 0;

            FileStream fs = null;
            try
            {
                fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fs.Flush();
                foreach (byte b in serializedConfig)
                    fs.WriteByte(b);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        #endregion

        private static byte[] SerializeObjectToBinary(object objectToSerialize, MemoryStream destinationStream)
        {
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(destinationStream, objectToSerialize);
            return destinationStream.ToArray();
        }

        private static T DeserializeObjectFromBinary<T>(byte[] serializedObjectData)
        {
            T res = default(T);
            if (serializedObjectData != null)
            {
                IFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(serializedObjectData);
                ms.Position = 0;
                res = (T)formatter.Deserialize(ms);
            }
            return res;
        }
    }
}
