using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LedController2Client.Services
{
    public class SliderCanvasService : IColorMultislider
    {
        #region Constants

        private const double __FULL_CIRCLE_DEGREES = 360;
        private const double __FULL_CIRCLE_RADIANS = 2 * Math.PI;

        private const int __GRADIENT_CIRCLE_RESOLUTION = 256;
        private const double __GRADIENT_CIRCLE_MARGIN = 22;
        private const double __GRADIENT_CIRCLE_WIDTH = 18;
        private const int __GRADIENT_CIRCLE_SINGLE_POLYGON_POINT_COUNT = 4;

        private const double __GRADIENT_CIRCLE_COLOR_MARKER_ANGLE_DIVIDER = 54;
        private const double __GRADIENT_CIRCLE_COLOR_MARKER_ANGLE = __FULL_CIRCLE_RADIANS / __GRADIENT_CIRCLE_COLOR_MARKER_ANGLE_DIVIDER;
        private const int __GRADIENT_CIRCLE_COLOR_MARKER_RESOLUTION = (int)(__GRADIENT_CIRCLE_RESOLUTION / __GRADIENT_CIRCLE_COLOR_MARKER_ANGLE_DIVIDER);
        private const double __GRADIENT_CIRCLE_COLOR_MARKER_MARGIN = 5;
        private const double __GRADIENT_CIRCLE_COLOR_MARKER_WIDTH = 15;

        private const double __GRADIENT_CIRCLE_PROGRESS_MARKER_ANGLE_DIVIDER = 9;
        private const double __GRADIENT_CIRCLE_PROGRESS_MARKER_ANGLE = __FULL_CIRCLE_RADIANS / __GRADIENT_CIRCLE_PROGRESS_MARKER_ANGLE_DIVIDER;
        private const int __GRADIENT_CIRCLE_PROGRESS_MARKER_RESOLUTION = (int)(__GRADIENT_CIRCLE_RESOLUTION / __GRADIENT_CIRCLE_PROGRESS_MARKER_ANGLE_DIVIDER);
        private const double __GRADIENT_CIRCLE_PROGRESS_MARKER_MARGIN = 5;
        private const double __GRADIENT_CIRCLE_PROGRESS_MARKER_WIDTH = 15;

        private const string __rk_SLIDER_TRACK_BACKGROUND = "SliderTrackBackground";

        #endregion

        #region Attributes

        private double _canvasW;
        private double _canvasH;

        private double _canvasWDiv2;
        private double _canvasHDiv2;
        private double _canvasHDiv4;

        private double _gradientCircleInnerRadius;
        private double _gradientCircleOuterRadius;
        private double _gradientCircleColorMarkersTrackInnerRadius;
        private double _gradientCircleColorMarkersTrackOuterRadius;
        private double _gradientCircleProgressMarkerTrackInnerRadius;
        private double _gradientCircleProgressMarkerTrackOuterRadius;

        // ---------

        private List<SCSColorMarker> _colorMarkers;
        private int _grabbedMarkerIndex;
        private bool _grabbedProgressMarker;

        // ---------

        private List<Polygon> _gradientCircle;
        private Polygon _gradientCircleColorMarkersTrack;
        private List<Polygon> _gradientCircleColorMarkers;
        private Polygon _gradientCircleProgressMarkerTrack;
        private Polygon _gradientCircleProgressMarker;

        // ---------

        private SolidColorBrush _markerTracksBrush;

        #endregion

        #region Properties

        private Canvas _canvas;
        public Canvas Canvas
        {
            get { return _canvas; }
            set
            {
                _canvas = value;
                _canvas.Loaded += new RoutedEventHandler(_canvas_Loaded);
                _canvas.SizeChanged += _canvas_SizeChanged;
                _canvas.MouseMove += new System.Windows.Input.MouseEventHandler(_canvas_MouseMove);
                _canvas.MouseLeftButtonUp += _canvas_MouseLeftButtonUp;

                Init();
            }
        }

        #endregion

        #region Event handling

        void _canvas_Loaded(object sender, RoutedEventArgs e)
        {

        }

        void _canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshSizes();
            RefreshCanvasObjects();
        }

        void _canvas_MouseMove(object sender, MouseEventArgs e)
        {
            TryToMoveGrabbedColorMarker();
            TryToMoveGrabbedProgressMarker();
        }

        void _canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GlobalMouseLeftButtonUp(sender, e);
        }

        void MarkerPolygon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e, byte markerIndex)
        {
            _grabbedMarkerIndex = markerIndex;

            if (MarkerSelected != null)
            {
                MarkerSelected(new ColorMultisliderEventArgs() { MarkerIndex = markerIndex });
            }
        }

        void MarkerPolygon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e, byte markerIndex)
        {
            GlobalMouseLeftButtonUp(sender, e);
        }

        void _gradientCircleProgressMarker_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _grabbedProgressMarker = true;
        }

        void _gradientCircleProgressMarker_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GlobalMouseLeftButtonUp(sender, e);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inits.
        /// </summary>
        public virtual void Init()
        {
            _markerTracksBrush = Application.Current.Resources[__rk_SLIDER_TRACK_BACKGROUND] as SolidColorBrush;

            InitGradientCircle();
            InitGradientCircleColorMarkersTrack();
            InitGradientCircleColorMarkers();
            InitGradientCircleProgressMarkerTrack();
            InitGradientCircleProgressMarker();
        }

        /// <summary>
        /// Inits gradient circle.
        /// </summary>
        protected virtual void InitGradientCircle()
        {
            // Remove old polys

            if (_gradientCircle != null && _gradientCircle.Any())
                foreach (Polygon poly in _gradientCircle)
                    _canvas.Children.Remove(poly);

            // Create new polys

            _gradientCircle = new List<Polygon>();
            for (int polyIx = 0; polyIx < __GRADIENT_CIRCLE_RESOLUTION; ++polyIx)
            {
                Polygon p = new Polygon();
                for (int i = 0; i < __GRADIENT_CIRCLE_SINGLE_POLYGON_POINT_COUNT; ++i)
                    p.Points.Add(new Point());
                p.StrokeThickness = 1;
                _canvas.Children.Add(p);
                _gradientCircle.Add(p);
            }
        }

        protected virtual void InitGradientCircleColorMarkersTrack()
        {
            InitSinglePolygon(out _gradientCircleColorMarkersTrack);
            _gradientCircleColorMarkersTrack.Fill = _markerTracksBrush;
            _gradientCircleColorMarkersTrack.Stroke = _markerTracksBrush;
        }

        /// <summary>
        /// Inits gradient circle color markers.
        /// </summary>
        protected virtual void InitGradientCircleColorMarkers()
        {
            _colorMarkers = new List<SCSColorMarker>();
            _gradientCircleColorMarkers = new List<Polygon>();
            _grabbedMarkerIndex = -1;
        }

        protected virtual void InitGradientCircleProgressMarkerTrack()
        {
            InitSinglePolygon(out _gradientCircleProgressMarkerTrack);
            _gradientCircleProgressMarkerTrack.Fill = _markerTracksBrush;
            _gradientCircleProgressMarkerTrack.Stroke = _markerTracksBrush;
        }

        /// <summary>
        /// Inits gradient circle progress marker.
        /// </summary>
        protected virtual void InitGradientCircleProgressMarker()
        {
            InitSinglePolygon(out _gradientCircleProgressMarker);
        }

        /// <summary>
        /// Inits single polygon.
        /// </summary>
        /// <param name="p">Polygon reference.</param>
        protected virtual void InitSinglePolygon(out Polygon p)
        {
            _canvas.Children.Add(p = new Polygon() { StrokeThickness = 1 });
        }

        /// <summary>
        /// Refreshes canvas objects.
        /// </summary>
        protected virtual void RefreshCanvasObjects()
        {
            RefreshGradientCircle();
            RefreshGradientCircleColorMarkersTrack();
            RefreshGradientCircleColorMarkers();
            RefreshGradientCircleProgressMarkerTrack();
            RefreshGradientCircleProgressMarker();
        }

        protected virtual void RefreshGradientCircle()
        {
            RefreshGradientCircleGeometry();
            RefreshGradientCircleColor();
        }

        protected virtual void RefreshGradientCircleGeometry()
        {
            double angle = -Math.PI / 2.0;
            double angleStep = 2 * Math.PI / __GRADIENT_CIRCLE_RESOLUTION;

            double innerX;
            double innerY;
            double outerX;
            double outerY;

            CalculateCircularBarPoints(ref angle, angleStep, _gradientCircleInnerRadius, _gradientCircleOuterRadius, out innerX, out innerY, out outerX, out outerY);

            for (int polyIx = 0; polyIx < __GRADIENT_CIRCLE_RESOLUTION; ++polyIx)
            {
                Polygon p = _gradientCircle[polyIx];

                p.Fill = _markerTracksBrush;
                p.Stroke = _markerTracksBrush;

                p.Points[0] = new Point(innerX, innerY);
                p.Points[1] = new Point(outerX, outerY);

                CalculateCircularBarPoints(ref angle, angleStep, _gradientCircleInnerRadius, _gradientCircleOuterRadius, out innerX, out innerY, out outerX, out outerY);

                p.Points[2] = new Point(outerX, outerY);
                p.Points[3] = new Point(innerX, innerY);
            }
        }

        protected virtual void RefreshGradientCircleColor()
        {
            if (_colorMarkers == null || _colorMarkers.Count < 2)
                return;

            int colorMarkerIndex = 0;
            int colorMarkerCount = _colorMarkers.Count;
            SCSColorMarker colorMarkerPrev = _colorMarkers[colorMarkerIndex++];
            SCSColorMarker colorMarkerNext = _colorMarkers[colorMarkerIndex++];

            for (int polyIx = 0; polyIx < __GRADIENT_CIRCLE_RESOLUTION; ++polyIx)
            {
                Polygon p = _gradientCircle[polyIx];

                double inCirclePosition = polyIx / (double)__GRADIENT_CIRCLE_RESOLUTION;
                if (inCirclePosition >= colorMarkerNext.Position)
                {
                    colorMarkerPrev = colorMarkerNext;
                    colorMarkerNext = _colorMarkers[colorMarkerIndex++];
                }

                double betweenMarkerProgress = (inCirclePosition - colorMarkerPrev.Position) / (colorMarkerNext.Position - colorMarkerPrev.Position);
                Color inCircleColor;
                MixColors(colorMarkerPrev.Color, colorMarkerNext.Color, (float)betweenMarkerProgress, out inCircleColor);
                SolidColorBrush brush = new SolidColorBrush(inCircleColor);
                p.Fill = brush;
                p.Stroke = brush;
            }
        }

        protected virtual void RefreshGradientCircleColorMarkersTrack()
        {
            CreateCircularBar(_gradientCircleColorMarkersTrack, __FULL_CIRCLE_RADIANS, __GRADIENT_CIRCLE_RESOLUTION, _gradientCircleColorMarkersTrackInnerRadius, _gradientCircleColorMarkersTrackOuterRadius);
        }

        /// <summary>
        /// Refreshes all color marker polygons.
        /// </summary>
        protected virtual void RefreshGradientCircleColorMarkers()
        {
            foreach (Polygon cm in _gradientCircleColorMarkers)
                _canvas.Children.Remove(cm);
            _gradientCircleColorMarkers.Clear();

            int cnt = _colorMarkers.Count;
            for (byte cmIx = 0; cmIx < cnt; ++cmIx)
            {
                SCSColorMarker scscm = _colorMarkers[cmIx];

                Polygon cm = new Polygon();
                cm.StrokeThickness = 1;

                _gradientCircleColorMarkers.Add(cm);
                _canvas.Children.Add(cm);

                RefreshGradientCircleColorMarkerGeometry(cmIx);
                RefreshGradientCircleColorMarkerColorAndPosition(cmIx);

                byte indexCopy = cmIx;
                cm.MouseLeftButtonDown += (sender, e) => MarkerPolygon_MouseLeftButtonDown(sender, e, indexCopy);
                cm.MouseLeftButtonUp += (sender, e) => MarkerPolygon_MouseLeftButtonUp(sender, e, indexCopy);
            }
        }

        /// <summary>
        /// Refreshes (builds) geometry for gradient
        /// </summary>
        protected virtual void RefreshGradientCircleColorMarkerGeometry(byte index)
        {
            Polygon cm = _gradientCircleColorMarkers[index];
            CreateCircularBar(cm, __GRADIENT_CIRCLE_COLOR_MARKER_ANGLE, __GRADIENT_CIRCLE_COLOR_MARKER_RESOLUTION, _gradientCircleColorMarkersTrackInnerRadius, _gradientCircleColorMarkersTrackOuterRadius);
        }

        /// <summary>
        /// Refreshes single color marker on gradient circle.
        /// </summary>
        /// <param name="index">Index of color marker.</param>
        protected virtual void RefreshGradientCircleColorMarkerColorAndPosition(byte index)
        {
            SCSColorMarker scscm = _colorMarkers[index];
            Polygon cm = _gradientCircleColorMarkers[index];

            cm.RenderTransform = new RotateTransform((scscm.Position - .75) * __FULL_CIRCLE_DEGREES, _canvasWDiv2, _canvasHDiv2);
            cm.Fill = new SolidColorBrush(scscm.Color);
            cm.Stroke = new SolidColorBrush(scscm.Color);
        }

        protected virtual void RefreshGradientCircleProgressMarkerTrack()
        {
            CreateCircularBar(_gradientCircleProgressMarkerTrack, __FULL_CIRCLE_RADIANS, __GRADIENT_CIRCLE_RESOLUTION, _gradientCircleProgressMarkerTrackInnerRadius, _gradientCircleProgressMarkerTrackOuterRadius);
        }

        /// <summary>
        /// Refreshes gradient circle progress marker.
        /// </summary>
        protected virtual void RefreshGradientCircleProgressMarker()
        {
            RefreshGradientCircleProgressMarkerGeometry();
            RefreshGradientCircleProgressMarkerColorAndPosition(0);

            _gradientCircleProgressMarker.MouseLeftButtonDown += _gradientCircleProgressMarker_MouseLeftButtonDown;
            _gradientCircleProgressMarker.MouseLeftButtonUp += _gradientCircleProgressMarker_MouseLeftButtonUp;
        }

        /// <summary>
        /// Refreshes gradient circle progress marker geometry.
        /// </summary>
        protected virtual void RefreshGradientCircleProgressMarkerGeometry()
        {
            CreateCircularBar(_gradientCircleProgressMarker, __GRADIENT_CIRCLE_PROGRESS_MARKER_ANGLE, __GRADIENT_CIRCLE_PROGRESS_MARKER_RESOLUTION, _gradientCircleProgressMarkerTrackInnerRadius, _gradientCircleProgressMarkerTrackOuterRadius);
        }

        /// <summary>
        /// Refreshes gradient circle progress marker color and position.
        /// </summary>
        /// <param name="progress">Progress.</param>
        protected virtual void RefreshGradientCircleProgressMarkerColorAndPosition(double progress)
        {
            if (_colorMarkers == null || _colorMarkers.Count < 2)
                return;

            _gradientCircleProgressMarker.RenderTransform = new RotateTransform((progress - .75) * __FULL_CIRCLE_DEGREES, _canvasWDiv2, _canvasHDiv2);

            int colorMarkerIndex = 0;
            int colorMarkerCount = _colorMarkers.Count;
            SCSColorMarker colorMarkerPrev = _colorMarkers[colorMarkerIndex++];
            SCSColorMarker colorMarkerNext = _colorMarkers[colorMarkerIndex++];

            int cnt = _colorMarkers.Count;
            for (int markerIx = 0; markerIx < cnt; ++markerIx)
            {
                if (progress < 1.0 && progress >= colorMarkerNext.Position)
                {
                    colorMarkerPrev = colorMarkerNext;
                    colorMarkerNext = _colorMarkers[colorMarkerIndex++];
                }
                else
                    break;
            }

            double betweenMarkerProgress = (progress - colorMarkerPrev.Position) / (colorMarkerNext.Position - colorMarkerPrev.Position);
            Color inCircleColor;
            MixColors(colorMarkerPrev.Color, colorMarkerNext.Color, (float)betweenMarkerProgress, out inCircleColor);
            SolidColorBrush brush = new SolidColorBrush(inCircleColor);
            _gradientCircleProgressMarker.Fill = brush;
            _gradientCircleProgressMarker.Stroke = brush;
        }

        /// <summary>
        /// Refreshes canvas size.
        /// </summary>
        protected virtual void RefreshSizes()
        {
            // Canvas

            _canvasW = _canvas.ActualWidth;
            _canvasH = _canvas.ActualHeight;
            _canvasWDiv2 = _canvasW / 2.0;
            _canvasHDiv2 = _canvasH / 2.0;
            _canvasHDiv4 = _canvasH / 4.0;

            // Radiuses

            CalculateRadiusesForGradientCircle(out _gradientCircleInnerRadius, out _gradientCircleOuterRadius);
            CalculateRadiusesForGradientCircleColorMarkersTrack(out _gradientCircleColorMarkersTrackInnerRadius, out _gradientCircleColorMarkersTrackOuterRadius);
            CalculateRadiusesForGradientCircleProgressMarkersTrack(out _gradientCircleProgressMarkerTrackInnerRadius, out _gradientCircleProgressMarkerTrackOuterRadius);

            //
        }

        protected virtual void CalculateRadiusesForGradientCircle(out double innerRadius, out double outerRadius)
        {
            outerRadius = _canvasWDiv2 - __GRADIENT_CIRCLE_MARGIN;
            innerRadius = outerRadius - __GRADIENT_CIRCLE_WIDTH;
        }

        protected virtual void CalculateRadiusesForGradientCircleColorMarkersTrack(out double innerRadius, out double outerRadius)
        {
            innerRadius = _canvasWDiv2 - __GRADIENT_CIRCLE_MARGIN + __GRADIENT_CIRCLE_COLOR_MARKER_MARGIN;
            outerRadius = innerRadius + __GRADIENT_CIRCLE_COLOR_MARKER_WIDTH;
        }

        protected virtual void CalculateRadiusesForGradientCircleProgressMarkersTrack(out double innerRadius, out double outerRadius)
        {
            outerRadius = _canvasWDiv2 - __GRADIENT_CIRCLE_MARGIN - __GRADIENT_CIRCLE_WIDTH - __GRADIENT_CIRCLE_PROGRESS_MARKER_MARGIN;
            innerRadius = outerRadius - __GRADIENT_CIRCLE_PROGRESS_MARKER_WIDTH;
        }

        protected virtual void CreateCircularBar(Polygon circularBar, double angle, int resolution, double innerRadius, double outerRadius)
        {
            double initAngle = angle / 2.0;
            double angleStep = angle / (double)(resolution - 1);

            double innerX;
            double innerY;
            double outerX;
            double outerY;

            circularBar.Points.Clear();

            Point[] outerPoints = new Point[resolution];
            for (int pIx = 0; pIx < resolution; ++pIx)
            {
                CalculateCircularBarPoints(ref initAngle, angleStep, innerRadius, outerRadius, out innerX, out innerY, out outerX, out outerY);

                circularBar.Points.Add(new Point(innerX, innerY));
                outerPoints[pIx] = new Point(outerX, outerY);
            }
            // This is becouse outer points have to be added with inverted order to create correct polygon.
            for (int pIx = resolution - 1; pIx >= 0; --pIx)
                circularBar.Points.Add(outerPoints[pIx]);
        }

        protected virtual void CalculateCircularBarPoints(
            ref double angle, double angleStep,
            double innerRadius, double outerRadius,
            out double innerX, out double innerY, out double outerX, out double outerY)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            innerX = cos * innerRadius + _canvasWDiv2;
            innerY = -sin * innerRadius + _canvasHDiv2;
            outerX = cos * outerRadius + _canvasWDiv2;
            outerY = -sin * outerRadius + _canvasHDiv2;

            angle -= angleStep;
        }

        protected virtual void MixColors(Color c0, Color c1, float ratio, out Color mixedColor)
        {
            byte r;
            byte g;
            byte b;
            MixColorsComponent(c0.R, c1.R, ratio, out r);
            MixColorsComponent(c0.G, c1.G, ratio, out g);
            MixColorsComponent(c0.B, c1.B, ratio, out b);
            mixedColor = new Color();
            mixedColor.A = Byte.MaxValue;
            mixedColor.R = r;
            mixedColor.G = g;
            mixedColor.B = b;
        }

        protected virtual void MixColorsComponent(byte c0Component, byte c1Component, float ratio, out byte mixedColorComponent)
        {
            float maxValueOfByteAsFloat = (float)Byte.MaxValue;
            float c0CompAsRatio = c0Component / maxValueOfByteAsFloat;
            float c1CompAsRatio = c1Component / maxValueOfByteAsFloat;
            float mcCompAsRatio = c0CompAsRatio * (1f - ratio) + c1CompAsRatio * ratio;
            mixedColorComponent = (byte)Math.Round(maxValueOfByteAsFloat * mcCompAsRatio);
        }

        private void GlobalMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _grabbedMarkerIndex = -1;
            _grabbedProgressMarker = false;
        }

        private void TryToMoveGrabbedColorMarker()
        {
            if (_grabbedMarkerIndex > 0 && _grabbedMarkerIndex < _colorMarkers.Count - 1)
            {
                // CALCULATE NEW ANGLE AND CONVERT NEW ANGLE TO EXACT ANGULAR POSITION

                double angle = CalculateGradientCircleAngle();
                byte positionAsTimePoint = CalculateGradientCircleExactAngularPosition(angle, Byte.MaxValue, b => (double)b, b => (byte)b);

                // CHECK IF POSITION IS CORRECT AND IF IT IS
                // APPLAY CALCULATED ANGLE TO MARKER POLYGON
                // BY UPDATING MARKER POSITION AND READING POSITION BACK
                if (positionAsTimePoint > _colorMarkers[_grabbedMarkerIndex - 1].TimePoint &&
                    positionAsTimePoint < _colorMarkers[_grabbedMarkerIndex + 1].TimePoint)
                {
                    if (MarkerMoved != null)
                    {
                        MarkerMoved(new ColorMultisliderEventArgs() { MarkerIndex = (byte)_grabbedMarkerIndex, MarkerTimePoint = positionAsTimePoint });
                    }
                }
            }
        }

        private void TryToMoveGrabbedProgressMarker()
        {
            if (_grabbedProgressMarker)
            {
                // CALCULATE NEW ANGLE AND CONVERT NEW ANGLE TO EXACT ANGULAR POSITION

                double angle = CalculateGradientCircleAngle();
                UInt16 positionAsTimeProgress = CalculateGradientCircleExactAngularPosition(angle, UInt16.MaxValue, b => (double)b, b => (UInt16)b);

                //

                if (ProgressMarkerMoved != null)
                {
                    ProgressMarkerMoved(new ColorMultisliderEventArgs() { TimeProgress = positionAsTimeProgress });
                }
            }
        }

        private TExactPosition CalculateGradientCircleExactAngularPosition<TExactPosition>(double angle, TExactPosition maxValue,
            Func<TExactPosition, double> toDoubleConverter, Func<double, TExactPosition> toExactPositionConverter)
            where TExactPosition : struct, IComparable, IConvertible
        {
            TExactPosition exactPosition = toExactPositionConverter(Math.Round(toDoubleConverter(maxValue) * angle / __FULL_CIRCLE_DEGREES));
            return exactPosition;
        }

        private double CalculateGradientCircleAngle()
        {
            Point mousePosition = Mouse.GetPosition(_canvas);
            Vector mouseVector = new Vector(mousePosition.X - _canvasWDiv2, mousePosition.Y - _canvasHDiv2);
            Vector zeroAngleVector = new Vector(0, 1);
            mouseVector.Normalize();
            double angle = -Vector.AngleBetween(mouseVector, zeroAngleVector);
            if (angle < 0)
                angle = __FULL_CIRCLE_DEGREES + angle;
            return angle;
        }

        #endregion

        #region IColorMultislider members

        /// <summary>
        /// Gets all markers.
        /// </summary>
        /// <returns></returns>
        public virtual List<SCSColorMarker> GetMarkers()
        {
            return _colorMarkers.ToList();
        }

        /// <summary>
        /// Set markers.
        /// </summary>
        /// <param name="markers">Markers.</param>
        public virtual void SetMarkers(List<SCSColorMarker> markers)
        {
            _colorMarkers.Clear();

            foreach (SCSColorMarker marker in markers)
                _colorMarkers.Add(marker);

            RefreshCanvasObjects();
        }

        /// <summary>
        /// Sets color marker color.
        /// </summary>
        /// <param name="markerIndex">Index of color marker.</param>
        /// <param name="color">Color.</param>
        public virtual void SetMarkerColor(byte markerIndex, Color color)
        {
            RefreshGradientCircle();

            if (markerIndex < _colorMarkers.Count)
            {
                _colorMarkers[markerIndex].Color = color;
                RefreshGradientCircleColorMarkerColorAndPosition(markerIndex);
            }
        }

        /// <summary>
        /// Set progress.
        /// </summary>
        /// <param name="progress"></param>
        public virtual void SetProgress(double progress)
        {
            RefreshGradientCircleProgressMarkerColorAndPosition(progress);
        }

        /// <summary>
        /// Occures on color marker selection action.
        /// </summary>
        public event Action<ColorMultisliderEventArgs> MarkerSelected;

        /// <summary>
        /// Occures on color marker move action.
        /// </summary>
        public event Action<ColorMultisliderEventArgs> MarkerMoved;

        /// <summary>
        /// Occures on progress marker move action.
        /// </summary>
        public event Action<ColorMultisliderEventArgs> ProgressMarkerMoved;

        #endregion
    }
}
