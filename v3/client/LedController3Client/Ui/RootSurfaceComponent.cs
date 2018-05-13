using LedController3Client.Communication;
using LedController3Client.Ui.Core;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace LedController3Client.Ui
{
    public class RootSurfaceComponent : Component, IDrawerComponent
    {
        private readonly IPhotonLedControllerCommunicator _photonLedControllerCommunicator;

        private readonly ColorTimeLineDrawingConfig _drawingConfig;
        private readonly ColorTimeLineComponentsDimensionsConfig _worldDimensions;

        private int _cycleTime;
        private float _timeProgress;

        private CircularTrack _colorTimePointsTrack;
        private readonly List<ColorTimePointSlider> _colorTimePointSliders;
        private readonly IColorPicker _colorPicker;
        
        private readonly TimeProgressSlider _timeProgressSlider;

        private GradientCircleDrawerComponent _gradientCircleDrawer;

        public RootSurfaceComponent(IPhotonLedControllerCommunicator photonLedControllerCommunicator)
        {
            // Dependencies.

            _photonLedControllerCommunicator = photonLedControllerCommunicator;

            // Drawing configuration.

            _drawingConfig = new ColorTimeLineDrawingConfig();
            _worldDimensions = _drawingConfig.WorldDimensions();

            // Track for color time points.

            _colorTimePointsTrack = new CircularTrack(_worldDimensions.Center, _worldDimensions.ColorsCircleRadius, _worldDimensions.ColorsCircleWidth, _drawingConfig.SliderTrackBackgroundColor);
            AddChild(_colorTimePointsTrack);

            // Container for color time point sliders - the only sliders which number is dynamic so generic list is used here.

            _colorTimePointSliders = new List<ColorTimePointSlider>();

            // Gradient circle component.

            _gradientCircleDrawer = new GradientCircleDrawerComponent(_drawingConfig, _colorTimePointSliders);
            AddChild(_gradientCircleDrawer);

            // Color picker.
            _colorPicker = new HsvColorPicker(_drawingConfig);
            _colorPicker.IsEnabled = false;
            AddChild(_colorPicker);

            // Time progress slider with cycle time slider inside.

            _timeProgressSlider = new TimeProgressSlider(_drawingConfig, SKColors.Black, _timeProgress, _photonLedControllerCommunicator);
            _timeProgressSlider.Slider.IsSelectedChanged += Slider_IsSelectedChanged;
            AddChild(_timeProgressSlider);
        }

        public void UpdateCycleTime(int cycleTime)
        {
            _cycleTime = cycleTime;

            _timeProgressSlider.CycleTimeSlider.UpdateCycleTime(_cycleTime);
        }

        public void UpdateTimeProgress(float timeProgress)
        {
            _timeProgress = timeProgress;

            _timeProgressSlider.Slider.Color = CurrentColor();
            _timeProgressSlider.Slider.ResetValue(_timeProgress);
        }

        public void UpdateColorTimePointSliders(ColorTimePoint[] colorTimePoints)
        {
            foreach (var ctps in _colorTimePointSliders)
            {
                RemoveChild(ctps);
                ctps.Slider.IsSelectedChanged -= Slider_IsSelectedChanged;
            }

            var currentSliders = _colorTimePointSliders.ToDictionary(ctps => ctps.Id);
            var updatedSliders = new List<ColorTimePointSlider>();
            foreach (var ctp in colorTimePoints)
            {
                if (!currentSliders.TryGetValue(ctp.Id, out ColorTimePointSlider ctps))
                {
                    ctps = new ColorTimePointSlider(_drawingConfig, ctp.Id, Convert(ctp.Color), ctp.Time, _colorPicker, _photonLedControllerCommunicator);
                }
                else
                {
                    ctps.Slider.Color = Convert(ctp.Color);
                    ctps.Slider.Value = ctp.Time;
                }
                updatedSliders.Add(ctps);
            }

            _colorTimePointSliders.Clear();
            _colorTimePointSliders.AddRange(updatedSliders);

            foreach (var ctps in _colorTimePointSliders)
            {
                AddChild(ctps);
                ctps.Slider.IsSelectedChanged += Slider_IsSelectedChanged;
            }

            // Update time progress slider.

            _timeProgressSlider.Slider.Color = CurrentColor();
        }

        private void Slider_IsSelectedChanged(object sender, EventArgs<bool> e)
        {
            _colorPicker.IsEnabled = false;
            _timeProgressSlider.CycleTimeSlider.IsEnabled = false;

            if (!e.Data)
                return;

            var selectedSlider = sender as ISlider;

            var selectableSliders = _colorTimePointSliders.Select(ctps => ctps.Slider).Union(new[] { _timeProgressSlider.Slider });

            foreach (var ss in selectableSliders)
            {
                if (ss.IsSelected && ss != selectedSlider)
                    ss.ResetIsSelected(false);
            }

            if (_colorTimePointSliders.Select(ctps => ctps.Slider).Contains(selectedSlider))
                _colorPicker.IsEnabled = true;
            if (_timeProgressSlider.Slider == selectedSlider)
                _timeProgressSlider.CycleTimeSlider.IsEnabled = true;
        }

        public void Draw(SKCanvas canvas, float scale)
        {
            canvas.Clear(_drawingConfig.BackgroundColor);
        }

        private SKColor CurrentColor()
        {
            return new ColorTimeLine(_colorTimePointSliders.Select(ctps => ctps.Slider).ToArray()).ColorAt(_timeProgress);
        }

        private SKColor Convert(ColorTimePointColor color)
        {
            return new SKColor(color.R, color.G, color.B);
        }
    }
}
