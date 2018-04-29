using LedController3Client.Mobile.ColorTimeLineDrawing;
using LedController3Client.Mobile.Mathematics;
using LedController3Client.Mobile.PhotonLedController;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using ColorTimePointColor = LedController3Client.Mobile.PhotonLedController.ColorTimePointColor;

namespace LedController3Client.Mobile
{
    public partial class MainPage : ContentPage
    {
        private IPhotonLedControllerCommunicator _photonLedControllerCommunicator;
        private IPhotonLedControllerPollingService _photonLedControllerPollingService;

        private float _timeProgress;

        private ColorTimeLineDrawingConfig _drawingConfig;
        private ColorTimeLineDrawingInput _drawingInput;

        private long? _currentTouchId; 
        private ColorTimeLineSlider _pressedSlider;
        private ColorTimeLineSlider _movedSlider;
        private ColorTimeLineSlider _selectedSlider;

        private double _pageWidth;
        public double PageWidth
        {
            get { return _pageWidth;  }
            set
            {
                _pageWidth = value;
                OnPropertyChanged(nameof(PageWidth));
            }
        }

        public MainPage()
        {
            InitializeComponent();

            _drawingConfig = new ColorTimeLineDrawingConfig(1f);
            _drawingInput = new ColorTimeLineDrawingInput();

            _photonLedControllerCommunicator = new FakePhotonLedControllerCommunicator();
            _photonLedControllerCommunicator.ColorTimePointsRead += _photonLedControllerCommunicator_ColorTimePointsRead;
            _photonLedControllerCommunicator.TimeProgressRead += _photonLedControllerCommunicator_TimeProgressRead;
            _photonLedControllerCommunicator.ReadColorTimePoints();
            _photonLedControllerCommunicator.ReadCycleTime();
            _photonLedControllerPollingService = new PhotonLedControllerPollingService(_photonLedControllerCommunicator);
            _photonLedControllerPollingService.Start();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            PageWidth = width;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _photonLedControllerPollingService.Stop();
        }

        private void OnSkiaCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            var dSrv = new ColorTimeLineDrawingService();
            dSrv.Init(args.Info, args.Surface);
            dSrv.Draw(_drawingInput);
        }

        private void OnTouch(object sender, SKTouchEventArgs e)
        {
            var allSliders = _drawingInput.ColorTimePointSliders.Select(ctps => ctps.Slider).ToList();
            allSliders.Add(_drawingInput.TimeProgressSlider.Slider);
            
            // Only single active touch allowed. Ignore all new touches if current touch hasn't been released.
            if (_currentTouchId.HasValue && e.Id != _currentTouchId.Value)
            {
                return;
            }

            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    // Save current touch id.
                    _currentTouchId = e.Id;

                    // Reset pressed and moved sliders.
                    _pressedSlider = null;
                    _movedSlider = null;

                    // Detect pressed slider.
                    var normalizedLocationX = e.Location.X / CanvasView.CanvasSize.Width - _drawingConfig.Center;
                    var normalizedLocationY = e.Location.Y / CanvasView.CanvasSize.Width - _drawingConfig.Center;
                    foreach (var s in allSliders)
                    {
                        if (s.HitTest(normalizedLocationX, normalizedLocationY))
                        {
                            _pressedSlider = s;
                            break;
                        }
                    }

                    break;

                case SKTouchAction.Moved:
                    // If slider is pressed and touch moved remember pressed slider as moved slider.
                    if (_pressedSlider != null)
                    {
                        _movedSlider = _pressedSlider;
                    }
                    // Recalculate and set new slider position.
                    if (_movedSlider != null)
                    {
                        var touchPosition = new Vector(e.Location.X, e.Location.Y);
                        var orbitCenter = new Vector(CanvasView.CanvasSize.Width * _drawingConfig.Center, CanvasView.CanvasSize.Width * _drawingConfig.Center);
                        var touchVector = touchPosition - orbitCenter;
                        touchVector.Normalize();
                        var angle = touchVector.AngleFrom(new Vector(1, 0));
                        var fullCircleAngle = 2 * (float)Math.PI;
                        if (angle < 0)
                            angle = fullCircleAngle + angle;
                        var time = angle / fullCircleAngle;

                        _movedSlider.UpdateTime(time);

                        if (_drawingInput.TimeProgressSlider != null && _drawingInput.TimeProgressSlider.Slider == _movedSlider)
                        {
                            _photonLedControllerCommunicator.WriteTimeProgress(time);
                            _photonLedControllerCommunicator.ReadTimeProgress();
                        }
                        else if (_drawingInput.ColorTimePointSliders != null)
                        {
                            var ctpSlider = _drawingInput.ColorTimePointSliders.First(ctps => ctps.Slider == _movedSlider);

                            _photonLedControllerCommunicator.WriteColorTimePointTime(ctpSlider.Id, time);
                            _photonLedControllerCommunicator.ReadColorTimePoints();
                        }
                    }
                    break;

                case SKTouchAction.Released:
                    // Reset current touch id to allowe new touches to be handled.
                    _currentTouchId = null;

                    // If any slider is pressed and now is released withouth moving it between press and release then this slider is remembered as selected slider.
                    if (_pressedSlider != null && _movedSlider == null)
                    {
                        if (_selectedSlider != _pressedSlider)
                            _selectedSlider = _pressedSlider;
                        else
                            _selectedSlider = null;

                        _drawingInput.SelectedSlider = _selectedSlider;
                    }
                    
                    break;
            }

            e.Handled = true;

            Device.BeginInvokeOnMainThread(() => CanvasView.InvalidateSurface());
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
                    ctps = new ColorTimePointSlider() { Id = ctp.Id, Slider = new ColorTimeLineSlider(Convert(ctp.Color), ctp.Time, _drawingConfig.ColorsCircleRadius, _drawingConfig.ColorsCircleWidth) };
                }
                else
                {
                    ctps.Slider.UpdateColor(Convert(ctp.Color));
                    ctps.Slider.UpdateTime(ctp.Time);
                }
                updatedSliders.Add(ctps);
            }

            _drawingInput.ColorTimePointSliders = updatedSliders.ToArray();

            // Update time progress slider.

            if (_drawingInput.TimeProgressSlider != null)
            {
                _drawingInput.TimeProgressSlider.Slider.UpdateColor(CurrentColor());
            }

            // Invalidate surface.

            Device.BeginInvokeOnMainThread(() => CanvasView.InvalidateSurface());
        }

        private void _photonLedControllerCommunicator_TimeProgressRead(object sender, EventArgs<float> e)
        {
            _timeProgress = e.Data;

            // Update time progress slider.

            if (_drawingInput.TimeProgressSlider == null)
            {
                _drawingInput.TimeProgressSlider = new TimeProgressSlider() { Slider = new ColorTimeLineSlider(CurrentColor(), _timeProgress, _drawingConfig.ProgressCircleRadius, _drawingConfig.ProgressCircleWidth) };
            }
            else
            {
                _drawingInput.TimeProgressSlider.Slider.UpdateColor(CurrentColor());
                _drawingInput.TimeProgressSlider.Slider.UpdateTime(_timeProgress);
            }

            // Invalidate surface.

            Device.BeginInvokeOnMainThread(() => CanvasView.InvalidateSurface());
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
