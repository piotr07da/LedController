using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace LedController3Client.Mobile
{
    public partial class MainPage : ContentPage
    {
        private LedControllerClient _client;

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

            _client = new LedControllerClient();
            _client.RefreshSurfaceRequested += _client_RefreshSurfaceRequested;
            _client.Start();
        }

        private void _client_RefreshSurfaceRequested()
        {
            Device.BeginInvokeOnMainThread(() => CanvasView.InvalidateSurface());
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

            _client.Stop();
        }

        private void OnSkiaCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            _client.OnPaintSurface(args.Info, args.Surface);
        }

        private void OnTouch(object sender, SKTouchEventArgs e)
        {
            TouchAction touchAction = TouchAction.None;
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    touchAction = TouchAction.Pressed;
                    break;
                case SKTouchAction.Released:
                    touchAction = TouchAction.Released;
                    break;
                case SKTouchAction.Moved:
                    touchAction = TouchAction.Moved;
                    break;
            }

            _client.OnTouch(CanvasView.CanvasSize, e.Id, e.Location, touchAction);

            e.Handled = true;
        }
    }
}
