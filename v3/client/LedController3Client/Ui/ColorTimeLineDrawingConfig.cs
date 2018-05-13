using SkiaSharp;

namespace LedController3Client.Ui
{
    public class ColorTimeLineDrawingConfig
    {
        public ColorTimeLineDrawingConfig()
        {
            BackgroundColor = new SKColor(0x25, 0x25, 0x25);
            SliderTrackBackgroundColor = new SKColor(0x33, 0x33, 0x33);
        }

        public SKColor BackgroundColor { get; private set; }
        public SKColor SliderTrackBackgroundColor { get; private set; }
        public ColorTimeLineComponentsDimensionsConfig WorldDimensions() { return new ColorTimeLineComponentsDimensionsConfig(); }
        public ColorTimeLineComponentsDimensionsConfig ScreenDimensions(float scale) { return new ColorTimeLineComponentsDimensionsConfig(scale); }
    }

    public class ColorTimeLineComponentsDimensionsConfig
    {
        public ColorTimeLineComponentsDimensionsConfig()
            : this(1f)
        {

        }

        public ColorTimeLineComponentsDimensionsConfig(float scale)
        {
            Size = 1f;
            SizeDiv2 = .5f;
            Center = new SKPoint(.5f, .5f);
            OuterMargin = .038f;
            ColorsCircleWidth = .038f;
            GradientCircleWidth = .052f;
            ProgressCircleWidth = .038f;
            CycleTimeCircleWidth = .038f;
            HsvColorPickerValueCircleWidth = .038f;
            BetweenCirclesMargin = .015f;
            ColorsCircleRadius = Center.X - OuterMargin - ColorsCircleWidth / 2f;
            GradientCircleRadius = ColorsCircleRadius - ColorsCircleWidth / 2f - BetweenCirclesMargin - GradientCircleWidth / 2f;
            ProgressCircleRadius = GradientCircleRadius - GradientCircleWidth / 2f - BetweenCirclesMargin - ProgressCircleWidth / 2f;
            CycleTimeCircleRadius = ProgressCircleRadius - ProgressCircleWidth / 2f - BetweenCirclesMargin - CycleTimeCircleWidth / 2f;
            HsvColorPickerValueCircleRadius = CycleTimeCircleRadius;
            HsvColorPickerHueSaturationCircleRadius = HsvColorPickerValueCircleRadius - HsvColorPickerValueCircleWidth / 2f - BetweenCirclesMargin;
            HsvColorPcikerHueSaturationSliderRadius = .019f;
            AddDeleteColorTimePointButtonRadius = .07f;
            InnerHorizontalSliderBarWidth = .040f;
            InnerHorizontalSlidersX0 = .27f;
            InnerHorizontalSlidersX1 = .73f;
            InnerHorizontalSlidersY0of3 = .40f;
            InnerHorizontalSlidersY1of3 = .50f;
            InnerHorizontalSlidersY2of3 = .60f;
            InnerHorizontalSlidersY0of1 = .50f;

            Size *= scale;
            SizeDiv2 *= scale;
            Center = new SKPoint(Center.X * scale, Center.Y * scale);
            OuterMargin *= scale;
            ColorsCircleWidth *= scale;
            GradientCircleWidth *= scale;
            ProgressCircleWidth *= scale;
            CycleTimeCircleWidth *= scale;
            HsvColorPickerValueCircleWidth *= scale;
            BetweenCirclesMargin *= scale;
            ColorsCircleRadius *= scale;
            GradientCircleRadius *= scale;
            ProgressCircleRadius *= scale;
            CycleTimeCircleRadius *= scale;
            HsvColorPickerValueCircleRadius *= scale;
            HsvColorPickerHueSaturationCircleRadius *= scale;
            HsvColorPcikerHueSaturationSliderRadius *= scale;
            AddDeleteColorTimePointButtonRadius *= scale;
            InnerHorizontalSliderBarWidth *= scale;
            InnerHorizontalSlidersX0 *= scale;
            InnerHorizontalSlidersX1 *= scale;
            InnerHorizontalSlidersY0of3 *= scale;
            InnerHorizontalSlidersY1of3 *= scale;
            InnerHorizontalSlidersY2of3 *= scale;
            InnerHorizontalSlidersY0of1 *= scale;
        }

        public float Size { get; private set; }
        public float SizeDiv2 { get; private set; }
        public SKPoint Center { get; private set; }
        public float OuterMargin { get; private set; }
        public float ColorsCircleWidth { get; private set; }
        public float GradientCircleWidth { get; private set; }
        public float ProgressCircleWidth { get; private set; }
        public float CycleTimeCircleWidth { get; private set; }
        public float HsvColorPickerValueCircleWidth { get; private set; }
        public float BetweenCirclesMargin { get; private set; }
        public float ColorsCircleRadius { get; private set; }
        public float GradientCircleRadius { get; private set; }
        public float ProgressCircleRadius { get; private set; }
        public float CycleTimeCircleRadius { get; private set; }
        public float HsvColorPickerValueCircleRadius { get; private set; }
        public float HsvColorPickerHueSaturationCircleRadius { get; private set; }
        public float HsvColorPcikerHueSaturationSliderRadius { get; private set; }
        public float AddDeleteColorTimePointButtonRadius { get; private set; }
        public float InnerHorizontalSliderBarWidth { get; private set; }
        public float InnerHorizontalSlidersX0 { get; private set; }
        public float InnerHorizontalSlidersX1 { get; private set; }
        public float InnerHorizontalSlidersY0of3 { get; private set; }
        public float InnerHorizontalSlidersY1of3 { get; private set; }
        public float InnerHorizontalSlidersY2of3 { get; private set; }
        public float InnerHorizontalSlidersY0of1 { get; private set; }
    }
}
