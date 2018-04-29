using LedController3Client.Mobile.PhotonLedController;
using SkiaSharp;
using System;
using System.Linq;

namespace LedController3Client.Mobile.ColorTimeLineDrawing
{
    public class ColorTimeLineDrawingService
    {
        private SKImageInfo _imageInfo;
        private SKSurface _surface;
        private SKCanvas _canvas;
        private float _size;
        private ColorTimeLineDrawingConfig _cfg;
        private ColorTimeLineDrawingInput _inp;

        public void Init(SKImageInfo imageInfo, SKSurface surface)
        {
            _imageInfo = imageInfo;
            _surface = surface;
            _canvas = surface.Canvas;
            _size = _imageInfo.Width;
            _cfg = new ColorTimeLineDrawingConfig(_size);
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

            if (_inp.ColorTimePointSliders != null)
            {
                foreach (var ctps in _inp.ColorTimePointSliders)
                {
                    DrawSlider(ctps.Slider);
                }
            }

            if (_inp.TimeProgressSlider != null)
            {
                DrawSlider(_inp.TimeProgressSlider.Slider);
            }
        }

        private void DrawSlider(ColorTimeLineSlider slider)
        {
            var offset = CenterPoint();
            var sliderCenter = offset + new SKPoint(slider.X * _size, slider.Y * _size);

            var isSelected = slider == _inp.SelectedSlider;

            var radiusScale = !isSelected ? 1f : 1.3f;
            var radius = slider.Radius * radiusScale * _size;

            _canvas.DrawCircle(sliderCenter, radius + _cfg.BetweenCirclesMargin, new SKPaint() { Shader = SKShader.CreateColor(_cfg.BackgroundColor), IsStroke = false });
            _canvas.DrawCircle(sliderCenter, radius, new SKPaint() { Shader = SKShader.CreateColor(slider.Color), IsStroke = false });
            if (isSelected)
                _canvas.DrawCircle(sliderCenter, radius / 2f, new SKPaint() { Shader = SKShader.CreateColor(_cfg.BackgroundColor), IsStroke = false });
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
            var colors = _inp.ColorTimePointSliders.Select(ctps => ctps.Slider.Color).ToList();
            var positions = _inp.ColorTimePointSliders.Select(ctps => ctps.Slider.Time).ToList();

            var weldingColor = new ColorTimeLine(_inp.ColorTimePointSliders.Select(ctps => ctps.Slider).ToArray()).ColorAt(0);

            colors.Insert(0, weldingColor);
            colors.Add(weldingColor);
            positions.Insert(0, 0f);
            positions.Add(1f);

            return SKShader.CreateSweepGradient(CenterPoint(), colors.ToArray(), positions.ToArray());
        }

        private SKShader ProgressCircleShader()
        {
            return SKShader.CreateColor(_cfg.CirclesBackgroundColor);
        }

        private SKPoint CenterPoint()
        {
            return new SKPoint(_cfg.Center, _cfg.Center);
        }
    }
}
