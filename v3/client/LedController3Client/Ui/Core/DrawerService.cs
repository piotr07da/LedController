using SkiaSharp;
using System.Collections.Generic;

namespace LedController3Client.Ui.Core
{
    public class DrawerService
    {
        public void Draw(IComponent component, SKCanvas canvas, float scale)
        {
            var allComponents = new List<IComponent>();
            component.AppendTo(allComponents);
            foreach (var c in allComponents)
            {
                var dc = c as IDrawerComponent;
                if (dc == null)
                    continue;

                dc.Draw(canvas, scale);
            }
        }
    }
}
