using SkiaSharp;

namespace LedController3Client.Ui
{
    public interface ISliderBody<TValue>
    {
        TValue PositionToValue(SKPoint position);
        SKPoint ValueToPosition(TValue value);
    }
}
