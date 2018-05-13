using LedController3Client.Communication;
using LedController3Client.Ui.Core;
using SkiaSharp;
using System;

namespace LedController3Client.Ui
{
    public class CycleTimeSlider : Component
    {
        // min * b ^ a = max
        // b ^ a = max / min
        // log b (max/min) = a
        // a = log b (max/min)
        private const float MinCycleTime = 3 * 1000;
        private const float MaxCycleTime = 24 * 60 * 60 * 1000;
        private static readonly float Base = (float)Math.E;
        private static readonly float Alpha = (float)Math.Log(MaxCycleTime / MinCycleTime, Base);

        private readonly ColorTimeLineDrawingConfig _drawingConfig;
        private readonly ColorTimeLineComponentsDimensionsConfig _worldDimensions;
        private readonly IPhotonLedControllerCommunicator _photonLedControllerCommunicator;
        private readonly float _x0;
        private readonly float _x1;
        private readonly float _y;
        private SKPoint _p0;
        private SKPoint _p1;
        private readonly LinearTrack _track;
        private readonly Slider<float> _slider;

        public CycleTimeSlider(ColorTimeLineDrawingConfig drawingConfig, IPhotonLedControllerCommunicator photonLedControllerCommunicator)
        {
            _drawingConfig = drawingConfig;
            _worldDimensions = _drawingConfig.WorldDimensions();
            _photonLedControllerCommunicator = photonLedControllerCommunicator;

            _x0 = _worldDimensions.InnerHorizontalSlidersX0;
            _x1 = _worldDimensions.InnerHorizontalSlidersX1;
            _y = _worldDimensions.InnerHorizontalSlidersY0of1;
            _p0 = new SKPoint(_x0, _y);
            _p1 = new SKPoint(_x1, _y);

            _track = new LinearTrack(_p0, _p1, _worldDimensions.InnerHorizontalSliderBarWidth, _drawingConfig.SliderTrackBackgroundColor);
            AddChild(_track);

            var sliderBody = new LinearSliderBody(_p0, _p1);
            _slider = new Slider<float>(_drawingConfig, 0, SKColors.Black, _worldDimensions.InnerHorizontalSliderBarWidth, false, false, sliderBody);
            _slider.ValueChanged += _slider_ValueChanged;
            AddChild(_slider);
        }

        public void UpdateCycleTime(int cycleTime)
        {
            float value = (float)Math.Log(cycleTime / MinCycleTime, Base) / Alpha;
            _slider.ResetValue(value);
        }

        private void _slider_ValueChanged(object sender, EventArgs<float> e)
        {
            float value = e.Data;
            int cycleTime = (int)(MinCycleTime * Math.Pow(Base, Alpha * value)); // it gives cycleTime in range [minTime; maxTime] for value in range [0; 1]

            _photonLedControllerCommunicator.WriteCycleTime(cycleTime);
            _photonLedControllerCommunicator.ReadCycleTime();
        }
    }
}
