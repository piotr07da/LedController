using SkiaSharp;
using System;

namespace LedController3Client.Mobile.ColorTimeLineDrawing
{
    public class ColorTimeLineSlider
    {
        public ColorTimeLineSlider(SKColor color, float time, float orbitRadius, float radius)
        {
            Color = color;
            Time = time;
            OrbitRadius = orbitRadius;
            Radius = radius;

            RecalculatePosition();
        }

        public SKColor Color { get; private set; }
        public float Time { get; private set; }
        public float OrbitRadius { get; private set; }
        public float Radius { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }


        public void UpdateColor(SKColor color)
        {
            Color = color;
        }

        public void UpdateTime(float time)
        {
            Time = time;
            RecalculatePosition();
        }

        public bool HitTest(float x, float y)
        {
            var xd = x - X;
            var yd = y - Y;
            return xd * xd + yd * yd < Radius * Radius * 1.5f; // 1.5 ratio is just for making collision circle bigger than real shape.
        }

        private void RecalculatePosition()
        {
            var angle = 2.0 * Math.PI * Time;
            X = OrbitRadius * (float)Math.Cos(angle);
            Y = OrbitRadius * (float)Math.Sin(angle);
        }
    }
}
