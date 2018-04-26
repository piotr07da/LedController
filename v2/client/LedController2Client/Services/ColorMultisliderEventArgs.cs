using System;

namespace LedController2Client.Services
{
    public class ColorMultisliderEventArgs : EventArgs
    {
        public byte MarkerIndex { get; set; }

        public byte MarkerTimePoint { get; set; }

        public ushort TimeProgress { get; set; }
    }
}
