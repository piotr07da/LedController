using LedController3Client.Mobile.PhotonLedController;

namespace LedController3Client.Mobile.ColorTimeLineDrawing
{
    public class ColorTimeLineDrawingInput
    {
        public ColorTimePointSlider[] ColorTimePointSliders { get; set; }
        public TimeProgressSlider TimeProgressSlider { get; set; }
        public ColorTimeLineSlider SelectedSlider { get; set; }
    }
}
