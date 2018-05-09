using LedController3Client.Ui.Core;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace LedController3Client.Ui
{
    public class Poly
    {
        public SKColor Color { get; set; }
        public List<SKPoint> Points { get; set; } = new List<SKPoint>();
    }

    public class HvColorPicker : Component, IColorPicker, IDrawerComponent
    {
        private readonly ColorTimeLineDrawingConfig _drawingConfig;
        private readonly ColorTimeLineComponentsDimensionsConfig _worldDimensions;

        private SKBitmap _backgroundBitmap;

        public HvColorPicker(ColorTimeLineDrawingConfig drawingConfig)
        {
            _drawingConfig = drawingConfig;
            _worldDimensions = _drawingConfig.WorldDimensions();
        }

        public event EventHandler<EventArgs<SKColor>> ColorChanged;

        public void ResetColor(SKColor color)
        {

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
            var baseHueColors = new[] { SKColors.Red, SKColors.Green, SKColors.Blue, SKColors.Red, SKColors.Red };

            var fullCirlce = (float)Math.PI * 2f;
            var fullRadius = .8f * _worldDimensions.ProgressCircleRadius;

            const int cirRes = 180;
            const int radRes = 100;

            var alphaDelta = fullCirlce / cirRes;
            var radiusDelta = fullRadius / radRes;

            var sin0 = 0f;
            var cos0 = 1f;

            for (int aIx = 1; aIx <= cirRes; ++aIx)
            {
                var alpha = aIx * alphaDelta;

                var sin1 = (float)Math.Sin(alpha);
                var cos1 = (float)Math.Cos(alpha);

                var rad0 = 0f;

                for (int rIx = 1; rIx < radRes; ++rIx)
                {
                    var rad1 = rIx * radiusDelta;

                    var p0 = new SKPoint(cos0 * rad0, sin0 * rad0) + _worldDimensions.Center;
                    var p1 = new SKPoint(cos1 * rad0, sin1 * rad0) + _worldDimensions.Center;
                    var p2 = new SKPoint(cos1 * rad1, sin1 * rad1) + _worldDimensions.Center;
                    var p3 = new SKPoint(cos0 * rad1, sin0 * rad1) + _worldDimensions.Center;

                    var alphaRatio = alpha / fullCirlce;
                    var alphaColorStep = (int)Math.Floor(alphaRatio * 3);
                    SKColor c0 = baseHueColors[alphaColorStep];
                    SKColor c1 = baseHueColors[alphaColorStep + 1];
                    if (alphaRatio >= 2 / 3f)
                        alphaRatio -= 2 / 3f;
                    else if (alphaRatio >= 1 / 3f)
                        alphaRatio -= 1 / 3f;
                    alphaRatio *= 3f;

                    new ColorsInterpolator().InterpolateColors(c0, c1, alphaRatio, out SKColor c);

                    SKColor valueColor;
                    var radiusRatio = rad1 / fullRadius;
                    if (radiusRatio < .5f)
                    {
                        valueColor = SKColors.White;
                        radiusRatio = .5f - radiusRatio;
                        radiusRatio /= .5f;
                    }
                    else if (radiusRatio > .5f)
                    {
                        valueColor = SKColors.Black;
                        radiusRatio -= .5f;
                        radiusRatio /= .5f;
                    }
                    else
                    {
                        valueColor = SKColor.Empty;
                        radiusRatio = 0f;
                    }

                    //radiusRatio *= radiusRatio;

                    new ColorsInterpolator().InterpolateColors(c, valueColor, radiusRatio, out c);

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

                sin0 = sin1;
                cos0 = cos1;
            }
        }
    }
}
