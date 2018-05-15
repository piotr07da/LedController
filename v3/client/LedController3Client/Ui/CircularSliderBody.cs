using LedController3Client.Mathematics;
using SkiaSharp;
using System;

namespace LedController3Client.Ui
{
    public class CircularSliderBody : ISliderBody<float>
    {
        private readonly SKPoint _orbitCenter;
        private readonly float _orbitRadius;
        private readonly Func<float, float> _inputValueConverter;
        private readonly Func<float, float> _outputValueConverter;

        public CircularSliderBody(SKPoint orbitCenter, float orbitRadius)
            :this(orbitCenter, orbitRadius, v => v, v => v)
        {
        }

        public CircularSliderBody(SKPoint orbitCenter, float orbitRadius, Func<float, float> inputValueConverter, Func<float, float> outputValueConverter)
        {
            _orbitCenter = orbitCenter;
            _orbitRadius = orbitRadius;
            _inputValueConverter = inputValueConverter;
            _outputValueConverter = outputValueConverter;
        }

        public SKPoint OrbitCenter { get => _orbitCenter; }
        public float OrbitRadius { get => _orbitRadius; }

        public float PositionToValue(SKPoint dragPoint, out SKPoint outputPosition)
        {
            var dragPointVec = Convert(dragPoint);
            var orbitCenterVec = Convert(OrbitCenter);
            var touchVector = dragPointVec - orbitCenterVec;
            touchVector.Normalize();
            var angle = touchVector.AngleFrom(new Vector(1, 0));
            var fullCircleAngle = 2 * (float)Math.PI;
            if (angle < 0)
                angle = fullCircleAngle + angle;
            var value = angle / fullCircleAngle;

            touchVector *= OrbitRadius;
            outputPosition = OrbitCenter + new SKPoint(touchVector.X, touchVector.Y);

            return _outputValueConverter(value);
        }

        public SKPoint ValueToPosition(float value)
        {
            value = _inputValueConverter(value);

            var angle = 2.0 * Math.PI * value;
            var x = OrbitCenter.X + OrbitRadius * (float)Math.Cos(angle);
            var y = OrbitCenter.Y + OrbitRadius * (float)Math.Sin(angle);
            return new SKPoint(x, y);
        }

        private Vector Convert(SKPoint skPoint)
        {
            return new Vector(skPoint.X, skPoint.Y);
        }
    }
}
