using System;
using LedController3Client.Ui.Core;
using SkiaSharp;

namespace LedController3Client.Ui
{
    public class ThreeSlidersColorPicker : Component, IColorPicker
    {
        private readonly ColorTimeLineDrawingConfig _drawingConfig;
        private readonly ColorComponentSlider[] _colorComponentSliders;

        public ThreeSlidersColorPicker(ColorTimeLineDrawingConfig drawingConfig)
        {
            _drawingConfig = drawingConfig;

            _colorComponentSliders = new[]
            {
                new ColorComponentSlider(_drawingConfig, ColorComponentType.R),
                new ColorComponentSlider(_drawingConfig, ColorComponentType.G),
                new ColorComponentSlider(_drawingConfig, ColorComponentType.B),
            };

            foreach (var ccs in _colorComponentSliders)
            {
                ccs.ColorChanged += ColorComponentSlider_ColorChanged;
                AddChild(ccs);
            }
        }

        public event EventHandler<EventArgs<SKColor>> ColorChanged;

        private void ColorComponentSlider_ColorChanged(object sender, EventArgs<SKColor> e)
        {
            ColorChanged?.Invoke(this, new EventArgs<SKColor>(new SKColor(_colorComponentSliders[0].ColorComponentValue, _colorComponentSliders[1].ColorComponentValue, _colorComponentSliders[2].ColorComponentValue)));
        }

        public void ResetColor(SKColor color)
        {
            _colorComponentSliders[0].Reset(color.Red);
            _colorComponentSliders[1].Reset(color.Green);
            _colorComponentSliders[2].Reset(color.Blue);
        }
    }
}
