using LedController3Client.Ui.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LedController3Client.Ui.Core
{
    public class ColorPositions
    {
        private ColorPosition[] _colorPositions;

        public ColorPositions(ColorPosition[] colorPositions)
        {
            _colorPositions = colorPositions;
        }

        public event EventHandler<EventArgs<ColorPosition[]>> Updated;

        public ColorPosition[] AsArray()
        {
            return _colorPositions;
        }

        public void Update(ColorPosition[] colorPositions)
        {
            _colorPositions = colorPositions;
            Updated?.Invoke(this, new EventArgs<ColorPosition[]>(_colorPositions));
        }
    }
}
