using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LedController3Client.Ui.Core
{
    public class Engine
    {
        private const int Fps = 30;
        private const int DrawingThreadSleep = 1000 / Fps;

        private readonly SynchronizationContext _syncContext;

        private Thread _drawingThread;
        private bool _run;

        public Engine()
        {
            _syncContext = SynchronizationContext.Current;
        }

        public event Action RefreshSurfaceRequested;

        public void Start()
        {
            _run = true;
            _drawingThread = new Thread(new ThreadStart(StartMainLoop));
            _drawingThread.Start();
        }

        public void Stop()
        {
            _run = false;
        }

        private void StartMainLoop()
        {
            while (_run)
            {
                _syncContext.Send(p =>
                {
                    RefreshSurfaceRequested?.Invoke();
                }, this);
                Thread.Sleep(DrawingThreadSleep);
            }
        }
    }
}
