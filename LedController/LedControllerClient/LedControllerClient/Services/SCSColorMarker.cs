using System.Windows.Media;

namespace LedControllerClient.Services
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

        /// <summary>
        /// Gets or sets color.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets position.
        /// </summary>
        public double Position { get; set; }
    }
}
