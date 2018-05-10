using LedController3Client.Ui.Core;
using SkiaSharp;

namespace LedController3Client.Ui
{
    public class SliderDrawerComponent : Component, IDrawerComponent
    {
        private readonly ColorTimeLineDrawingConfig _drawingConfig;
        private readonly ISlider _slider;

        public SliderDrawerComponent(ColorTimeLineDrawingConfig drawingConfig, ISlider slider)
        {
            _drawingConfig = drawingConfig;
            _slider = slider;
        }

        public void Draw(SKCanvas canvas, float scale)
        {
            var screenDimensions = _drawingConfig.ScreenDimensions(scale);

            var sliderCenter = new SKPoint(_slider.Position.X * scale, _slider.Position.Y * scale);

            var radiusScale = !_slider.IsSelected ? 1f : 1.3f;
            var radius = _slider.Radius * radiusScale * scale;

            canvas.DrawCircle(sliderCenter, radius + screenDimensions.BetweenCirclesMargin, new SKPaint() { Shader = SKShader.CreateColor(_drawingConfig.BackgroundColor), IsStroke = false, IsAntialias = true });
            canvas.DrawCircle(sliderCenter, radius, new SKPaint() { Shader = SKShader.CreateColor(_slider.Color), IsStroke = false, IsAntialias = true });
            if (_slider.IsSelected)
                canvas.DrawCircle(sliderCenter, radius / 2f, new SKPaint() { Shader = SKShader.CreateColor(_drawingConfig.BackgroundColor), IsStroke = false, IsAntialias = true });
        }
    }
}
