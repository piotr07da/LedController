using LedController3Client.Ui.Core;
using SkiaSharp;

namespace LedController3Client.Ui
{
    public class ColorPositionLine
    {
        private readonly ColorsInterpolator _colorsInterpolator;
        private readonly ColorPosition[] _colorPositions;

        public ColorPositionLine(ColorPosition[] colorPositions)
        {
            _colorsInterpolator = new ColorsInterpolator();
            _colorPositions = colorPositions;
        }

        public SKColor ColorAt(float position)
        {
            var pointCount = _colorPositions != null ? _colorPositions.Length : 0;

            if (pointCount == 0)
            {
                return SKColors.Black;
            }

            if (pointCount == 1)
            {
                return _colorPositions[0].Color;
            }

            var lctp = _colorPositions[0];
            var rctp = _colorPositions[_colorPositions.Length - 1];

            if (position < lctp.Position || position > rctp.Position)
            {
                Swap(ref lctp, ref rctp);
            }
            else
            {
                for (var i = 1; i < pointCount - 1; ++i)
                {
                    var ctp = _colorPositions[i];
                    var ctpTime = ctp.Position;

                    if (ctpTime <= position && ctpTime > lctp.Position)
                        lctp = ctp;
                    if (ctpTime >= position && ctpTime < rctp.Position)
                        rctp = ctp;
                }
            }

            var ratio = InverseLerp(lctp.Position, rctp.Position, position);

            _colorsInterpolator.InterpolateColors(lctp.Color, rctp.Color, ratio, out SKColor outColor);

            return outColor;
        }

        private float InverseLerp(float lValue, float rValue, float value)
        {
            var progress = value - lValue;
            var range = rValue - lValue;

            if (progress < 0)
                progress = 1 + progress;

            if (range < 0)
                range = 1 + range;

            if (range > 0)
            {
                return progress / range;
            }
            return .5f;
        }

        private void Swap<T>(ref T lhs, ref T rhs)
        {
            T tmp = lhs;
            lhs = rhs;
            rhs = tmp;
        }
    }
}
