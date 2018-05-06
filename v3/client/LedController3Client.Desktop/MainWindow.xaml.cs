using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SkiaSharp.Views.WPF;

namespace LedController3Client.Desktop
{
    public partial class MainWindow : Window
    {
        private LedControllerClient _client;
        private long? _touchId;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _client = new LedControllerClient();
            _client.RefreshSurfaceRequested += _client_RefreshSurfaceRequested;
            _client.Start();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            _client.Stop();
        }

        private void _client_RefreshSurfaceRequested()
        {
            Dispatcher.BeginInvoke(new Action(() => SkElement.InvalidateVisual()));
        }

        private void SKElement_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            _client.OnPaintSurface(e.Info, e.Surface);
        }

        private void SkElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _touchId = 0;
            OnTouch(e, TouchAction.Pressed);
        }

        private void SkElement_MouseMove(object sender, MouseEventArgs e)
        {
            if (_touchId.HasValue)
                OnTouch(e, TouchAction.Moved);
        }

        private void SkElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OnTouch(e, TouchAction.Released);
            _touchId = null;
        }

        private void OnTouch(MouseEventArgs e, TouchAction touchAction)
        {
            _client.OnTouch(SkElement.CanvasSize, _touchId.Value, e.GetPosition(SkElement).ToSKPoint(), touchAction);
        }
    }
}
