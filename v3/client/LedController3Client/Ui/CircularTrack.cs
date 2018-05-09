using LedController3Client.Ui.Core;
using SkiaSharp;
using System.Linq;

namespace LedController3Client.Ui
{
    public class CircularTrack : Component, IDrawerComponent
    {
        private readonly SKPoint _center;
        private readonly float _radius;
        private readonly float _thickness;
        private readonly SKColor _color;

        public CircularTrack(SKPoint center, float radius, float thickness, SKColor color)
        {
            _center = center;
            _radius = radius;
            _thickness = thickness;
            _color = color;
        }

        public void Draw(SKCanvas canvas, float scale)
        {
            var shader = SKShader.CreateColor(_color);
            var paint = new SKPaint() { Shader = shader, StrokeWidth = _thickness * scale, IsStroke = true, IsAntialias = true };
            canvas.DrawCircle(_center.X * scale, _center.Y * scale, _radius * scale, paint);
        }
    }
}
