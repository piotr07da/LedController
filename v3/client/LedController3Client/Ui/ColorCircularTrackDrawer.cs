using LedController3Client.Ui.Core;
using SkiaSharp;

namespace LedController3Client.Ui
{
    public class ColorCircularTrackDrawer : Component, IDrawerComponent
    {
        private readonly SKPoint _center;
        private readonly float _radius;
        private readonly float _thickness;
        private readonly SKColor _color;

        public ColorCircularTrackDrawer(SKPoint center, float radius, float thickness, SKColor color)
        {
            _center = center;
            _radius = radius;
            _thickness = thickness;
            _color = color;
        }

        public void Draw(SKCanvas canvas, float scale)
        {
            var paint = new SKPaint() { StrokeWidth = _thickness * scale, IsStroke = true, Color = _color };
            canvas.DrawCircle(_center.X * scale, _center.Y * scale, _radius * scale, paint);
        }
    }
}
