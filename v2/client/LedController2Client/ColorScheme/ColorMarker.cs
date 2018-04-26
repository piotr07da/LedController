using System;

namespace LedController2Client
{
    [Serializable]
    public class ColorMarker
    {
        public byte TimePoint { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
    }
}
