using LedController3Client.Ui.Core;
using SkiaSharp;

namespace LedController3Client.Ui
{
    public class CircularTrack : Component
    {
        private readonly IDrawerComponent _drawer;
        
        public CircularTrack(SKPoint center, float radius, float thickness, SKColor color)
        {
            _drawer = new ColorCircularTrackDrawer(center, radius, thickness, color);
            AddChild(_drawer);
        }

        public CircularTrack(SKPoint center, float radius, float thickness, int circularResolution, ColorPositions colorPositions)
        {
            _drawer = new GradientCircularTrackDrawer(center, radius, thickness, circularResolution, colorPositions);
            AddChild(_drawer);
        }
    }
}
