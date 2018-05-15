using SkiaSharp;
using System;

namespace LedController3Client.Ui
{
    public class ColorsInterpolator
    {
        public void InterpolateColors(SKColor lColor, SKColor rColor, float ratio, out SKColor outColor)
        {
            var lColorMax = Math.Max(Math.Max(lColor.Red, lColor.Green), lColor.Blue);
            var rColorMax = Math.Max(Math.Max(rColor.Red, rColor.Green), rColor.Blue);
            var cColorMax = (lColorMax + rColorMax) / 2;

            var cColor = new SKColor(
                (byte)((lColor.Red + rColor.Red) / 2),
                (byte)((lColor.Green + rColor.Green) / 2),
                (byte)((lColor.Blue + rColor.Blue) / 2));

            var colorMax = Math.Max(Math.Max(cColor.Red, cColor.Green), cColor.Blue);
            if (colorMax > 0)
            {
                var mr = cColorMax / (float)colorMax;
                cColor = new SKColor(
                    (byte)(mr * cColor.Red),
                    (byte)(mr * cColor.Green),
                    (byte)(mr * cColor.Blue));
            }

            SKColor iColor;
            if (ratio < .5f)
            {
                iColor = lColor;
            }
            else
            {
                iColor = rColor;
                ratio = 1f - ratio;
            }

            ratio *= 2f;

            InterpolateColorsComponents(iColor.Red, cColor.Red, ratio, out byte r);
            InterpolateColorsComponents(iColor.Green, cColor.Green, ratio, out byte g);
            InterpolateColorsComponents(iColor.Blue, cColor.Blue, ratio, out byte b);

            outColor = new SKColor(r, g, b);
        }

        private void InterpolateColorsComponents(byte lColorComponent, byte rColorComponent, float ratio, out byte outColorComponent)
        {
            outColorComponent = (byte)(lColorComponent * (1f - ratio) + rColorComponent * ratio);
        }
    }
}
