using SkiaSharp;

namespace LedController3Client.Ui.Drawing
{
    public class ColorTimeLine
    {
        public ColorTimeLine(ISlider[] colorTimeLineSliders)
        {
            _colorTimeLineSliders = colorTimeLineSliders;
        }

        private readonly ISlider[] _colorTimeLineSliders;

        public SKColor ColorAt(float timeProgress)
        {
            var ctls = _colorTimeLineSliders;
            var pointCount = ctls != null ? ctls.Length : 0;

            if (pointCount == 0)
            {
                return SKColors.Black;
            }

            if (pointCount == 1)
            {
                return ctls[0].Color;
            }

            var lctp = ctls[0];
            var rctp = ctls[ctls.Length - 1];

            if (timeProgress < lctp.Value || timeProgress > rctp.Value)
            {
                Swap(ref lctp, ref rctp);
            }
            else
            {
                for (var i = 1; i < pointCount - 1; ++i)
                {
                    var ctp = ctls[i];
                    var ctpTime = ctp.Value;

                    if (ctpTime <= timeProgress && ctpTime > lctp.Value)
                        lctp = ctp;
                    if (ctpTime >= timeProgress && ctpTime < rctp.Value)
                        rctp = ctp;
                }
            }

            var ratio = InverseLerp(lctp.Value, rctp.Value, timeProgress);

            InterpolateColors(lctp.Color, rctp.Color, ratio, out SKColor outColor);
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

        public void InterpolateColors(SKColor lColor, SKColor rColor, float ratio, out SKColor outColor)
        {
            InterpolateColorsComponents(lColor.Red, rColor.Red, ratio, out byte r);
            InterpolateColorsComponents(lColor.Green, rColor.Green, ratio, out byte g);
            InterpolateColorsComponents(lColor.Blue, rColor.Blue, ratio, out byte b);
            outColor = new SKColor(r, g, b);
        }

        private void InterpolateColorsComponents(byte lColorComponent, byte rColorComponent, float ratio, out byte outColorComponent)
        {
            outColorComponent = (byte)(lColorComponent * (1f - ratio) + rColorComponent * ratio);
        }

        private void Swap<T>(ref T lhs, ref T rhs)
        {
            T tmp = lhs;
            lhs = rhs;
            rhs = tmp;
        }
    }
}
