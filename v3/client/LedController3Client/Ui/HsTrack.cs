using LedController3Client.Ui.Core;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LedController3Client.Ui
{
    public class HsTrack : Component, IDrawerComponent
    {
        private readonly ColorTimeLineDrawingConfig _drawingConfig;
        private readonly ColorTimeLineComponentsDimensionsConfig _worldDimensions;
        private readonly HsvRgbConverter _hsvRgb = new HsvRgbConverter();

        private SKBitmap _backgroundBitmap;
        private SKColor _brightnessColor;

        public HsTrack(ColorTimeLineDrawingConfig drawingConfig)
        {
            _drawingConfig = drawingConfig;
            _worldDimensions = _drawingConfig.WorldDimensions();
            _brightnessColor = SKColors.Black;
        }

        public void Draw(SKCanvas canvas, float scale)
        {
            if (_backgroundBitmap == null)
            {
                _backgroundBitmap = new SKBitmap((int)scale, (int)scale);
                var backgroundCanvas = new SKCanvas(_backgroundBitmap);

                var vertices = new List<SKPoint>();
                var colors = new List<SKColor>();
                var indices = new List<ushort>();
                ushort polyIx = 0;
                foreach (var poly in BackgroundPolys())
                {
                    vertices.AddRange(poly.Points.Select(p => new SKPoint(p.X * scale, p.Y * scale)));
                    colors.AddRange(poly.Colors);
                    var offsetIx = polyIx * 4;
                    indices.Add((ushort)(offsetIx + 0));
                    indices.Add((ushort)(offsetIx + 1));
                    indices.Add((ushort)(offsetIx + 2));
                    indices.Add((ushort)(offsetIx + 0));
                    indices.Add((ushort)(offsetIx + 2));
                    indices.Add((ushort)(offsetIx + 3));
                    ++polyIx;
                }
                backgroundCanvas.DrawVertices(SKVertexMode.Triangles, vertices.ToArray(), null, colors.ToArray(), indices.ToArray(), new SKPaint() { });
                backgroundCanvas.Flush();
            }

            var screenDimensions = _drawingConfig.ScreenDimensions(scale);

            canvas.DrawBitmap(_backgroundBitmap, new SKPoint(0, 0), new SKPaint() { });
        }

        public void UpdateBrightness(float brightness)
        {
            var cc = (byte)(255 * brightness);
            _brightnessColor = new SKColor(cc, cc, cc);
        }

        private IEnumerable<Poly> BackgroundPolys()
        {
            var fullCirlce = (float)Math.PI * 2f;
            var fullRadius0 = 0f;
            var fullRadius1 = _worldDimensions.HsvColorPickerHueSaturationCircleRadius;
            var fullRadiusRange = fullRadius1 - fullRadius0;

            const int cirRes = 36;
            const int radRes = 20;

            var alphaDelta = fullCirlce / cirRes;
            var radiusDelta = fullRadiusRange / radRes;

            var ci = new CircularIterator(cirRes, true);

            while (ci.Next() && ci.Alpha1Ratio <= 1.00000001f)
            {
                var rad0 = fullRadius0;

                for (int rIx = 1; rIx <= radRes; ++rIx)
                {
                    var rad1 = fullRadius0 + rIx * radiusDelta;

                    var p0 = new SKPoint(ci.Cos0 * rad0, ci.Sin0 * rad0) + _worldDimensions.Center;
                    var p1 = new SKPoint(ci.Cos1 * rad0, ci.Sin1 * rad0) + _worldDimensions.Center;
                    var p2 = new SKPoint(ci.Cos1 * rad1, ci.Sin1 * rad1) + _worldDimensions.Center;
                    var p3 = new SKPoint(ci.Cos0 * rad1, ci.Sin0 * rad1) + _worldDimensions.Center;

                    _hsvRgb.Hsv2Rgb(ci.Alpha0 / fullCirlce, (rad0 - fullRadius0) / fullRadiusRange, 1f, out SKColor c0);
                    _hsvRgb.Hsv2Rgb(ci.Alpha1 / fullCirlce, (rad0 - fullRadius0) / fullRadiusRange, 1f, out SKColor c1);
                    _hsvRgb.Hsv2Rgb(ci.Alpha1 / fullCirlce, (rad1 - fullRadius0) / fullRadiusRange, 1f, out SKColor c2);
                    _hsvRgb.Hsv2Rgb(ci.Alpha0 / fullCirlce, (rad1 - fullRadius0) / fullRadiusRange, 1f, out SKColor c3);

                    var poly = new Poly();

                    poly.Points.Add(p0);
                    poly.Points.Add(p1);
                    poly.Points.Add(p2);
                    poly.Points.Add(p3);

                    poly.Colors.Add(c0);
                    poly.Colors.Add(c1);
                    poly.Colors.Add(c2);
                    poly.Colors.Add(c3);

                    yield return poly;

                    rad0 = rad1;
                }
            }
        }

        private class Poly
        {
            public List<SKColor> Colors { get; set; } = new List<SKColor>();
            public List<SKPoint> Points { get; set; } = new List<SKPoint>();
        }
    }
}
