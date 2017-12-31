using System;
using System.Windows.Markup;

namespace LedController2Client.Converters
{
    /// <summary>
    /// Base class for all value converters.
    /// </summary>
    public abstract class ValueConverterBase : MarkupExtension
    {
        /// <summary>
        /// Creates new instance of <see cref="ValueConverterBase"/>.
        /// </summary>
        public ValueConverterBase()
        {

        }

        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
