using SkiaSharp;

namespace LedController3Client.Ui.Core
{
    public interface ITouchHandlerComponent : IComponent
    {
        bool Handle(long touchId, SKPoint touchLocation, TouchAction touchAction);
    }
}
