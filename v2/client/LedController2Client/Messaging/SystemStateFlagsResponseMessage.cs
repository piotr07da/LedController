
namespace LedController2Client
{
    public class SystemStateFlagsResponseMessage
    {
        /// <summary>
        /// Data
        /// </summary>
        public byte[] Data { get; set; }

        public bool IsSystemEnabled { get { return (Data[0] & 1) > 0; } }

        public bool IsSoundEnabled { get { return (Data[0] & 2) > 0; } }
    }
}
