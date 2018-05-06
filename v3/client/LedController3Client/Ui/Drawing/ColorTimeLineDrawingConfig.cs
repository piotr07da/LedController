using SkiaSharp;

namespace LedController3Client.Ui.Drawing
{
    public class ColorTimeLineDrawingConfig
    {
        public ColorTimeLineDrawingConfig()
            :this(1f)
        {

        }

        public ColorTimeLineDrawingConfig(float scale)
        {
            Size = 1f;
            SizeDiv2 = .5f;
            Center = new SKPoint(.5f, .5f);
            OuterMargin = .035f;
            ColorsCircleWidth = .040f;
            GradientCircleWidth = .055f;
            ProgressCircleWidth = .040f;
            BetweenCirclesMargin = .015f;
            ColorsCircleRadius = Center.X - OuterMargin - ColorsCircleWidth / 2f;
            GradientCircleRadius = ColorsCircleRadius - ColorsCircleWidth / 2f - BetweenCirclesMargin - GradientCircleWidth / 2f;
            ProgressCircleRadius = GradientCircleRadius - GradientCircleWidth / 2f - BetweenCirclesMargin - ProgressCircleWidth / 2f;
            InnerHorizontalSliderBarWidth = .040f;
            InnerHorizontalSlidersX0 = .30f;
            InnerHorizontalSlidersX1 = .70f;
            InnerHorizontalSlidersY0of3 = .35f;
            InnerHorizontalSlidersY1of3 = .50f;
            InnerHorizontalSlidersY2of3 = .65f;
            InnerHorizontalSlidersY0of1 = .50f;

            Size *= scale;
            SizeDiv2 *= scale;
            Center = new SKPoint(Center.X * scale, Center.Y * scale);
            OuterMargin *= scale;
            ColorsCircleWidth *= scale;
            GradientCircleWidth *= scale;
            ProgressCircleWidth *= scale;
            BetweenCirclesMargin *= scale;
            ColorsCircleRadius *= scale;
            GradientCircleRadius *= scale;
            ProgressCircleRadius *= scale;
            InnerHorizontalSliderBarWidth *= scale;
            InnerHorizontalSlidersX0 *= scale;
            InnerHorizontalSlidersX1 *= scale;
            InnerHorizontalSlidersY0of3 *= scale;
            InnerHorizontalSlidersY1of3 *= scale;
            InnerHorizontalSlidersY2of3 *= scale;
            InnerHorizontalSlidersY0of1 *= scale;

            BackgroundColor = new SKColor(0x25, 0x25, 0x25);
            CirclesBackgroundColor = new SKColor(0x33, 0x33, 0x33);
        }

        public float Size { get; private set; }
        public float SizeDiv2 { get; private set; }
        public SKPoint Center { get; private set; }
        public float OuterMargin { get; private set; }
        public float ColorsCircleWidth { get; private set; }
        public float GradientCircleWidth { get; private set; }
        public float ProgressCircleWidth { get; private set; }
        public float BetweenCirclesMargin { get; private set; }
        public float ColorsCircleRadius { get; private set; }
        public float GradientCircleRadius { get; private set; }
        public float ProgressCircleRadius { get; private set; }
        public float InnerHorizontalSliderBarWidth { get; private set; }
        public float InnerHorizontalSlidersX0 { get; private set; }
        public float InnerHorizontalSlidersX1 { get; private set; }
        public float InnerHorizontalSlidersY0of3 { get; private set; }
        public float InnerHorizontalSlidersY1of3 { get; private set; }
        public float InnerHorizontalSlidersY2of3 { get; private set; }
        public float InnerHorizontalSlidersY0of1 { get; private set; }
        public SKColor BackgroundColor { get; private set; }
        public SKColor CirclesBackgroundColor { get; private set; }
    }
}
