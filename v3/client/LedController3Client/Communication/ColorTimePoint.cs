
namespace LedController3Client.Communication
{
    public class ColorTimePoint
    {
        public ColorTimePoint(byte id, ColorTimePointColor c, float t)
        {
            Id = id;
            Color = c;
            Time = t;
        }

        public byte Id { get; set; }
        public ColorTimePointColor Color { get; set; }
        public float Time { get; set; }
    }
}
