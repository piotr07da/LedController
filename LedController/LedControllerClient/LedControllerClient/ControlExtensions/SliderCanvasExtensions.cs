using LedControllerClient.Services;
using System.Windows;
using System.Windows.Controls;

namespace LedControllerClient.ControlExtensions
{
    public static class SliderCanvasExtensions
    {
        public static SliderCanvasService GetSliderService(DependencyObject obj)
        {
            return (SliderCanvasService)obj.GetValue(SliderServiceProperty);
        }

        public static void SetSliderService(DependencyObject obj, SliderCanvasService value)
        {
            obj.SetValue(SliderServiceProperty, value);
        }

        public static readonly DependencyProperty SliderServiceProperty = DependencyProperty.RegisterAttached("SliderService", typeof(SliderCanvasService), typeof(SliderCanvasExtensions), new PropertyMetadata(null, SliderServicePropertyChanged));

        public static void SliderServicePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (e.NewValue as SliderCanvasService).Canvas = d as Canvas;
        }
    }
}
