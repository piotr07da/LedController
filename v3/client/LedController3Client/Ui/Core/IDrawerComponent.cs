using SkiaSharp;

namespace LedController3Client.Ui.Core
{
    public interface IDrawerComponent : IComponent
    {
        void Draw(SKCanvas canvas, float scale);
    }
}
