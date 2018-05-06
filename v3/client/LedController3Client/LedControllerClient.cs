using LedController3Client.Communication;
using LedController3Client.Ui.Drawing;
using SkiaSharp;
using System.Linq;
using System.Collections.Generic;
using System;

namespace LedController3Client
{
    public class LedControllerClient
    {
        private IPhotonLedControllerCommunicator _photonLedControllerCommunicator;
        private IPhotonLedControllerPollingService _photonLedControllerPollingService;

        private float _timeProgress;

        private ColorTimeLineDrawingConfig _drawingConfig;
        private ColorTimeLineDrawingInput _drawingInput;

        private long? _currentTouchId;
        private ISlider _pressedSlider;
        private ISlider _movedSlider;
        private ISlider _selectedSlider;

        public event Action RefreshSurfaceRequested;
         
        public void Start()
        {
            _drawingConfig = new ColorTimeLineDrawingConfig(1f);
            _drawingInput = new ColorTimeLineDrawingInput();
            _drawingInput.ColorComponentSliders = new ISlider[]
            {
                new Slider(0, SKColors.Black, _drawingConfig.InnerHorizontalSliderBarWidth, false, false, new LinearSliderBody(new SKPoint(_drawingConfig.InnerHorizontalSlidersX0, _drawingConfig.InnerHorizontalSlidersY0of3), new SKPoint(_drawingConfig.InnerHorizontalSlidersX1, _drawingConfig.InnerHorizontalSlidersY0of3))),
                new Slider(0, SKColors.Black, _drawingConfig.InnerHorizontalSliderBarWidth, false, false, new LinearSliderBody(new SKPoint(_drawingConfig.InnerHorizontalSlidersX0, _drawingConfig.InnerHorizontalSlidersY1of3), new SKPoint(_drawingConfig.InnerHorizontalSlidersX1, _drawingConfig.InnerHorizontalSlidersY1of3))),
                new Slider(0, SKColors.Black, _drawingConfig.InnerHorizontalSliderBarWidth, false, false, new LinearSliderBody(new SKPoint(_drawingConfig.InnerHorizontalSlidersX0, _drawingConfig.InnerHorizontalSlidersY2of3), new SKPoint(_drawingConfig.InnerHorizontalSlidersX1, _drawingConfig.InnerHorizontalSlidersY2of3))),
            };

            _drawingInput.CycleTimeSlider = new Slider(0, SKColors.Black, _drawingConfig.InnerHorizontalSliderBarWidth, false, false, new LinearSliderBody(new SKPoint(_drawingConfig.InnerHorizontalSlidersX0, _drawingConfig.InnerHorizontalSlidersY0of1), new SKPoint(_drawingConfig.InnerHorizontalSlidersX1, _drawingConfig.InnerHorizontalSlidersY0of1)));

            _photonLedControllerCommunicator = new FakePhotonLedControllerCommunicator();
            _photonLedControllerCommunicator.ColorTimePointsRead += _photonLedControllerCommunicator_ColorTimePointsRead;
            _photonLedControllerCommunicator.TimeProgressRead += _photonLedControllerCommunicator_TimeProgressRead;
            _photonLedControllerCommunicator.ReadColorTimePoints();
            _photonLedControllerCommunicator.ReadCycleTime();
            _photonLedControllerPollingService = new PhotonLedControllerPollingService(_photonLedControllerCommunicator);
            _photonLedControllerPollingService.Start();
        }

        public void Stop()
        {
            _photonLedControllerPollingService.Stop();
        }

        public void OnPaintSurface(SKImageInfo imageInfo, SKSurface surface)
        {
            var dSrv = new ColorTimeLineDrawingService();
            dSrv.Init(imageInfo, surface);
            dSrv.Draw(_drawingInput);
        }

        public void OnTouch(SKSize canvasSize, long touchId, SKPoint touchLocation, TouchAction touchAction)
        {
            var allSliders = new List<ISlider>();
            allSliders.AddRange(_drawingInput.ColorTimePointSliders.Select(ctps => ctps.Slider));
            allSliders.Add(_drawingInput.TimeProgressSlider.Slider);
            allSliders.AddRange(_drawingInput.ColorComponentSliders);
            allSliders.Add(_drawingInput.CycleTimeSlider);

            // Only single active touch allowed. Ignore all new touches if current touch hasn't been released.
            if (_currentTouchId.HasValue && touchId != _currentTouchId.Value)
            {
                return;
            }

            switch (touchAction)
            {
                case TouchAction.Pressed:
                    // Save current touch id.
                    _currentTouchId = touchId;

                    // Reset pressed and moved sliders.
                    _pressedSlider = null;
                    _movedSlider = null;

                    // Detect pressed slider.
                    foreach (var s in allSliders)
                    {
                        if (s.HitTest(Normalize(touchLocation, canvasSize)))
                        {
                            _pressedSlider = s;
                            break;
                        }
                    }

                    break;

                case TouchAction.Moved:
                    // If slider is pressed and touch moved remember pressed slider as moved slider.
                    if (_pressedSlider != null)
                    {
                        _movedSlider = _pressedSlider;
                    }
                    // Drag slider and use its position.
                    if (_movedSlider != null)
                    {
                        _movedSlider.Drag(Normalize(touchLocation, canvasSize));
                        var sliderValue = _movedSlider.Value;
                    }
                    break;

                case TouchAction.Released:
                    // Reset current touch id to allowe new touches to be handled.
                    _currentTouchId = null;

                    // If any slider is pressed and now is released withouth moving it between press and release then this slider is remembered as selected slider.
                    if (_pressedSlider != null && _movedSlider == null)
                    {
                        // Mark previously selected slider (if such exists) as unselected.
                        if (_selectedSlider != null)
                            _selectedSlider.IsSelected = false;

                        if (_selectedSlider != _pressedSlider)
                        {
                            // Mark pressed slider as selected and save it as selected slider.
                            _pressedSlider.IsSelected = true;
                            _selectedSlider = _pressedSlider;
                        }
                        else
                        {
                            // Null previously selected slider.
                            _selectedSlider = null;
                        }

                        if (_selectedSlider != null)
                        {
                            if (_movedSlider == _drawingInput.TimeProgressSlider.Slider)
                            {
                                _drawingInput.CycleTimeSlider.IsVisible = true;
                            }
                            else if (_drawingInput.ColorTimePointSliders.FirstOrDefault(ctps => ctps.Slider == _movedSlider) != null)
                            {
                                foreach(var colorComponentSlider in _drawingInput.ColorComponentSliders)
                                {
                                    colorComponentSlider.IsVisible = true;
                                }
                            }
                        }
                    }

                    break;
            }

            RefreshSurfaceRequested?.Invoke();
        }

        private SKPoint Normalize(SKPoint screenSpacePosition, SKSize canvasSize)
        {
            return new SKPoint(screenSpacePosition.X / canvasSize.Width, screenSpacePosition.Y / canvasSize.Width);
        }

        private void _photonLedControllerCommunicator_ColorTimePointsRead(object sender, EventArgs<ColorTimePoint[]> e)
        {
            // Update color time point sliders.

            var currentSliders = new List<ColorTimePointSlider>(_drawingInput.ColorTimePointSliders ?? new ColorTimePointSlider[0]).ToDictionary(ctps => ctps.Id);
            var updatedSliders = new List<ColorTimePointSlider>();
            foreach (var ctp in e.Data)
            {
                if (!currentSliders.TryGetValue(ctp.Id, out ColorTimePointSlider ctps))
                {
                    ctps = new ColorTimePointSlider(ctp.Id, new Slider(ctp.Time, Convert(ctp.Color), _drawingConfig.ColorsCircleWidth, true, false, new CircularSliderBody(_drawingConfig.Center, _drawingConfig.ColorsCircleRadius)), _drawingInput.ColorComponentSliders, _photonLedControllerCommunicator);
                }
                else
                {
                    ctps.Slider.Color = Convert(ctp.Color);
                    ctps.Slider.Value = ctp.Time;
                }
                updatedSliders.Add(ctps);
            }

            _drawingInput.ColorTimePointSliders = updatedSliders.ToArray();

            // Update time progress slider.

            if (_drawingInput.TimeProgressSlider != null)
            {
                _drawingInput.TimeProgressSlider.Slider.Color = CurrentColor();
            }

            // Refresh surface.

            RefreshSurfaceRequested?.Invoke();
        }

        private void _photonLedControllerCommunicator_TimeProgressRead(object sender, EventArgs<float> e)
        {
            _timeProgress = e.Data;

            // Update time progress slider.

            if (_drawingInput.TimeProgressSlider == null)
            {
                _drawingInput.TimeProgressSlider = new TimeProgressSlider(new Slider(_timeProgress, CurrentColor(), _drawingConfig.ProgressCircleWidth, true, false, new CircularSliderBody(_drawingConfig.Center, _drawingConfig.ProgressCircleRadius)), _drawingInput.CycleTimeSlider, _photonLedControllerCommunicator);
            }
            else
            {
                _drawingInput.TimeProgressSlider.Slider.Color = CurrentColor();
                _drawingInput.TimeProgressSlider.Slider.Value = _timeProgress;
            }

            // Refresh surface.

            RefreshSurfaceRequested?.Invoke();
        }

        private SKColor CurrentColor()
        {
            return new ColorTimeLine(_drawingInput.ColorTimePointSliders.Select(ctps => ctps.Slider).ToArray()).ColorAt(_timeProgress);
        }

        private SKColor Convert(ColorTimePointColor color)
        {
            return new SKColor(color.R, color.G, color.B);
        }
    }
}
