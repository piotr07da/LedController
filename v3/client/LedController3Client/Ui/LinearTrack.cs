using LedController3Client.Ui.Core;
using SkiaSharp;
using System.Linq;

namespace LedController3Client.Ui
{
    public class LinearTrack : Component, IDrawerComponent
    {
        private readonly SKPoint _p0;
        private readonly SKPoint _p1;
        private readonly float _thickness;
        private readonly SKColor? _color;
        private readonly ColorPosition[] _colorPositions;

        public LinearTrack(SKPoint p0, SKPoint p1, float thickness, SKColor color)
            : this(p0, p1, thickness)
        {
            _color = color;
        }

        public LinearTrack(SKPoint p0, SKPoint p1, float thickness, ColorPosition[] colorPositions)
            :this(p0, p1, thickness)
        {
            _colorPositions = colorPositions;
        }

        private LinearTrack(SKPoint p0, SKPoint p1, float thickness)
        {
            _p0 = p0;
            _p1 = p1;
            _thickness = thickness;
        }

        public void Draw(SKCanvas canvas, float scale)
        {
            var shader = Shader(scale);
            var paint = new SKPaint() { Shader = shader, StrokeWidth = _thickness * scale, IsStroke = true, IsAntialias = true, StrokeCap = SKStrokeCap.Round };
            canvas.DrawLine(_p0.Multiply(scale), _p1.Multiply(scale), paint);
        }

        private SKShader Shader(float scale)
        {
            if (_color != null)
                return SKShader.CreateColor(_color.Value);
            if (_colorPositions != null)
                return SKShader.CreateLinearGradient(_p0.Multiply(scale), _p1.Multiply(scale), _colorPositions.Select(cp => cp.Color).ToArray(), _colorPositions.Select(cp => cp.Position).ToArray(), SKShaderTileMode.Clamp);
            return SKShader.CreateColor(SKColors.Red);
        }
    }
}
