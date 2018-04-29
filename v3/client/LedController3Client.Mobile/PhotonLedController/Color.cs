using System;
using System.Collections.Generic;
using System.Text;

namespace LedController3Client.Mobile.PhotonLedController
{
    public class Color
    {
        public Color(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
    }
}
