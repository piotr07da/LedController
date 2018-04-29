using SkiaSharp;

namespace LedController3Client.Mobile.ColorTimeLineDrawing
{
    public class ColorTimeLineDrawingConfig
    {
        public ColorTimeLineDrawingConfig()
            :this(1f)
        {

        }

        public ColorTimeLineDrawingConfig(float scale)
        {
            Center = .5f;
            OuterMargin = .035f;
            ColorsCircleWidth = .040f;
            GradientCircleWidth = .055f;
            ProgressCircleWidth = .040f;
            BetweenCirclesMargin = .015f;
            ColorsCircleRadius = Center - OuterMargin - ColorsCircleWidth / 2f;
            GradientCircleRadius = ColorsCircleRadius - ColorsCircleWidth / 2f - BetweenCirclesMargin - GradientCircleWidth / 2f;
            ProgressCircleRadius = GradientCircleRadius - GradientCircleWidth / 2f - BetweenCirclesMargin - ProgressCircleWidth / 2f;

            Center *= scale;
            OuterMargin *= scale;
            ColorsCircleWidth *= scale;
            GradientCircleWidth *= scale;
            ProgressCircleWidth *= scale;
            BetweenCirclesMargin *= scale;
            ColorsCircleRadius *= scale;
            GradientCircleRadius *= scale;
            ProgressCircleRadius *= scale;

            BackgroundColor = new SKColor(0x25, 0x25, 0x25);
            CirclesBackgroundColor = new SKColor(0x33, 0x33, 0x33);
    }

        public float Center { get; private set; }
        public float OuterMargin { get; private set; }
        public float ColorsCircleWidth { get; private set; }
        public float GradientCircleWidth { get; private set; }
        public float ProgressCircleWidth { get; private set; }
        public float BetweenCirclesMargin { get; private set; }
        public float ColorsCircleRadius { get; private set; }
        public float GradientCircleRadius { get; private set; }
        public float ProgressCircleRadius { get; private set; }
        public SKColor BackgroundColor { get; private set; }
        public SKColor CirclesBackgroundColor { get; private set; }
    }
}
