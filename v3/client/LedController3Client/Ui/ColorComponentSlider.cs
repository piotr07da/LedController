using LedController3Client.Ui.Core;
using SkiaSharp;
using System;

namespace LedController3Client.Ui
{
    public class ColorComponentSlider : Component
    {
        private readonly ColorTimeLineDrawingConfig _drawingConfig;
        private readonly ColorTimeLineComponentsDimensionsConfig _worldDimensions;
        private readonly ColorComponentType _colorComponentType;
        private readonly float _x0;
        private readonly float _x1;
        private readonly float _y;
        private SKPoint _p0;
        private SKPoint _p1;
        private readonly LinearTrack _track;
        private readonly Slider _slider;

        public ColorComponentSlider(ColorTimeLineDrawingConfig drawingConfig, ColorComponentType colorComponentType)
        {
            _drawingConfig = drawingConfig;
            _worldDimensions = _drawingConfig.WorldDimensions();
            _colorComponentType = colorComponentType;

            _x0 = _worldDimensions.InnerHorizontalSlidersX0;
            _x1 = _worldDimensions.InnerHorizontalSlidersX1;

            switch (_colorComponentType)
            {
                case ColorComponentType.R:
                    _y = _worldDimensions.InnerHorizontalSlidersY0of3;
                    break;
                case ColorComponentType.G:
                    _y = _worldDimensions.InnerHorizontalSlidersY1of3;
                    break;

                case ColorComponentType.B:
                    _y = _worldDimensions.InnerHorizontalSlidersY2of3;
                    break;
            }

            _p0 = new SKPoint(_x0, _y);
            _p1 = new SKPoint(_x1, _y);

            //_track = new LinearTrack(_p0, _p1, _worldDimensions.InnerHorizontalSliderBarWidth, new[] { new ColorPositionPair(SKColors.Black, 0f), new ColorPositionPair(ToColor(1f), 1f) });
            _track = new LinearTrack(_p0, _p1, _worldDimensions.InnerHorizontalSliderBarWidth, _drawingConfig.SliderTrackBackgroundColor);
            AddChild(_track);

            var sliderBody = new LinearSliderBody(_p0, _p1);
            _slider = new Slider(_drawingConfig, 0, SKColors.Black, _worldDimensions.InnerHorizontalSliderBarWidth, false, false, sliderBody);
            _slider.ValueChanged += _slider_ValueChanged;
            AddChild(_slider);
        }

        public byte ColorComponentValue => (byte)(_slider.Value * 255);

        public event EventHandler<EventArgs<SKColor>> ColorChanged;

        private void _slider_ValueChanged(object sender, EventArgs<float> e)
        {
            var value = e.Data;
            SetColorByValue(value);
            ColorChanged?.Invoke(this, new EventArgs<SKColor>(_slider.Color));
        }

        public void Reset(byte colorComponentValue)
        {
            var value = colorComponentValue / 255f;
            _slider.ResetValue(value);
            SetColorByValue(value);
        }

        private void SetColorByValue(float value)
        {
            _slider.Color = ToColor(value);
        }

        private SKColor ToColor(float value)
        {
            var ccv = (byte)(value * 255);

            switch (_colorComponentType)
            {
                case ColorComponentType.R:
                    return new SKColor(ccv, 0, 0);

                case ColorComponentType.G:
                    return new SKColor(0, ccv, 0);

                case ColorComponentType.B:
                    return new SKColor(0, 0, ccv);
            }
            return SKColors.White;
        }
    }

    public enum ColorComponentType { R, G, B }
}
