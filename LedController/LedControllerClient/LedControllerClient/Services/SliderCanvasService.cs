using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LedControllerClient.Services
{
    public class SliderCanvasService : IColorMultislider
    {
        #region Constants

        private const int __MIN_MARKER_COUNT = 3;
        private const double __MARKER_HEIGHT = 14;
        private const double __MARKER_WIDTH = 12;
        private const int __MARKER_POINT_COUNT = 3;
        private const int __GRADIENT_BAR_POINT_COUNT = 4;

        #endregion

        #region Attributes

        private double _canvasW;
        private double _canvasH;

        private double _canvasWDiv2;
        private double _canvasHDiv2;

        private double _barHeight;

        private List<SCSColorMarker> _colorMarkers;
        private List<Polygon> _colorMarkerPolys;
        private Polygon _gradientBarPoly;

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
                _canvas.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(_canvas_MouseLeftButtonDown);
                _canvas.MouseMove += new System.Windows.Input.MouseEventHandler(_canvas_MouseMove);

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

        void _canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        void _canvas_MouseMove(object sender, MouseEventArgs e)
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Inits.
        /// </summary>
        public virtual void Init()
        {
            InitGradientBar();
            InitColorMarkers();
        }

        /// <summary>
        /// Inits gradient bar.
        /// </summary>
        protected virtual void InitGradientBar()
        {
            if (_gradientBarPoly != null && _canvas.Children.Contains(_gradientBarPoly))
                _canvas.Children.Remove(_gradientBarPoly);

            _gradientBarPoly = new Polygon();
            for (int pIx = 0; pIx < __GRADIENT_BAR_POINT_COUNT; ++pIx)
                _gradientBarPoly.Points.Add(new Point());
            _canvas.Children.Add(_gradientBarPoly);
        }

        /// <summary>
        /// Inits color markers.
        /// </summary>
        protected virtual void InitColorMarkers()
        {
            _colorMarkers = new List<SCSColorMarker>();
            _colorMarkerPolys = new List<Polygon>();

            AddSCSColorMarker(Colors.White, 0.0);
            AddColorMarkerPoly();
            AddSCSColorMarker(Colors.Red, 0.5);
            AddColorMarkerPoly();
            AddSCSColorMarker(Colors.White, 1.0);
            AddColorMarkerPoly();
        }

        /// <summary>
        /// Refreshes canvas objects.
        /// </summary>
        protected virtual void RefreshCanvasObjects()
        {
            RefreshGradientBar();
            RefreshMarkerPolygons();
        }

        /// <summary>
        /// Refreshes gradient bar.
        /// </summary>
        protected virtual void RefreshGradientBar()
        {
            _gradientBarPoly.Points[0] = new Point(0, 0);
            _gradientBarPoly.Points[1] = new Point(_canvasW, 0);
            _gradientBarPoly.Points[2] = new Point(_canvasW, _canvasH);
            _gradientBarPoly.Points[3] = new Point(0, _canvasH);

            LinearGradientBrush gradientBrush = new LinearGradientBrush();
            gradientBrush.StartPoint = new Point(0, 0);
            gradientBrush.EndPoint = new Point(1.0, 0);
            foreach (SCSColorMarker cm in _colorMarkers)
                gradientBrush.GradientStops.Add(new GradientStop(cm.Color, cm.Position));

            _gradientBarPoly.Fill = gradientBrush;
            _gradientBarPoly.Stroke = new SolidColorBrush(Colors.Black);
            _gradientBarPoly.StrokeThickness = 1;
        }

        /// <summary>
        /// Refreshes all color marker polygons.
        /// </summary>
        protected virtual void RefreshMarkerPolygons()
        {
            int cnt = _colorMarkers.Count;
            for (int cmIx = 0; cmIx < cnt; ++cmIx)
            {
                SetMarkerPolygonData(_colorMarkerPolys[cmIx], _colorMarkers[cmIx]);
            }
        }

        /// <summary>
        /// Sets marker polygon data.
        /// </summary>
        /// <param name="polygon">Polygon.</param>
        /// <param name="marker">Marker data.</param>
        protected virtual void SetMarkerPolygonData(Polygon polygon, SCSColorMarker marker)
        {
            if (polygon.Points.Count != __MARKER_POINT_COUNT)
            {
                polygon.Points.Clear();
                for (int pIx = 0; pIx < __MARKER_POINT_COUNT; ++pIx)
                    polygon.Points.Add(new Point());
            }

            polygon.Points[0] = new Point(_canvasW * marker.Position, _canvasH - __MARKER_HEIGHT);
            polygon.Points[1] = new Point(_canvasW * marker.Position + __MARKER_WIDTH / 2, _canvasH);
            polygon.Points[2] = new Point(_canvasW * marker.Position - __MARKER_WIDTH / 2, _canvasH);

            polygon.Fill = new SolidColorBrush(marker.Color);
            polygon.Stroke = new SolidColorBrush(Colors.Black);
            polygon.StrokeThickness = 1;
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

            // Others

            _barHeight = _canvasH - __MARKER_HEIGHT;
        }

        /// <summary>
        /// Adds new <see cref="SCSColorMarker"/>.
        /// </summary>
        /// <param name="color">Color.</param>
        /// <param name="position">Position.</param>
        protected virtual void AddSCSColorMarker(Color color, double position)
        {
            SCSColorMarker newMarker = new SCSColorMarker();
            newMarker.Color = color;
            newMarker.Position = position;
            _colorMarkers.Add(newMarker);
        }

        /// <summary>
        /// Addds new polygon for color marker.
        /// </summary>
        protected virtual void AddColorMarkerPoly()
        {
            Polygon poly = new Polygon();
            _colorMarkerPolys.Add(poly);
            _canvas.Children.Add(poly);
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
        /// Adds color marker at the end of marker list.
        /// </summary>
        public virtual void AddMarker()
        {
            if (_colorMarkers.Count < __MIN_MARKER_COUNT)
                throw new InvalidOperationException("Cannot add marker becouse of lack of boundary markers. Call Init() first.");

            foreach (SCSColorMarker cm in _colorMarkers)
                cm.Position *= (_colorMarkers.Count - 1) / (double)_colorMarkers.Count;

            AddSCSColorMarker(_colorMarkers[0].Color, 1.0);
            AddColorMarkerPoly();

            RefreshCanvasObjects();
        }

        /// <summary>
        /// Removes color marker at the end of marker list.
        /// </summary>
        public virtual void RemoveMarker()
        {
            if (_colorMarkers.Count > __MIN_MARKER_COUNT)
            {
                _colorMarkers.RemoveAt(_colorMarkers.Count - 1);

                Polygon poly = _colorMarkerPolys[_colorMarkerPolys.Count - 1];
                _colorMarkerPolys.Remove(poly);
                _canvas.Children.Remove(poly);

                // Move last marker at the end of timeline.
                _colorMarkers[_colorMarkers.Count - 1].Position = 1.0;
                _colorMarkers[_colorMarkers.Count - 1].Color = _colorMarkers[0].Color;
            }

            RefreshCanvasObjects();
        }

        /// <summary>
        /// Sets color marker color.
        /// </summary>
        /// <param name="markerIndex">Index of color marker.</param>
        /// <param name="color">Color.</param>
        public virtual void SetMarkerColor(int markerIndex, Color color)
        {
            if (markerIndex < _colorMarkers.Count)
                _colorMarkers[markerIndex].Color = color;

            RefreshCanvasObjects();
        }

        /// <summary>
        /// Occures on color marker selection action.
        /// </summary>
        public event Action<ColorMultisliderEventArgs> MarkerSelected;

        /// <summary>
        /// Occures on color marker move action.
        /// </summary>
        public event Action<ColorMultisliderEventArgs> MarkerMoved;

        #endregion
    }
}
