using SkiaSharp;

namespace LedController3Client.Ui
{
    public static class SKPointExtensions
    {
        public static SKPoint Multiply(this SKPoint point, float ratio)
        {
            return new SKPoint(point.X * ratio, point.Y * ratio);
        }
    }
}
