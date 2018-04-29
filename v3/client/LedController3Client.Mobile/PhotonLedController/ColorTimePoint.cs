
namespace LedController3Client.Mobile.PhotonLedController
{
    public class ColorTimePoint
    {
        public ColorTimePoint(int id, ColorTimePointColor c, float t)
        {
            Id = id;
            Color = c;
            Time = t;
        }

        public int Id { get; set; }
        public ColorTimePointColor Color { get; set; }
        public float Time { get; set; }
    }
}
