using System;

namespace LedController2Client
{
    [Serializable]
    public class ColorScheme
    {
        public virtual string Name { get; set; }

        public virtual ColorMarker[] Gradient { get; set; }

        public UInt16 TimeSpan { get; set; }
    }
}
