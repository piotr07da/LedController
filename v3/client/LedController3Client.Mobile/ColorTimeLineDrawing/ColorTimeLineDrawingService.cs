using LedController3Client.Mobile.PhotonLedController;
using SkiaSharp;
using System;
using System.Linq;

namespace LedController3Client.Mobile.ColorTimeLineDrawing
{
    public class ColorTimeLineSlider
    {
        public ColorTimeLineSlider(SKColor color, float time, float xOffset, float yOffset, float circleRadius, float sliderRadius)
        {
            Color = color;
            var angle = 2.0 * Math.PI * time;
            X = xOffset + circleRadius * (float)Math.Cos(angle);
            Y = yOffset + circleRadius * (float)Math.Sin(angle);
            Radius = sliderRadius;
        }

        public SKColor Color { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Radius { get; private set; }

        public bool HitTest(float x, float y)
        {
            var xd = x - X;
            var yd = y - Y;
            return xd * xd + yd * yd < Radius * Radius;
        }
    }

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

    public class ColorTimeLineDrawingService
    {
        private SKImageInfo _imageInfo;
        private SKSurface _surface;
        private SKCanvas _canvas;
        private ColorTimeLineDrawingConfig _cfg;
        private ColorTimeLineDrawingInput _inp;

        public void Init(SKImageInfo imageInfo, SKSurface surface, ColorTimeLineDrawingConfig config)
        {
            _imageInfo = imageInfo;
            _surface = surface;
            _canvas = surface.Canvas;
            _cfg = config;
        }

        public void Draw(ColorTimeLineDrawingInput input)
        {
            _inp = input;

            _canvas.Clear(_cfg.BackgroundColor);

            var centerPoint = CenterPoint();

            CirclesPaints(out SKPaint ccPaint, out SKPaint gcPaint, out SKPaint pcPaint);

            _canvas.DrawCircle(centerPoint, _cfg.ColorsCircleRadius, ccPaint);
            _canvas.DrawCircle(centerPoint, _cfg.GradientCircleRadius, gcPaint);
            _canvas.DrawCircle(centerPoint, _cfg.ProgressCircleRadius, pcPaint);
            
            foreach (var ctps in _inp.ColorTimePointSliders)
            {
                DrawSlider(ctps);
            }

            DrawSlider(_inp.TimeProgressSlider);
        }

        private void DrawSlider(ColorTimeLineSlider slider)
        {
            _canvas.DrawCircle(slider.X, slider.Y, slider.Radius + _cfg.BetweenCirclesMargin, new SKPaint() { Shader = SKShader.CreateColor(_cfg.BackgroundColor), IsStroke = false });
            _canvas.DrawCircle(slider.X, slider.Y, slider.Radius, new SKPaint() { Shader = SKShader.CreateColor(slider.Color), IsStroke = false });
        }

        private SKPoint CenterPoint()
        {
            return new SKPoint(_cfg.SizeDiv2, _cfg.SizeDiv2);
        }

        private void CirclesPaints(out SKPaint colorsCircleRadius, out SKPaint gradientCircleRadius, out SKPaint progressCircleRadius)
        {
            colorsCircleRadius = new SKPaint() { Shader = ColorsCircleShader(), StrokeWidth = _cfg.ColorsCircleWidth, IsStroke = true };
            gradientCircleRadius = new SKPaint() { Shader = GradientCircleShader(), StrokeWidth = _cfg.GradientCircleWidth, IsStroke = true };
            progressCircleRadius = new SKPaint() { Shader = ProgressCircleShader(), StrokeWidth = _cfg.ProgressCircleWidth, IsStroke = true };
        }

        private SKShader ColorsCircleShader()
        {
            return SKShader.CreateColor(_cfg.CirclesBackgroundColor);
        }

        private SKShader GradientCircleShader()
        {
            return SKShader.CreateSweepGradient(new SKPoint(_cfg.SizeDiv2, _cfg.SizeDiv2), _inp.Gradient.Colors, _inp.Gradient.Positions);
        }

        private SKShader ProgressCircleShader()
        {
            return SKShader.CreateColor(_cfg.CirclesBackgroundColor);
        }

        

        
    }
}
