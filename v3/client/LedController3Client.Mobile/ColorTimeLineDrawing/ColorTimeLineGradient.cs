using SkiaSharp;

namespace LedController3Client.Mobile.ColorTimeLineDrawing
{
    public class ColorTimeLineGradient
    {
        public ColorTimeLineGradient(SKColor[] colors, float[] positions)
        {
            Colors = colors;
            Positions = positions;
        }

        public SKColor[] Colors { get; private set; }
        public float[] Positions { get; private set; }
    }
}
