using SkiaSharp;

namespace LedController3Client.Ui
{
    public interface ISliderBody<TValue>
    {
        TValue PositionToValue(SKPoint position, out SKPoint outputPosition);
        SKPoint ValueToPosition(TValue value);
    }
}
