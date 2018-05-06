
using LedController3Client.Communication;
using SkiaSharp;

namespace LedController3Client.Ui.Drawing
{
    public class ColorTimePointSlider
    {
        private readonly byte _id;
        private readonly ISlider _slider;
        private readonly ISlider[] _colorComponentSliders;
        private readonly IPhotonLedControllerCommunicator _photonLedControllerCommunicator;

        public ColorTimePointSlider(byte id, ISlider slider, ISlider[] colorComponentSliders, IPhotonLedControllerCommunicator photonLedControllerCommunicator)
        {
            _id = id;
            _slider = slider;
            _colorComponentSliders = colorComponentSliders;
            _photonLedControllerCommunicator = photonLedControllerCommunicator;

            _slider.ValueChanged += _slider_ValueChanged;
            _slider.IsSelectedChanged += _slider_IsSelectedChanged;

            foreach(var ccs in _colorComponentSliders)
                ccs.ValueChanged += ColorComponentSlider_ValueChanged;
        }

        public byte Id => _id;

        public ISlider Slider => _slider;

        private void _slider_ValueChanged(object sender, EventArgs<float> e)
        {
            _photonLedControllerCommunicator.WriteColorTimePointTime(_id, e.Data);
            _photonLedControllerCommunicator.ReadColorTimePoints();
        }

        private void _slider_IsSelectedChanged(object sender, EventArgs<bool> e)
        {
            foreach (var ccs in _colorComponentSliders)
            {
                ccs.IsVisible = e.Data;
            }

            if (!e.Data)
                return;

            _colorComponentSliders[0].Color = new SKColor(_slider.Color.Red, 0, 0);
            _colorComponentSliders[1].Color = new SKColor(0, _slider.Color.Green, 0);
            _colorComponentSliders[2].Color = new SKColor(0, 0, _slider.Color.Blue);

            foreach (var ccs in _colorComponentSliders)
                ccs.ValueChanged -= ColorComponentSlider_ValueChanged;

            _colorComponentSliders[0].Value = _slider.Color.Red / 255f;
            _colorComponentSliders[1].Value = _slider.Color.Green / 255f;
            _colorComponentSliders[2].Value = _slider.Color.Blue / 255f;

            foreach (var ccs in _colorComponentSliders)
                ccs.ValueChanged += ColorComponentSlider_ValueChanged;
        }

        private void ColorComponentSlider_ValueChanged(object sender, EventArgs<float> e)
        {
            // If color components are changing but those color components aren't from this particular color time point - because this one is not selected.
            if (!_slider.IsSelected)
                return;

            _photonLedControllerCommunicator.WriteColorTimePointColor(_id, new ColorTimePointColor((byte)(_colorComponentSliders[0].Value * 255), (byte)(_colorComponentSliders[1].Value * 255), (byte)(_colorComponentSliders[2].Value * 255)));
            _photonLedControllerCommunicator.ReadColorTimePoints();
        }
    }
}
