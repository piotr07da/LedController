using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LedController2Client.Converters
{
    public class ColorSchemeGradientToBrushConverter : ValueConverterBase, IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The System.Type of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ColorMarker[] gradient = value as ColorMarker[];
            if (gradient == null)
                return new SolidColorBrush(Colors.Red);

            LinearGradientBrush gb = new LinearGradientBrush();
            foreach (ColorMarker cm in gradient)
                gb.GradientStops.Add(new GradientStop(Color.FromRgb(cm.R, cm.G, cm.B), cm.TimePoint / (double)Byte.MaxValue));

            return gb;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in System.Windows.Data.BindingMode.TwoWay bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The System.Type of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the source object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Cannot convert from brush to color scheme gradient.");
        }
    }
}
