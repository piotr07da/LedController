using System.Windows.Media;

namespace LedController2Client.Services
{
    public class SCSColorMarker
    {
        /// <summary>
        /// Creates new instance of <see cref="SCSColorMarker"/>.
        /// </summary>
        public SCSColorMarker()
        {
            //
        }

        private byte _timePoint;
        private double _position;

        /// <summary>
        /// Gets or sets color.
        /// </summary>
        public Color Color { get; set; }

        public byte TimePoint
        {
            get { return _timePoint; }
            set
            {
                _timePoint = value;
                _position = _timePoint / 255.0;
            }
        }

        /// <summary>
        /// Gets or sets position.
        /// </summary>
        public double Position { get { return _position; } }
    }
}
