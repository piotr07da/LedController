using SkiaSharp;
using System.Linq;

namespace LedController3Client.Ui.Drawing
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

            CirclesPaints(out SKPaint ccPaint, out SKPaint gcPaint, out SKPaint pcPaint);

            _canvas.DrawCircle(_cfg.Center, _cfg.ColorsCircleRadius, ccPaint);
            _canvas.DrawCircle(_cfg.Center, _cfg.GradientCircleRadius, gcPaint);
            _canvas.DrawCircle(_cfg.Center, _cfg.ProgressCircleRadius, pcPaint);

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

            if (_inp.ColorComponentSliders != null)
            {
                foreach(var ccs in _inp.ColorComponentSliders)
                {
                    DrawSlider(ccs);
                }
            }

            if (_inp.CycleTimeSlider != null)
            {
                DrawSlider(_inp.CycleTimeSlider);
            }
        }

        private void DrawSlider(ISlider slider)
        {
            if (!slider.IsVisible)
                return;

            var sliderCenter = new SKPoint(slider.Position.X * _size, slider.Position.Y * _size);

            var radiusScale = !slider.IsSelected ? 1f : 1.3f;
            var radius = slider.Radius * radiusScale * _size;

            _canvas.DrawCircle(sliderCenter, radius + _cfg.BetweenCirclesMargin, new SKPaint() { Shader = SKShader.CreateColor(_cfg.BackgroundColor), IsStroke = false, IsAntialias = true });
            _canvas.DrawCircle(sliderCenter, radius, new SKPaint() { Shader = SKShader.CreateColor(slider.Color), IsStroke = false, IsAntialias = true });
            if (slider.IsSelected)
                _canvas.DrawCircle(sliderCenter, radius / 2f, new SKPaint() { Shader = SKShader.CreateColor(_cfg.BackgroundColor), IsStroke = false, IsAntialias = true });
        }

        private void CirclesPaints(out SKPaint colorsCircleRadius, out SKPaint gradientCircleRadius, out SKPaint progressCircleRadius)
        {
            colorsCircleRadius = new SKPaint() { Shader = ColorsCircleShader(), StrokeWidth = _cfg.ColorsCircleWidth, IsStroke = true, IsAntialias = true };
            gradientCircleRadius = new SKPaint() { Shader = GradientCircleShader(), StrokeWidth = _cfg.GradientCircleWidth, IsStroke = true, IsAntialias = true };
            progressCircleRadius = new SKPaint() { Shader = ProgressCircleShader(), StrokeWidth = _cfg.ProgressCircleWidth, IsStroke = true, IsAntialias = true };
        }

        private SKShader ColorsCircleShader()
        {
            return SKShader.CreateColor(_cfg.CirclesBackgroundColor);
        }

        private SKShader GradientCircleShader()
        {
            var colors = _inp.ColorTimePointSliders.Select(ctps => ctps.Slider.Color).ToList();
            var positions = _inp.ColorTimePointSliders.Select(ctps => ctps.Slider.Value).ToList();

            var weldingColor = new ColorTimeLine(_inp.ColorTimePointSliders.Select(ctps => ctps.Slider).ToArray()).ColorAt(0);

            colors.Insert(0, weldingColor);
            colors.Add(weldingColor);
            positions.Insert(0, 0f);
            positions.Add(1f);

            return SKShader.CreateSweepGradient(_cfg.Center, colors.ToArray(), positions.ToArray());
        }

        private SKShader ProgressCircleShader()
        {
            return SKShader.CreateColor(_cfg.CirclesBackgroundColor);
        }
    }
}
