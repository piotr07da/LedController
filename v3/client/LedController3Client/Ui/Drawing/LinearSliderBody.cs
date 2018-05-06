using LedController3Client.Mathematics;
using SkiaSharp;
using System;

namespace LedController3Client.Ui.Drawing
{
    public class LinearSliderBody : ISliderBody
    {
        private readonly SKPoint _p0;
        private readonly SKPoint _p1;

        public LinearSliderBody(SKPoint p0, SKPoint p1)
        {
            _p0 = p0;
            _p1 = p1;
        }

        public float PositionToValue(SKPoint position)
        {
            var dragPointVec = Convert(position);
            dragPointVec.ClosestPointOnSegment(Convert(_p0), Convert(_p1), out float ratio);
            return ratio;
        }

        public SKPoint ValueToPosition(float value)
        {
            var angle = 2.0 * Math.PI * value;
            var x = _p0.X + (_p1.X - _p0.X) * value;
            var y = _p0.Y + (_p1.Y - _p0.Y) * value;
            return new SKPoint(x, y);
        }

        private Vector Convert(SKPoint skPoint)
        {
            return new Vector(skPoint.X, skPoint.Y);
        }
    }
}
