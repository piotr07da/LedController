using SkiaSharp;

namespace LedController3Client.Ui
{
    public interface ISliderBody
    {
        float PositionToValue(SKPoint position);
        SKPoint ValueToPosition(float value);
    }
}
