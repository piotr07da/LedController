using SkiaSharp;
using System;

namespace LedController3Client.Ui
{
    public class HsvRgbConverter
    {
        public void Hsv2Rgb(float h, float s, float v, out SKColor rgb)
        {
            var r = 0f;
            var g = 0f;
            var b = 0f;

            var i = (float)Math.Floor(h * 6);
            var f = h * 6 - i;
            var p = v * (1 - s);
            var q = v * (1 - f * s);
            var t = v * (1 - (1 - f) * s);

            switch (i % 6)
            {
                case 0: r = v; g = t; b = p; break;
                case 1: r = q; g = v; b = p; break;
                case 2: r = p; g = v; b = t; break;
                case 3: r = p; g = q; b = v; break;
                case 4: r = t; g = p; b = v; break;
                case 5: r = v; g = p; b = q; break;
            }

            r *= 255;
            g *= 255;
            b *= 255;

            rgb = new SKColor((byte)r, (byte)g, (byte)b);
        }

        public void Rgb2Hsv(SKColor rgb, out float h, out float s, out float v)
        {
            const float epsilon = .000001f;

            var r = (float)rgb.Red;
            var g = (float)rgb.Green;
            var b = (float)rgb.Blue;

            r /= 255;
            g /= 255;
            b /= 255;

            var max = Math.Max(Math.Max(r, g), b);
            var min = Math.Min(Math.Min(r, g), b);

            h = max;
            s = max;
            v = max;

            var d = max - min;
            s = max == 0 ? 0 : d / max;

            if (Math.Abs(max - min) < epsilon) // max == min
            {
                h = 0; // achromatic
            }
            else
            {
                if (Math.Abs(max - r) < epsilon) h = (g - b) / d + (g < b ? 6 : 0);
                if (Math.Abs(max - g) < epsilon) h = (b - r) / d + 2;
                if (Math.Abs(max - b) < epsilon) h = (r - g) / d + 4;
            }

            h /= 6;
        }
    }
}
