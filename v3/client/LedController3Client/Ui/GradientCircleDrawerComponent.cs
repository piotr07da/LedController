using LedController3Client.Ui.Core;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace LedController3Client.Ui
{
    public class GradientCircleDrawerComponent : Component, IDrawerComponent
    {
        
        private readonly ColorTimeLineDrawingConfig _drawingConfig;
        private readonly IEnumerable<ColorTimePointSlider> _colorTimePointSliders;

        public GradientCircleDrawerComponent(ColorTimeLineDrawingConfig drawingConfig, IEnumerable<ColorTimePointSlider> colorTimePointSliders)
        {
            _drawingConfig = drawingConfig;
            _colorTimePointSliders = colorTimePointSliders;
        }

        public void Draw(SKCanvas canvas, float scale)
        {
            // TODO - regenerować vertexy i kolory tylko wtedy gdy mieniają się slidery (ich liczba, ich pozycje, ich kolory)

            var screenDimensions = _drawingConfig.ScreenDimensions(scale);

            var ctl = new ColorTimeLine(_colorTimePointSliders.Select(ctps => ctps.Slider).ToArray());

            var ci = new CircularIterator(360);

            var r0 = screenDimensions.GradientCircleRadius - screenDimensions.GradientCircleWidth / 2f;
            var r1 = screenDimensions.GradientCircleRadius + screenDimensions.GradientCircleWidth / 2f;

            var vs = new List<SKPoint>();
            var cs = new List<SKColor>();

            while (ci.Next())
            {
                var v0 = screenDimensions.Center + new SKPoint(ci.Cos0 * r0, ci.Sin0 * r0);
                var v1 = screenDimensions.Center + new SKPoint(ci.Cos0 * r1, ci.Sin0 * r1);
                var v2 = screenDimensions.Center + new SKPoint(ci.Cos1 * r0, ci.Sin1 * r0);
                var v3 = screenDimensions.Center + new SKPoint(ci.Cos1 * r1, ci.Sin1 * r1);

                var c0 = ctl.ColorAt(ci.Alpha0Ratio);
                var c1 = ctl.ColorAt(ci.Alpha1Ratio);

                vs.Add(v0); cs.Add(c0);
                vs.Add(v1); cs.Add(c0);
                vs.Add(v2); cs.Add(c1);
                vs.Add(v3); cs.Add(c1);
            }

            canvas.DrawVertices(SKVertexMode.TriangleStrip, vs.ToArray(), cs.ToArray(), new SKPaint() { IsAntialias = true });
        }
    }
}
