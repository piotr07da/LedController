using LedController3Client.Mathematics;
using SkiaSharp;
using System;

namespace LedController3Client.Ui
{
    public class HsSliderBody : ISliderBody<SKColor>
    {
        private static readonly float FullCircleAngle = (float)Math.PI * 2f;
        private static readonly float Sin0of3 = 0f;
        private static readonly float Cos0of3 = 1f;
        private static readonly float Sin1of3 = (float)Math.Sin(FullCircleAngle / 3f);
        private static readonly float Cos1of3 = (float)Math.Cos(FullCircleAngle / 3f);
        private static readonly float Sin2of3 = (float)Math.Sin(FullCircleAngle * 2f / 3f);
        private static readonly float Cos2of3 = (float)Math.Cos(FullCircleAngle * 2f / 3f);
        private static readonly SKColor R = new SKColor(255, 0, 0);
        private static readonly SKColor G = new SKColor(0, 255, 0);
        private static readonly SKColor B = new SKColor(0, 0, 255);
        private static readonly SKColor[] BaseHueColors = new[] { R, G, B, R, R };

        private readonly SKPoint _areaCenter;
        private readonly float _areaRadius;

        public HsSliderBody(SKPoint areaCenter, float areaRadius)
        {
            _areaCenter = areaCenter;
            _areaRadius = areaRadius;
        }

        public SKColor PositionToValue(SKPoint position, out SKPoint outputPosition)
        {
            var vec = new Vector(position.X - _areaCenter.X, position.Y - _areaCenter.Y);
            var mag = vec.Magnitude();
            vec.Normalize();

            var angle = vec.AngleFrom(new Vector(1, 0));
            if (angle < 0)
                angle = FullCircleAngle + angle;
            var angleRatio = angle / FullCircleAngle;

            var radius = mag;
            var radiusRatio = radius / _areaRadius;

            if (radiusRatio > 1f)
                radiusRatio = 1f;

            vec *= radiusRatio * _areaRadius;
            outputPosition = _areaCenter + new SKPoint(vec.X, vec.Y);

            new HsvRgbConverter().Hsv2Rgb(angleRatio, radiusRatio, 1f, out SKColor value);
            return value;
        }

        public SKPoint ValueToPosition(SKColor value)
        {
            if (value == SKColors.Black)
                return _areaCenter + new SKPoint(_areaRadius * -1f, _areaRadius * 0);

            new HsvRgbConverter().Rgb2Hsv(value, out float h, out float s, out float v);

            // Instead of using h as angle and calculating each time cos and sin for this angle we calculate cos and sin as mean of circular quantities which gives same result but its cheaper.
            // https://en.wikipedia.org/wiki/Mean_of_circular_quantities

            var r = value.Red / 255f;
            var g = value.Green / 255f;
            var b = value.Blue / 255f;

            // We know that:
            // R is at angle 2PI * 0/3
            // G is at angle 2PI * 1/3
            // B is at angle 2PI * 2/3
            var cos = (Cos0of3 * r + Cos1of3 * g + Cos2of3 * b) / 3f;
            var sin = (Sin0of3 * r + Sin1of3 * g + Sin2of3 * b) / 3f;

            var vec = new Vector(cos, sin);
            vec.Normalize();
            vec *= _areaRadius * s;

            return _areaCenter + new SKPoint(vec.X, vec.Y);
        }
    }
}
