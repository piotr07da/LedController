using System;
using System.Collections.Generic;
using System.Text;

namespace LedController3Client.Mobile.PhotonLedController
{
    public class ColorTimePoint
    {
        public ColorTimePoint(int id, Color c, float t, bool welding)
        {
            Id = id;
            Color = c;
            Time = t;
            IsWeldingPoint = welding;
        }

        public int Id { get; set; }
        public Color Color { get; set; }
        public float Time { get; set; }
        public bool IsWeldingPoint { get; set; }
    }
}
