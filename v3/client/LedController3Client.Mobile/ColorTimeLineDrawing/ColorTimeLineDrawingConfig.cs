using SkiaSharp;

namespace LedController3Client.Mobile.ColorTimeLineDrawing
{
    public class ColorTimeLineDrawingConfig
    {
        private const float OuterMarginRatio = .035f;
        private const float ColorsCircleWidthRatio = .040f;
        private const float GradientCircleWidthRatio = .055f;
        private const float ProgressCircleWidthRatio = .040f;
        private const float BetweenCirclesMarginRatio = .015f;

        public ColorTimeLineDrawingConfig(float size)
        {
            Size = size;
            SizeDiv2 = size / 2f;
            OuterMargin = OuterMarginRatio * size;
            ColorsCircleWidth = ColorsCircleWidthRatio * size;
            GradientCircleWidth = GradientCircleWidthRatio * size;
            ProgressCircleWidth = ProgressCircleWidthRatio * size;
            BetweenCirclesMargin = BetweenCirclesMarginRatio * size;

            ColorsCircleRadius = SizeDiv2 - OuterMargin - ColorsCircleWidth / 2f;
            GradientCircleRadius = ColorsCircleRadius - ColorsCircleWidth / 2f - BetweenCirclesMargin - GradientCircleWidth / 2f;
            ProgressCircleRadius = GradientCircleRadius - GradientCircleWidth / 2f - BetweenCirclesMargin - ProgressCircleWidth / 2f;

            BackgroundColor = new SKColor(0x25, 0x25, 0x25);
            CirclesBackgroundColor = new SKColor(0x33, 0x33, 0x33);
        }

        public float Size { get; private set; }
        public float SizeDiv2 { get; private set; }

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
