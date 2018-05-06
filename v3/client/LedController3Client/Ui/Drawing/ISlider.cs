using SkiaSharp;
using System;

namespace LedController3Client.Ui.Drawing
{
    public interface ISlider
    {
        event EventHandler<EventArgs<float>> ValueChanged;
        event EventHandler<EventArgs<bool>> IsSelectedChanged;

        float Value { get; set; }
        SKColor Color { get; set; }
        SKPoint Position { get; }
        float Radius { get; set; }
        bool IsVisible { get; set; }
        bool IsSelected { get; set; }
        bool HitTest(SKPoint hitPoint);
        void Drag(SKPoint dragPoint);
    }
}
