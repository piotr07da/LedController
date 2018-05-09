using LedController3Client.Ui.Core;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace LedController3Client.Ui
{
    public class GradientCircleDrawerComponent : Component, IDrawerComponent
    {
        private readonly ColorTimeLineDrawingConfig _drawingConfig;
        private readonly IEnumerable<ColorTimePointSlider> _colorTimePointSliders;

        public GradientCircleDrawerComponent(ColorTimeLineDrawingConfig drawingConfig, IEnumerable<ColorTimePointSlider> colorTimePointSliders)
        {
            _drawingConfig = drawingConfig;
            _colorTimePointSliders = colorTimePointSliders;
        }

        public void Draw(SKCanvas canvas, float scale)
        {
            var screenDimensions = _drawingConfig.ScreenDimensions(scale);
            var paint = new SKPaint() { Shader = GradientCircleShader(screenDimensions), StrokeWidth = screenDimensions.GradientCircleWidth * 1f, IsStroke = true, IsAntialias = true };
            canvas.DrawCircle(screenDimensions.Center, screenDimensions.GradientCircleRadius, paint);
        }

        private SKShader GradientCircleShader(ColorTimeLineComponentsDimensionsConfig screenDimensions)
        {
            var colors = _colorTimePointSliders.Select(ctps => ctps.Slider.Color).ToList();
            var positions = _colorTimePointSliders.Select(ctps => ctps.Slider.Value).ToList();

            var weldingColor = new ColorTimeLine(_colorTimePointSliders.Select(ctps => ctps.Slider).ToArray()).ColorAt(0);

            colors.Insert(0, weldingColor);
            colors.Add(weldingColor);
            positions.Insert(0, 0f);
            positions.Add(1f);

            return SKShader.CreateSweepGradient(screenDimensions.Center, colors.ToArray(), positions.ToArray());
        }
    }
}
