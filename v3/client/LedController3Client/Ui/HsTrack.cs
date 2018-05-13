using LedController3Client.Ui.Core;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace LedController3Client.Ui
{
    public class HsTrack : Component, IDrawerComponent
    {
        private readonly ColorTimeLineDrawingConfig _drawingConfig;
        private readonly ColorTimeLineComponentsDimensionsConfig _worldDimensions;

        private SKBitmap _backgroundBitmap;

        public HsTrack(ColorTimeLineDrawingConfig drawingConfig)
        {
            _drawingConfig = drawingConfig;
            _worldDimensions = _drawingConfig.WorldDimensions();
        }

        public void Draw(SKCanvas canvas, float scale)
        {
            if (_backgroundBitmap == null)
            {
                _backgroundBitmap = new SKBitmap((int)scale, (int)scale);
                var backgroundCanvas = new SKCanvas(_backgroundBitmap);
                foreach (var poly in BackgroundPolys())
                {
                    var path = new SKPath();
                    path.MoveTo(poly.Points[0].Multiply(scale));
                    for (var i = 1; i < poly.Points.Count; ++i)
                        path.LineTo(poly.Points[i].Multiply(scale));
                    path.Close();

                    var shader = SKShader.CreateColor(poly.Color);
                    var paint = new SKPaint() { Shader = shader, Style = SKPaintStyle.StrokeAndFill, StrokeWidth = 1, IsAntialias = true };
                    backgroundCanvas.DrawPath(path, paint);
                }
            }

            canvas.DrawBitmap(_backgroundBitmap, new SKPoint(0, 0));
        }

        private IEnumerable<Poly> BackgroundPolys()
        {
            var fullCirlce = (float)Math.PI * 2f;
            var fullRadius = _worldDimensions.HsvColorPickerHueSaturationCircleRadius;

            const int cirRes = 180;
            const int radRes = 100;

            var alphaDelta = fullCirlce / cirRes;
            var radiusDelta = fullRadius / radRes;

            var ci = new CircularIterator(cirRes);

            while (ci.Next())
            {
                var rad0 = 0f;

                for (int rIx = 1; rIx < radRes; ++rIx)
                {
                    var rad1 = rIx * radiusDelta;

                    var p0 = new SKPoint(ci.Cos0 * rad0, ci.Sin0 * rad0) + _worldDimensions.Center;
                    var p1 = new SKPoint(ci.Cos1 * rad0, ci.Sin1 * rad0) + _worldDimensions.Center;
                    var p2 = new SKPoint(ci.Cos1 * rad1, ci.Sin1 * rad1) + _worldDimensions.Center;
                    var p3 = new SKPoint(ci.Cos0 * rad1, ci.Sin0 * rad1) + _worldDimensions.Center;

                    new HsvRgbConverter().Hsv2Rgb(ci.Alpha1 / fullCirlce, rad1 / fullRadius, 1f, out SKColor c);

                    var poly = new Poly();
                    poly.Points.Add(p0);
                    if (rad0 > 0)
                        poly.Points.Add(p1);
                    poly.Points.Add(p2);
                    poly.Points.Add(p3);
                    poly.Color = c;

                    yield return poly;

                    rad0 = rad1;
                }
            }
        }

        private class Poly
        {
            public SKColor Color { get; set; }
            public List<SKPoint> Points { get; set; } = new List<SKPoint>();
        }
    }
}
