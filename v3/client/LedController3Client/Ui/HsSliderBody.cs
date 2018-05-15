using LedController3Client.Mathematics;
using SkiaSharp;
using System;

namespace LedController3Client.Ui
{
    public class HueSaturation
    {
        public HueSaturation(float h, float s)
        {
            H = h;
            S = s;
        }

        public float H { get; set; }
        public float S { get; set; }
    }

    public class HsSliderBody : ISliderBody<HueSaturation>
    {
        private static readonly float FullCircleAngle = (float)Math.PI * 2f;

        private readonly SKPoint _areaCenter;
        private readonly float _areaRadius;

        public HsSliderBody(SKPoint areaCenter, float areaRadius)
        {
            _areaCenter = areaCenter;
            _areaRadius = areaRadius;
        }

        public HueSaturation PositionToValue(SKPoint position, out SKPoint outputPosition)
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

            return new HueSaturation(angleRatio, radiusRatio);
        }

        public SKPoint ValueToPosition(HueSaturation value)
        {
            var cos = (float)Math.Cos(value.H * FullCircleAngle) * value.S * _areaRadius;
            var sin = (float)Math.Sin(value.H * FullCircleAngle) * value.S * _areaRadius;

            return _areaCenter + new SKPoint(cos, sin);
        }
    }
}
