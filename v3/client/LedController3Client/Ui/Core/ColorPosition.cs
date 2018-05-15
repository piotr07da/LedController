using SkiaSharp;

namespace LedController3Client.Ui.Core
{
    public class ColorPosition
    {
        public ColorPosition(SKColor color, float position)
        {
            Color = color;
            Position = position;
        }

        public SKColor Color { get; set; }
        public float Position { get; set; }
    }
}
