using SkiaSharp;
using System.Collections.Generic;

namespace LedController3Client.Ui.Core
{
    public class TouchHandlerService
    {
        public void Handle(IComponent component, long touchId, SKPoint touchLocation, TouchAction touchAction)
        {
            var allComponents = new List<IComponent>();
            component.AppendTo(allComponents);
            foreach (var c in allComponents)
            {
                var thc = c as ITouchHandlerComponent;
                if (thc == null)
                    continue;

                var hr = thc.Handle(touchId, touchLocation, touchAction);
                if (hr)
                    break;
            }
        }
    }
}
