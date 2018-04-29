using LedController3Client.Mobile.PhotonLedController;

namespace LedController3Client.Mobile.ColorTimeLineDrawing
{
    public class ColorTimeLineDrawingInput
    {
        public ColorTimeLineGradient Gradient { get; set; }
        public ColorTimeLineSlider[] ColorTimePointSliders { get; set; }
        public ColorTimeLineSlider TimeProgressSlider { get; set; }
    }
}
