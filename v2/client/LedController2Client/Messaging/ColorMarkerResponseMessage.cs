
namespace LedController2Client
{
    public class ColorMarkerResponseMessage : ResponseMessage
    {
        public byte MarkerIndex { get; set; }
        public byte TimePoint { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
    }
}
