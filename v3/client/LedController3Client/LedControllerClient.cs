using LedController3Client.Communication;
using LedController3Client.Ui;
using SkiaSharp;
using System;
using LedController3Client.Ui.Core;

namespace LedController3Client
{
    public class LedControllerClient
    {
        private IPhotonLedControllerCommunicator _photonLedControllerCommunicator;
        private IPhotonLedControllerPollingService _photonLedControllerPollingService;

        private static readonly object _sync = new object();
        private readonly Engine _engine;
        private readonly DrawerService _drawerService;
        private readonly TouchHandlerService _touchHandlerService;
        private RootSurfaceComponent _rootSurface;

        public LedControllerClient()
        {
            _engine = new Engine();
            _engine.RefreshSurfaceRequested += _engine_RefreshSurfaceRequested;
            _drawerService = new DrawerService();
            _touchHandlerService = new TouchHandlerService();
        }

        public event Action RefreshSurfaceRequested;

        public void Start()
        {
            _photonLedControllerCommunicator = new FakePhotonLedControllerCommunicator();
            _photonLedControllerCommunicator.CycleTimeRead += _photonLedControllerCommunicator_CycleTimeRead;
            _photonLedControllerCommunicator.TimeProgressRead += _photonLedControllerCommunicator_TimeProgressRead;
            _photonLedControllerCommunicator.ColorTimePointsRead += _photonLedControllerCommunicator_ColorTimePointsRead;
            _photonLedControllerCommunicator.Start();

            _rootSurface = new RootSurfaceComponent(_photonLedControllerCommunicator);

            _photonLedControllerCommunicator.ReadCycleTime();
            _photonLedControllerCommunicator.ReadColorTimePoints();
            _photonLedControllerPollingService = new PhotonLedControllerPollingService(_photonLedControllerCommunicator);
            _photonLedControllerPollingService.Start();

            _engine.Start();
        }

        public void Stop()
        {
            _engine.Stop();
            _photonLedControllerPollingService.Stop();
        }

        private void _engine_RefreshSurfaceRequested()
        {
            RefreshSurfaceRequested?.Invoke();
        }

        public void OnPaintSurface(SKImageInfo imageInfo, SKSurface surface)
        {
            lock (_sync)
            {
                _drawerService.Draw(_rootSurface, surface.Canvas, imageInfo.Width);
            }
        }

        public void OnTouch(SKSize canvasSize, long touchId, SKPoint touchLocation, TouchAction touchAction)
        {
            lock (_sync)
            {
                _touchHandlerService.Handle(_rootSurface, touchId, Normalize(touchLocation, canvasSize), touchAction);
            }
        }

        private void _photonLedControllerCommunicator_CycleTimeRead(object sender, EventArgs<int> e)
        {
            lock (_sync)
            {
                _rootSurface.UpdateCycleTime(e.Data);
            }
        }

        private void _photonLedControllerCommunicator_TimeProgressRead(object sender, EventArgs<float> e)
        {
            lock (_sync)
            {
                _rootSurface.UpdateTimeProgress(e.Data);
            }
        }

        private void _photonLedControllerCommunicator_ColorTimePointsRead(object sender, EventArgs<ColorTimePoint[]> e)
        {
            lock (_sync)
            {
                _rootSurface.UpdateColorTimePointSliders(e.Data);
            }
        }

        private SKPoint Normalize(SKPoint screenSpacePosition, SKSize canvasSize)
        {
            return new SKPoint(screenSpacePosition.X / canvasSize.Width, screenSpacePosition.Y / canvasSize.Width);
        }
    }
}
