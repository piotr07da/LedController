using LedController3Client.Ui.Core;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace LedController3Client.Ui
{
    public class HsvColorPicker : Component, IColorPicker
    {
        private readonly ColorTimeLineDrawingConfig _drawingConfig;
        private readonly ColorTimeLineComponentsDimensionsConfig _worldDimensions;

        private readonly HsTrack _hsTrack;
        private readonly HsSliderBody _hsSliderBody;
        private readonly Slider<SKColor> _hsSlider;

        private readonly CircularTrack _vTrack;
        private readonly CircularSliderBody _vSliderBody;
        private readonly Slider<float> _vSlider;

        public HsvColorPicker(ColorTimeLineDrawingConfig drawingConfig)
        {
            _drawingConfig = drawingConfig;
            _worldDimensions = _drawingConfig.WorldDimensions();

            _hsTrack = new HsTrack(_drawingConfig);
            AddChild(_hsTrack);

            _vTrack = new CircularTrack(_worldDimensions.Center, _worldDimensions.HsvColorPickerValueCircleRadius, _worldDimensions.HsvColorPickerValueCircleWidth, SKColors.White);
            AddChild(_vTrack);

            _hsSliderBody = new HsSliderBody(_worldDimensions.Center, _worldDimensions.HsvColorPickerHueSaturationCircleRadius);
            _hsSlider = new Slider<SKColor>(_drawingConfig, SKColors.Black, SKColors.Black, _worldDimensions.HsvColorPcikerHueSaturationSliderRadius, false, false, _hsSliderBody);
            _hsSlider.ValueChanged += _slider_ValueChanged;
            AddChild(_hsSlider);
            
            _vSliderBody = new CircularSliderBody(_worldDimensions.Center, _worldDimensions.HsvColorPickerValueCircleRadius);
            _vSlider = new Slider<float>(_drawingConfig, 0, SKColors.Black, _worldDimensions.HsvColorPickerValueCircleWidth / 2f, false, false, _vSliderBody);
            _vSlider.ValueChanged += _vSlider_ValueChanged;
            AddChild(_vSlider);
        }

        public event EventHandler<EventArgs<SKColor>> ColorChanged;

        private void _slider_ValueChanged(object sender, EventArgs<SKColor> e)
        {
            var color = e.Data;
            _hsSlider.Color = color;
            ColorChanged?.Invoke(this, new EventArgs<SKColor>(color));
        }

        private void _vSlider_ValueChanged(object sender, EventArgs<float> e)
        {
            
        }

        public void ResetColor(SKColor color)
        {
            _hsSlider.Color = color;
            _hsSlider.ResetValue(color);
        }

        public void Draw(SKCanvas canvas, float scale)
        {
            
        }

        
    }

    
}
