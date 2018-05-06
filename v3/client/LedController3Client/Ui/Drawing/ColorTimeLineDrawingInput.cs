
namespace LedController3Client.Ui.Drawing
{
    public class ColorTimeLineDrawingInput
    {
        public ColorTimePointSlider[] ColorTimePointSliders { get; set; }
        public TimeProgressSlider TimeProgressSlider { get; set; }
        public ISlider[] ColorComponentSliders { get; set; }
        public ISlider CycleTimeSlider { get; set; }
    }
}
