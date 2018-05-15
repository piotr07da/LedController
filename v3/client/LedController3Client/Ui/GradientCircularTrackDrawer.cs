using LedController3Client.Ui.Core;
using SkiaSharp;
using System.Linq;
using System.Collections.Generic;

namespace LedController3Client.Ui
{
    public class GradientCircularTrackDrawer : Component, IDrawerComponent
    {
        private readonly SKPoint _center;
        private readonly float _radius;
        private readonly float _thickness;
        private readonly int _circularResolution;
        private readonly ColorPositions _colorPositions;
        private SKPoint[] _vertices;

        public GradientCircularTrackDrawer(SKPoint center, float radius, float thickness, int circularResolution, ColorPositions colorPositions)
        {
            _center = center;
            _radius = radius;
            _thickness = thickness;
            _circularResolution = circularResolution;
            _colorPositions = colorPositions;
        }

        public void Draw(SKCanvas canvas, float scale)
        {
            var vs = Vertices().Select(v => new SKPoint(v.X * scale, v.Y * scale));

            var cpl = new ColorPositionLine(_colorPositions.AsArray());
            var ci = new CircularIterator(_circularResolution, false);
            var cs = new List<SKColor>();
            while (ci.Next())
            {
                var c0 = cpl.ColorAt(ci.Alpha0Ratio);
                cs.Add(c0);
                cs.Add(c0);
            }

            canvas.DrawVertices(SKVertexMode.TriangleStrip, vs.ToArray(), cs.ToArray(), new SKPaint());
        }

        private SKPoint[] Vertices()
        {
            if (_vertices == null)
            {
                var ci = new CircularIterator(_circularResolution, true);
                var vs = new List<SKPoint>();
                var r0 = _radius - _thickness / 2f;
                var r1 = _radius + _thickness / 2f;
                while (ci.Next())
                {
                    var v0 = _center + new SKPoint(ci.Cos0 * r0, ci.Sin0 * r0);
                    var v1 = _center + new SKPoint(ci.Cos0 * r1, ci.Sin0 * r1);

                    vs.Add(v0);
                    vs.Add(v1);
                }
                _vertices = vs.ToArray();
            }
            return _vertices;
        }
    }
}
