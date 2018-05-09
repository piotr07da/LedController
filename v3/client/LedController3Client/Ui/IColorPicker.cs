using LedController3Client.Ui.Core;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace LedController3Client.Ui
{
    public interface IColorPicker : IComponent
    {
        event EventHandler<EventArgs<SKColor>> ColorChanged;

        void ResetColor(SKColor color);
    }
}
