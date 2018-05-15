using LedController3Client.Ui.Core;
using SkiaSharp;
using System;

namespace LedController3Client.Ui
{
    public class HsvColorPicker : Component, IColorPicker
    {
        private readonly ColorTimeLineDrawingConfig _drawingConfig;
        private readonly ColorTimeLineComponentsDimensionsConfig _worldDimensions;
        private readonly float _hsAreaRadius;

        private readonly HsvRgbConverter _hsvRgb;

        private readonly ColorPositions _vTrackColorPositions;
        private readonly HsTrack _hsTrack;
        private readonly HsSliderBody _hsSliderBody;
        private readonly Slider<HueSaturation> _hsSlider;

        private readonly CircularTrack _vTrack;
        private readonly CircularSliderBody _vSliderBody;
        private readonly Slider<float> _vSlider;

        public HsvColorPicker(ColorTimeLineDrawingConfig drawingConfig)
        {
            _drawingConfig = drawingConfig;
            _worldDimensions = _drawingConfig.WorldDimensions();
            _hsAreaRadius = _worldDimensions.HsvColorPickerHueSaturationCircleRadius;

            _hsvRgb = new HsvRgbConverter();

            _hsTrack = new HsTrack(_drawingConfig);
            AddChild(_hsTrack);

            _vTrackColorPositions = new ColorPositions(new[] { new ColorPosition(SKColors.Black, .25f), new ColorPosition(SKColors.White, .75f) });
            _vTrack = new CircularTrack(_worldDimensions.Center, _worldDimensions.HsvColorPickerValueCircleRadius, _worldDimensions.HsvColorPickerValueCircleWidth, 36, _vTrackColorPositions);
            AddChild(_vTrack);

            _hsSliderBody = new HsSliderBody(_worldDimensions.Center, _hsAreaRadius);
            _hsSlider = new Slider<HueSaturation>(_drawingConfig, new HueSaturation(0, 0), SKColors.Black, _worldDimensions.HsvColorPcikerHueSaturationSliderRadius, false, false, _hsSliderBody);
            _hsSlider.ValueChanged += _hsSlider_ValueChanged;
            AddChild(_hsSlider);
            
            _vSliderBody = new CircularSliderBody(_worldDimensions.Center, _worldDimensions.HsvColorPickerValueCircleRadius, ConvertHSliderInputValue, ConvertHSliderOutputValue);
            _vSlider = new Slider<float>(_drawingConfig, 0, SKColors.Black, _worldDimensions.HsvColorPickerValueCircleWidth / 2f, false, false, _vSliderBody);
            _vSlider.ValueChanged += _vSlider_ValueChanged;
            AddChild(_vSlider);
        }

        public event EventHandler<EventArgs<SKColor>> ColorChanged;

        private void _hsSlider_ValueChanged(object sender, EventArgs<HueSaturation> e)
        {
            OnSliderValueChanged();
        }

        private void _vSlider_ValueChanged(object sender, EventArgs<float> e)
        {
            OnSliderValueChanged();
        }

        public void ResetColor(SKColor color)
        {
            _hsvRgb.Rgb2Hsv(color, out float h, out float s, out float v);

            _hsSlider.Color = color;
            _hsSlider.ResetValue(new HueSaturation(h, s));
             
            _vSlider.Color = color;
            _vSlider.ResetValue(v);

            UpdateHsTrackBrightness(v);
            UpdateVTrackColor(h, s);
        }

        private void OnSliderValueChanged()
        {
            var hs = _hsSlider.Value;
            var h = hs.H;
            var s = hs.S;
            var v = _vSlider.Value;

            _hsvRgb.Hsv2Rgb(h, s, v, out SKColor color);

            _hsSlider.Color = color;
            _vSlider.Color = color;

            UpdateHsTrackBrightness(v);
            UpdateVTrackColor(h, s);

            ColorChanged?.Invoke(this, new EventArgs<SKColor>(color));
        }

        private void UpdateHsTrackBrightness(float brightness)
        {
            _hsTrack.UpdateBrightness(brightness);
        }

        private void UpdateVTrackColor(float h, float s)
        {
            _hsvRgb.Hsv2Rgb(h, s, 1f, out SKColor color);
            _vTrackColorPositions.Update(new[]
            {
                new ColorPosition(SKColors.Black, .25f),
                new ColorPosition(color, .75f),
            });
        }

        private float ConvertHSliderInputValue(float value)
        {
            value *= -.5f;
            value += .25f;
            if (value < 0)
                value += 1f;
            return value;
        }

        private float ConvertHSliderOutputValue(float value)
        {
            value += .75f; // angle offset
            value %= 1f; // keep angle in 0-1 range
            value *= 2f; // double angle
            if (value > 1f) // treat second half of circle as decreasing value
                value = 2f - value;
            return value;
        }
    }
}
