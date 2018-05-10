using SkiaSharp;
using System;

namespace LedController3Client.Ui
{
    public class HvSliderBody : ISliderBody<SKColor>
    {
        private readonly float _areaRadius;

        public HvSliderBody(float areaRadius)
        {
            _areaRadius = areaRadius;
        }

        public SKColor PositionToValue(SKPoint position)
        {
            throw new NotImplementedException();
        }

        public SKPoint ValueToPosition(SKColor value)
        {
            throw new NotImplementedException();
        }
    }
}
