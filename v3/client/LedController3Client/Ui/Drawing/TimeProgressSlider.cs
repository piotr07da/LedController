using LedController3Client.Communication;
using System;

namespace LedController3Client.Ui.Drawing
{
    public class TimeProgressSlider
    {
        private const float MinCycleTime = 3 * 1000;
        private const float MaxCycleTime = 24 * 60 * 60 * 1000;
        private static readonly float Base = (float)Math.E;

        private readonly ISlider _slider;
        private readonly ISlider _cycleTimeSlider;
        private readonly IPhotonLedControllerCommunicator _photonLedControllerCommunicator;

        public TimeProgressSlider(ISlider slider, ISlider cycleTimeSlider, IPhotonLedControllerCommunicator photonLedControllerCommunicator)
        {
            _slider = slider;
            _cycleTimeSlider = cycleTimeSlider;
            _photonLedControllerCommunicator = photonLedControllerCommunicator;

            _slider.ValueChanged += _slider_ValueChanged;
            _slider.IsSelectedChanged += _slider_IsSelectedChanged;

            _cycleTimeSlider.ValueChanged += CycleTimeSlider_ValueChanged;
        }

        public ISlider Slider => _slider;

        private void _slider_ValueChanged(object sender, EventArgs<float> e)
        {
            _photonLedControllerCommunicator.WriteTimeProgress(e.Data);
        }

        private void _slider_IsSelectedChanged(object sender, EventArgs<bool> e)
        {
            _cycleTimeSlider.IsVisible = e.Data;
            // TODO - somehow access cycleTime here
            //_cycleTimeSlider.Value = (float)Math.Log(Base, )
        }

        private void CycleTimeSlider_ValueChanged(object sender, EventArgs<float> e)
        {
            float value = e.Data;
            float a = (float)Math.Log(MaxCycleTime / MinCycleTime, Base);

            // min * b ^ a = max
            // b ^ a = max / min
            // log b (max/min) = a
            // a = log b (max/min)

            int cycleTime = (int)(MinCycleTime * Math.Pow(Base, a * value)); // it gives cycleTime in range [minTime; maxTime] for value in range [0; 1]

            _photonLedControllerCommunicator.WriteCycleTime(cycleTime);
            _photonLedControllerCommunicator.ReadCycleTime();
        }
    }
}
