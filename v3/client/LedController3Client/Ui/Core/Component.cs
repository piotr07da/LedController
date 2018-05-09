using System.Collections.Generic;

namespace LedController3Client.Ui.Core
{
    public class Component : IComponent
    {
        public bool IsEnabled { get; set; } = true;

        private List<IComponent> _children = new List<IComponent>();
        public IEnumerable<IComponent> Children => _children;

        public void AddChild(IComponent child)
        {
            _children.Add(child);
        }

        public void RemoveChild(IComponent child)
        {
            _children.Remove(child);
        }

        public void AppendTo(IList<IComponent> components)
        {
            if (!IsEnabled)
                return;

            components.Add(this);

            foreach (var c in Children)
                c.AppendTo(components);
        }
    }
}
