using SkiaSharp;

namespace LedController3Client.Ui
{
    public class ColorsInterpolator
    {
        public void InterpolateColors(SKColor lColor, SKColor rColor, float ratio, out SKColor outColor)
        {
            InterpolateColorsComponents(lColor.Red, rColor.Red, ratio, out byte r);
            InterpolateColorsComponents(lColor.Green, rColor.Green, ratio, out byte g);
            InterpolateColorsComponents(lColor.Blue, rColor.Blue, ratio, out byte b);
            outColor = new SKColor(r, g, b);
        }

        private void InterpolateColorsComponents(byte lColorComponent, byte rColorComponent, float ratio, out byte outColorComponent)
        {
            outColorComponent = (byte)(lColorComponent * (1f - ratio) + rColorComponent * ratio);
        }
    }
}
