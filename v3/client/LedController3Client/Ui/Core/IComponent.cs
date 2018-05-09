using System.Collections.Generic;

namespace LedController3Client.Ui.Core
{
    public interface IComponent
    {
        bool IsEnabled { get; set; }

        IEnumerable<IComponent> Children { get; }

        void AddChild(IComponent child);

        void RemoveChild(IComponent child);

        void AppendTo(IList<IComponent> components);
    }
}
