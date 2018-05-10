using LedController3Client.Communication;
using LedController3Client.Ui.Core;
using SkiaSharp;

namespace LedController3Client.Ui
{
    public class ColorTimePointSlider : Component
    {
        private readonly ColorTimeLineDrawingConfig _drawingConfig;
        private readonly ColorTimeLineComponentsDimensionsConfig _worldDimensions;
        private readonly byte _id;
        private readonly Slider<float> _slider;
        private readonly IColorPicker _colorPicker;
        private readonly IPhotonLedControllerCommunicator _photonLedControllerCommunicator;

        public ColorTimePointSlider(ColorTimeLineDrawingConfig drawingConfig, byte id, SKColor color, float time, IColorPicker colorPicker, IPhotonLedControllerCommunicator photonLedControllerCommunicator)
        {
            _drawingConfig = drawingConfig;
            _worldDimensions = _drawingConfig.WorldDimensions();
            _id = id;
            var sliderBody = new CircularSliderBody(_worldDimensions.Center, _worldDimensions.ColorsCircleRadius);
            _slider = new Slider<float>(_drawingConfig, time, color, _worldDimensions.ColorsCircleWidth, false, true, sliderBody);
            AddChild(_slider);
            _colorPicker = colorPicker;
            _photonLedControllerCommunicator = photonLedControllerCommunicator;

            _slider.ValueChanged += _slider_ValueChanged;
            _slider.IsSelectedChanged += _slider_IsSelectedChanged;

            _colorPicker.ColorChanged += ColorComponent_ColorChanged;
        }

        public byte Id => _id;

        public Slider<float> Slider => _slider;

        private void _slider_ValueChanged(object sender, EventArgs<float> e)
        {
            _photonLedControllerCommunicator.WriteColorTimePointTime(_id, e.Data);
            _photonLedControllerCommunicator.ReadColorTimePoints();
        }

        private void _slider_IsSelectedChanged(object sender, EventArgs<bool> e)
        {
            if (!e.Data)
                return;

            _colorPicker.ResetColor(_slider.Color);
        }

        private void ColorComponent_ColorChanged(object sender, EventArgs<SKColor> e)
        {
            // If color components are changing but those color components aren't from this particular color time point - because this one is not selected.
            if (!_slider.IsSelected)
                return;

            var c = e.Data;

            _photonLedControllerCommunicator.WriteColorTimePointColor(_id, new ColorTimePointColor(c.Red, c.Green, c.Blue));
            _photonLedControllerCommunicator.ReadColorTimePoints();
        }
    }
}
