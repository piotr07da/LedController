using LedController3Client.Mathematics;
using SkiaSharp;

namespace LedController3Client.Ui
{
    public class LinearSliderBody : ISliderBody<float>
    {
        private readonly SKPoint _p0;
        private readonly SKPoint _p1;
        private readonly float _dx;
        private readonly float _dy;

        public LinearSliderBody(SKPoint p0, SKPoint p1)
        {
            _p0 = p0;
            _p1 = p1;
            _dx = _p1.X - _p0.X;
            _dy = _p1.Y - _p0.Y;
        }

        public float PositionToValue(SKPoint position, out SKPoint outputPosition)
        {
            var dragPointVec = Convert(position);
            dragPointVec.ClosestPointOnSegment(Convert(_p0), Convert(_p1), out float ratio);
            outputPosition = ValueToPosition(ratio);
            return ratio;
        }

        public SKPoint ValueToPosition(float value)
        {
            var x = _p0.X + _dx * value;
            var y = _p0.Y + _dy * value;
            return new SKPoint(x, y);
        }

        private Vector Convert(SKPoint skPoint)
        {
            return new Vector(skPoint.X, skPoint.Y);
        }
    }
}
