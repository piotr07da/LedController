using LedController3Client.Communication;
using LedController3Client.Ui.Core;
using SkiaSharp;

namespace LedController3Client.Ui
{
    public class TimeProgressSlider : Component
    {
        private readonly ColorTimeLineDrawingConfig _drawingConfig;
        private readonly ColorTimeLineComponentsDimensionsConfig _worldDimensions;
        private readonly CircularTrack _track;
        private readonly Slider _slider;
        private readonly CycleTimeSlider _cycleTimeSlider;
        private readonly IPhotonLedControllerCommunicator _photonLedControllerCommunicator;

        public TimeProgressSlider(ColorTimeLineDrawingConfig drawingConfig, SKColor initialColor, float initialTimeProgress, IPhotonLedControllerCommunicator photonLedControllerCommunicator)
        {
            _drawingConfig = drawingConfig;
            _worldDimensions = _drawingConfig.WorldDimensions();
            _photonLedControllerCommunicator = photonLedControllerCommunicator;

            _track = new CircularTrack(_worldDimensions.Center, _worldDimensions.ProgressCircleRadius, _worldDimensions.ProgressCircleWidth, _drawingConfig.SliderTrackBackgroundColor);
            AddChild(_track);

            var sliderBody = new CircularSliderBody(_worldDimensions.Center, _worldDimensions.ProgressCircleRadius);
            _slider = new Slider(_drawingConfig, initialTimeProgress, initialColor, _worldDimensions.ProgressCircleWidth, false, true, sliderBody);
            _slider.ValueChanged += _slider_ValueChanged;
            AddChild(_slider);

            _cycleTimeSlider = new CycleTimeSlider(_drawingConfig, _photonLedControllerCommunicator);
            _cycleTimeSlider.IsEnabled = false;
            AddChild(_cycleTimeSlider);
        }

        public Slider Slider => _slider;
        public CycleTimeSlider CycleTimeSlider => _cycleTimeSlider;

        private void _slider_ValueChanged(object sender, EventArgs<float> e)
        {
            _photonLedControllerCommunicator.WriteTimeProgress(e.Data);
        }
    }
}
