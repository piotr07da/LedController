using LedController3Client.Mobile.ColorTimeLineDrawing;
using LedController3Client.Mobile.PhotonLedController;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Color = LedController3Client.Mobile.PhotonLedController.Color;

namespace LedController3Client.Mobile
{
    public partial class MainPage : ContentPage
    {
        private IPhotonLedControllerCommunicator _photonLedControllerCommunicator;
        private IPhotonLedControllerPollingService _photonLedControllerPollingService;

        private ColorTimePoint[] _colorTimePoints;
        private float _timeProgress;

        private ColorTimeLineDrawingConfig _drawingConfig;
        private ColorTimeLineDrawingInput _drawingInput;

        public MainPage()
        {
            InitializeComponent();

            _photonLedControllerCommunicator = new FakePhotonLedControllerCommunicator();
            _photonLedControllerCommunicator.ColorTimePointsRead += _photonLedControllerCommunicator_ColorTimePointsRead;
            _photonLedControllerCommunicator.TimeProgressRead += _photonLedControllerCommunicator_TimeProgressRead;
            _photonLedControllerCommunicator.ReadColorTimePoints();
            _photonLedControllerCommunicator.ReadCycleTime();
            _photonLedControllerPollingService = new PhotonLedControllerPollingService(_photonLedControllerCommunicator);
            _photonLedControllerPollingService.Start();
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
            _drawingConfig = new ColorTimeLineDrawingConfig(args.Info.Width);
            _drawingInput = new ColorTimeLineDrawingInput();
            _drawingInput.Gradient = new ColorTimeLineGradient(_colorTimePoints.Select(ctp => Convert(ctp.Color)).ToArray(), _colorTimePoints.Select(ctp => ctp.Time).ToArray());
            _drawingInput.ColorTimePointSliders = _colorTimePoints.Where(ctp => !ctp.IsWeldingPoint).Select(ctp => new ColorTimeLineSlider(Convert(ctp.Color), ctp.Time, _drawingConfig.SizeDiv2, _drawingConfig.SizeDiv2, _drawingConfig.ColorsCircleRadius, _drawingConfig.ColorsCircleWidth)).ToArray();
            _drawingInput.TimeProgressSlider = new ColorTimeLineSlider(CurrentColor(), _timeProgress, _drawingConfig.SizeDiv2, _drawingConfig.SizeDiv2, _drawingConfig.ProgressCircleRadius, _drawingConfig.ProgressCircleWidth);

            var dSrv = new ColorTimeLineDrawingService();
            dSrv.Init(args.Info, args.Surface, _drawingConfig);
            dSrv.Draw(_drawingInput);
        }

        private void OnTouch(object sender, SKTouchEventArgs e)
        {
            
        }

        private void _photonLedControllerCommunicator_ColorTimePointsRead(object sender, EventArgs<ColorTimePoint[]> e)
        {
            _colorTimePoints = e.Data;
            Device.BeginInvokeOnMainThread(() => CanvasView.InvalidateSurface());
        }

        private void _photonLedControllerCommunicator_TimeProgressRead(object sender, EventArgs<float> e)
        {
            _timeProgress = e.Data;
            Device.BeginInvokeOnMainThread(() => CanvasView.InvalidateSurface());
        }



        // -------------

        private SKColor CurrentColor()
        {
            var ctps = _colorTimePoints;
            var progress = _timeProgress;
            var pointCount = ctps != null ? ctps.Length : 0;

            if (pointCount == 0)
            {
                return SKColors.Black;
            }

            if (pointCount == 1)
            {
                return Convert(ctps[0].Color);
            }

            var lctp = ctps[0];
            var rctp = ctps[ctps.Length - 1];

            for (var i = 1; i < pointCount - 1; ++i)
            {
                var ctp = ctps[i];
                var ctpTime = ctp.Time;

                if (ctpTime <= progress && ctpTime > lctp.Time)
                    lctp = ctp;
                if (ctpTime >= progress && ctpTime < rctp.Time)
                    rctp = ctp;
            }

            var ratio = InverseLerp(lctp.Time, rctp.Time, progress);

            InterpolateColors(Convert(lctp.Color), Convert(rctp.Color), ratio, out SKColor outColor);
            return outColor;
        }

        private float InverseLerp(float lValue, float rValue, float value)
        {
            var progress = value - lValue;
            var range = rValue - lValue;
            if (range > 0)
            {
                return progress / range;
            }
            return .5f;
        }

        private void InterpolateColors(SKColor lColor, SKColor rColor, float ratio, out SKColor outColor)
        {

            InterpolateColorsComponents(lColor.Red, rColor.Red, ratio, out byte r);
            InterpolateColorsComponents(lColor.Green, rColor.Green, ratio, out byte g);
            InterpolateColorsComponents(lColor.Blue, rColor.Blue, ratio, out byte b);
            outColor = new SKColor(r, g, b);
        }

        private void InterpolateColorsComponents(byte lColorComponent, byte rColorComponent, float ratio, out byte outColorComponent)
        {
            outColorComponent = (byte)(lColorComponent * (1f - ratio) + rColorComponent * ratio);
        }

        // ------------


        private SKColor Convert(Color color)
        {
            return new SKColor(color.R, color.G, color.B);
        }
    }
}
