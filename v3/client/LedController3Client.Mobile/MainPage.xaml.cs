using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LedController3Client.Mobile
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnSkiaCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            float strokeWidth = 50;
            float xRadius = (info.Width - strokeWidth) / 2;
            float yRadius = (info.Height - strokeWidth) / 2;

            var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Blue,
                StrokeWidth = strokeWidth
            };

            canvas.DrawOval(info.Width / 2, info.Height / 2, xRadius, yRadius, paint);
        }

    }
}
