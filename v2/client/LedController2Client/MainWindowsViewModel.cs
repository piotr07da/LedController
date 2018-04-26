using LedController2Client.SerialCommunication;
using LedController2Client.Services;
using SimpleMvvmToolkit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media;

namespace LedController2Client
{
    public class MainWindowsViewModel : ViewModelBase<MainWindowsViewModel>
    {
        #region Constants

        private const int __MIN_MARKER_COUNT = 2;
        private const double __TIME_SPAN_SCALE = 5;
        private const double __four1_PI2 = 6.28318530717959;
        private const double PI = 3.1415926535897932;
        private const double __FIRST_OCTAVE_LEFT_FREQUENCY = 11.3;
        private const string __COLOR_TEXT_DATA_FORMAT = "#{0:x2}{1:x2}{2:x2}";

        #endregion

        public MainWindowsViewModel()
        {
            SelectedMarkerIndex = -1;

            _sliderCanvasService = new SliderCanvasService();
            _sliderCanvasService.MarkerSelected += _sliderCanvasService_MarkerSelected;
            _sliderCanvasService.MarkerMoved += _sliderCanvasService_MarkerMoved;
            _sliderCanvasService.ProgressMarkerMoved += _sliderCanvasService_ProgressMarkerMoved;

            InitCommands();

#if FAKEDEVICE
            _transiverFactory = new FakeTransiverFactory();
            _messengerFactory = new FakeMessengerFactory();
#else
            _transiverFactory = new SerialTransiverFactory("COM9", 230400);
            _messengerFactory = new MessengerFactory();
#endif
            _transiver = _transiverFactory.CreateTransiver();
            _transiver.Opened += _transiver_Opened;
            _transiver.Closed += _transiver_Closed;
            _messenger = _messengerFactory.CreateMessenger(_transiver);
            _messenger.ByteReceived += _messenger_ByteReceived;
            _messengerWorker = new MessengerWorker(_messenger);
            _messengerWorker.AddMarkerCompleted += _messengerWorker_AddMarkerCompleted;
            _messengerWorker.RemMarkerCompleted += _messengerWorker_RemMarkerCompleted;
            _messengerWorker.SetMarkerCompleted += _messengerWorker_SetMarkerCompleted;
            _messengerWorker.SetTimeSpanCompleted += _messengerWorker_SetTimeSpanCompleted;
            _messengerWorker.SetTimeProgressCompleted += _messengerWorker_SetTimeProgressCompleted;
            _messengerWorker.ByteReceived += _messengerWorker_ByteReceived;
            _messengerWorker.ReadMarkerCompleted += _messengerWorker_ReadMarkerCompleted;
            _messengerWorker.ReadMarkerCountCompleted += _messengerWorker_ReadMarkerCountCompleted;
            _messengerWorker.ReadTimeSpanCompleted += _messengerWorker_ReadTimeSpanCompleted;
            _messengerWorker.ReadTimeProgressCompleted += _messengerWorker_ReadTimeProgressCompleted;
            _messengerWorker.ReadSystemStateFlagsCompleted += _messengerWorker_ReadSystemStateFlagsCompleted;

            //_audioDataReceiver = new AudioDataReceiver(1, 128, 64, 8);
            _audioDataReceiver = new AudioDataReceiver(1, 64, 32, 7);
            _audioDataReceiver.DataReceived += _audioDataReceiver_DataReceived;
            SpectrumSnapshotCommand = new DelegateCommand(SpectrumSnapshotAction);

            if (IsInDesignMode)
                _colorSchemeResource = new FakeColorSchemeResource();
            else
                _colorSchemeResource = new ColorSchemeResource();

            ColorSchemeCategories = new ObservableCollection<ColorSchemeCategory>(_colorSchemeResource.GetCategories());
        }

        void _messenger_ByteReceived(IMessenger messenger, byte obj)
        {
            //UCStatus += obj.ToString() + ",";
        }

        private Stopwatch _sw = new Stopwatch();
        private double _totalMs;
        private byte[] _audioData;



        void _audioDataReceiver_DataReceived(byte[] samples, byte[] spectrum, byte[] octaves)
        {
            _sw.Stop();
            _totalMs = _sw.ElapsedMilliseconds;
            _sw.Restart();
            UIDispatcher.Current.BeginInvoke(new Action(() =>
            {
                //_audioData = samples;
                //_sliderCanvasService.DrawAudioData(samples, spectrum, octaves);
            }));
        }

        private AudioDataReceiver _audioDataReceiver;


        public DelegateCommand SpectrumSnapshotCommand { get; set; }
        private void SpectrumSnapshotAction()
        {
            if (_audioData == null)
                return;

            StringBuilder codeBuilder = new StringBuilder(1024);
            codeBuilder.Append("byte[] testdata = new byte[] { ");
            for (int ix = 0; ix < _audioData.Length; ++ix)
            {
                codeBuilder.Append(_audioData[ix] + ",");
            }
            codeBuilder.Append("}");
            UCStatus = codeBuilder.ToString();
        }

        #region Attributes

        private ITransiverFactory _transiverFactory;
        private ITransiver _transiver;
        private IMessengerFactory _messengerFactory;
        private IMessenger _messenger;
        private IMessengerWorker _messengerWorker;
        private IColorSchemeResource _colorSchemeResource;

        private bool _loadMarkersAfterMarkerCountLoaded;
        private byte _markersToLoadCount;
        private List<SCSColorMarker> _loadedMarkers;

        #endregion

        #region Bindable properties

        private SliderCanvasService _sliderCanvasService;
        public SliderCanvasService SliderCanvasService
        {
            get { return _sliderCanvasService; }
            set
            {
                _sliderCanvasService = value;
                NotifyPropertyChanged(vm => vm.SliderCanvasService);
            }
        }

        private bool _transiverIsOpen;
        public bool TransiverIsOpen
        {
            get { return _transiverIsOpen; }
            set
            {
                _transiverIsOpen = value;
                NotifyPropertyChanged(vm => vm.TransiverIsOpen);
            }
        }

        private byte _markerCount;
        public byte MarkerCount
        {
            get { return _markerCount; }
            set
            {
                _markerCount = value;
                NotifyPropertyChanged(vm => vm.MarkerCount);
            }
        }

        private byte _r;
        public byte R
        {
            get { return _r; }
            set
            {
                SetOnlyR(value);
                SetCurrentMarker();
            }
        }
        public void SetOnlyR(byte r)
        {
            _r = r;
            NotifyPropertyChanged(vm => vm.R);
            UpdateColorTextData();
        }

        private byte _g;
        public byte G
        {
            get { return _g; }
            set
            {
                SetOnlyG(value);
                SetCurrentMarker();
            }
        }
        public void SetOnlyG(byte g)
        {
            _g = g;
            NotifyPropertyChanged(vm => vm.G);
            UpdateColorTextData();
        }

        private byte _b;
        public byte B
        {
            get { return _b; }
            set
            {
                SetOnlyB(value);
                SetCurrentMarker();
            }
        }
        public void SetOnlyB(byte b)
        {
            _b = b;
            NotifyPropertyChanged(vm => vm.B);
            UpdateColorTextData();
        }

        private string _colorTextData;
        public string ColorTextData
        {
            get { return _colorTextData; }
        }
        private void UpdateColorTextData()
        {
            _colorTextData = String.Format(__COLOR_TEXT_DATA_FORMAT, R, G, B);
            NotifyPropertyChanged(vm => vm.ColorTextData);
        }

        private int _selectedMarkerIndex;
        public int SelectedMarkerIndex
        {
            get { return _selectedMarkerIndex; }
            set
            {
                int val = value;
                if (val >= MarkerCount)
                    val = (byte)(MarkerCount - 1);
                _selectedMarkerIndex = val;
                NotifyPropertyChanged(vm => vm.SelectedMarkerIndex);
                UpdateMarkerIsSelected();
            }
        }

        private bool _markerIsSelected;
        public bool MarkerIsSelected
        {
            get { return _markerIsSelected; }
        }
        private void UpdateMarkerIsSelected()
        {
            _markerIsSelected = _loadedMarkers != null && SelectedMarkerIndex < _loadedMarkers.Count;
            NotifyPropertyChanged(vm => vm.MarkerIsSelected);
        }

        private int _timeLineTimeSpan;
        public int TimeLineTimeSpan
        {
            get { return _timeLineTimeSpan; }
            set
            {
                SetOnlyTimeSpan(value);
                SetTimeSpan((ushort)(TimeLineTimeSpan * __TIME_SPAN_SCALE));
            }
        }
        private void SetOnlyTimeSpan(int timeSpan)
        {
            _timeLineTimeSpan = timeSpan;
            NotifyPropertyChanged(vm => vm.TimeLineTimeSpan);
        }

        private int _timeLineTimeProgress;
        public int TimeLineTimeProgress
        {
            get { return _timeLineTimeProgress; }
            set
            {
                _timeLineTimeProgress = value;
                NotifyPropertyChanged(vm => vm.TimeLineTimeProgress);
            }
        }


        private bool? _isSystemEnabled;
        public bool? IsSystemEnabled
        {
            get { return _isSystemEnabled; }
            set
            {
                _isSystemEnabled = value;
                NotifyPropertyChanged(vm => vm.IsSystemEnabled);

                if (value != null && (bool)value)
                    _messengerWorker.TurnOn();
                else
                    _messengerWorker.TurnOff();
            }
        }

        private bool? _isSoundEnabled;
        public bool? IsSoundEnabled
        {
            get { return _isSoundEnabled; }
            set
            {
                _isSoundEnabled = value;
                NotifyPropertyChanged(vm => vm.IsSoundEnabled);

                if (value != null && (bool)value)
                    _messengerWorker.SoundOn();
                else
                    _messengerWorker.SoundOff();
            }
        }



        private string _uCStatus;
        public string UCStatus
        {
            get { return _uCStatus; }
            set
            {
                _uCStatus = value;
                NotifyPropertyChanged(vm => vm.UCStatus);
            }
        }

        private ObservableCollection<ColorSchemeCategory> _colorSchemeCategories;
        public ObservableCollection<ColorSchemeCategory> ColorSchemeCategories
        {
            get { return _colorSchemeCategories; }
            set
            {
                _colorSchemeCategories = value;
                NotifyPropertyChanged(vm => vm.ColorSchemeCategories);

                if (_colorSchemeCategories != null && _colorSchemeCategories.Any())
                    SelectedColorSchemeCategory = _colorSchemeCategories[0];
            }
        }

        private ColorSchemeCategory _selectedColorSchemeCategory;
        public ColorSchemeCategory SelectedColorSchemeCategory
        {
            get { return _selectedColorSchemeCategory; }
            set
            {
                ColorSchemeCategory previousColorSchemeCategory = _selectedColorSchemeCategory;
                _selectedColorSchemeCategory = value;
                NotifyPropertyChanged(vm => vm.SelectedColorSchemeCategory);

                ColorSchemes = new ObservableCollection<ColorScheme>(_colorSchemeResource.GetSchemes(_selectedColorSchemeCategory));
            }
        }

        private ObservableCollection<ColorScheme> _colorSchemes;
        public ObservableCollection<ColorScheme> ColorSchemes
        {
            get { return _colorSchemes; }
            set
            {
                _colorSchemes = value;
                NotifyPropertyChanged(vm => vm.ColorSchemes);

                if (_colorSchemes != null && _colorSchemes.Any())
                    SelectedColorScheme = _colorSchemes[0];
            }
        }

        private ColorScheme _selectedColorScheme;
        public ColorScheme SelectedColorScheme
        {
            get { return _selectedColorScheme; }
            set
            {
                _selectedColorScheme = value;
                NotifyPropertyChanged(vm => vm.SelectedColorScheme);

                LoadCurrentColorScheme();
            }
        }

        #endregion

        #region Commands

        public DelegateCommand SwitchConnectionCommand { get; set; }
        public DelegateCommand AddColorMarkerCommand { get; set; }
        public DelegateCommand RemColorMarkerCommand { get; set; }
        public DelegateCommand TmpSubmitColorCommand { get; set; }

        public DelegateCommand AddColorSchemeCommand { get; set; }
        public DelegateCommand RemColorSchemeCommand { get; set; }

        #endregion

        #region Commands actions

        private void SwitchConnectionAction()
        {
            if (!_transiver.IsOpen)
                _transiver.TryOpen();
            else
                _transiver.Close();

            bool transiverIsOpen = _transiver.IsOpen;

            if (transiverIsOpen)
            {
                _messengerWorker.Start();
                LoadDataFromMCU();
            }
            else
            {
                _messengerWorker.Stop();
            }
        }

        private void AddColorMarkerAction()
        {
            AddColorMarkers(1);
        }

        private void RemColorMarkerAction()
        {
            RemColorMarkers(1);
        }

        private void TmpSubmitColorAction()
        {
            //byte timePoint;
            //if (TmpMarkerIndex == 0)
            //    timePoint = 0;
            //else if (TmpMarkerIndex == 1)
            //    timePoint = 127;
            //else
            //    timePoint = 255;

            //_messengerWorker.SetMarker(TmpMarkerIndex, timePoint, Color.FromRgb(R, G, B));
            //LoadMarkers();
        }

        public void AddColorSchemeAction()
        {
            if (SelectedColorSchemeCategory != null)
            {
                var scheme = _colorSchemeResource.NewScheme(SelectedColorSchemeCategory);
                ColorSchemes.Add(scheme);
            }
        }

        public void RemColorSchemeAction()
        {
        }

        #endregion

        #region Event handling

        void _transiver_Opened()
        {
            TransiverIsOpen = true;
        }

        void _transiver_Closed()
        {
            TransiverIsOpen = false;
        }

        void _messengerWorker_ByteReceived(IMessenger sender, byte obj)
        {
            //UCStatus += obj.ToString();
        }

        void _messengerWorker_AddMarkerCompleted(IMessenger obj)
        {
            //
        }

        void _messengerWorker_RemMarkerCompleted(IMessenger obj)
        {
            //
        }

        void _messengerWorker_SetMarkerCompleted(IMessenger obj)
        {
            //
        }

        void _messengerWorker_SetTimeSpanCompleted(IMessenger obj)
        {
            //
        }

        void _messengerWorker_SetTimeProgressCompleted(IMessenger obj)
        {
            //
        }

        void _messengerWorker_ReadMarkerCompleted(IMessenger sender, ColorMarkerResponseMessage obj)
        {
            Color color = Color.FromRgb(obj.R, obj.G, obj.B);

            if (_markersToLoadCount > 0)
            {
                // MODE A: loading collection of markers

                --_markersToLoadCount;

                SCSColorMarker marker = new SCSColorMarker();
                marker.TimePoint = obj.TimePoint;
                marker.Color = color;
                _loadedMarkers.Add(marker);

                if (_markersToLoadCount == 0)
                {
                    UIDispatcher.Current.BeginInvoke(new Action(() =>
                    {
                        _sliderCanvasService.SetMarkers(_loadedMarkers);
                    }));
                }
            }
            else
            {
                // MODE B: loading single marker

                UIDispatcher.Current.BeginInvoke(new Action(() =>
                {
                    _sliderCanvasService.SetMarkerColor(obj.MarkerIndex, color);
                }));
            }
        }

        void _messengerWorker_ReadMarkerCountCompleted(IMessenger sender, ColorMarkerCountResponseMessage obj)
        {
            byte markerCount = obj.MarkerCount;
            MarkerCount = markerCount;

            if (_loadMarkersAfterMarkerCountLoaded)
            {
                _markersToLoadCount = markerCount;
                _loadedMarkers = new List<SCSColorMarker>();

                for (byte markerIx = 0; markerIx < markerCount; ++markerIx)
                {
                    _messengerWorker.LoadMarker(markerIx);
                }

                _loadMarkersAfterMarkerCountLoaded = false;
            }
        }

        void _messengerWorker_ReadTimeSpanCompleted(IMessenger sender, TimeSpanResponseMessage obj)
        {
            SetOnlyTimeSpan((int)(obj.TimeSpan / __TIME_SPAN_SCALE));
        }

        void _messengerWorker_ReadTimeProgressCompleted(IMessenger sender, TimeProgressResponseMessage obj)
        {
            TimeLineTimeProgress = obj.TimeProgress;
            UIDispatcher.Current.BeginInvoke(new Action(() =>
            {
                _sliderCanvasService.SetProgress(obj.TimeProgress / (double)UInt16.MaxValue);
            }));
        }

        void _messengerWorker_ReadSystemStateFlagsCompleted(IMessenger arg1, SystemStateFlagsResponseMessage obj)
        {
            IsSystemEnabled = obj.IsSystemEnabled;
            IsSoundEnabled = obj.IsSoundEnabled;
        }

        void _sliderCanvasService_MarkerSelected(ColorMultisliderEventArgs obj)
        {
            SelectedMarkerIndex = obj.MarkerIndex;
            SetOnlyR(_loadedMarkers[SelectedMarkerIndex].Color.R);
            SetOnlyG(_loadedMarkers[SelectedMarkerIndex].Color.G);
            SetOnlyB(_loadedMarkers[SelectedMarkerIndex].Color.B);
        }

        void _sliderCanvasService_MarkerMoved(ColorMultisliderEventArgs obj)
        {
            _loadedMarkers[SelectedMarkerIndex].TimePoint = obj.MarkerTimePoint;
            SetCurrentMarker();
        }

        void _sliderCanvasService_ProgressMarkerMoved(ColorMultisliderEventArgs obj)
        {
            SetTimeProgress((ushort)obj.TimeProgress);
        }

        #endregion

        #region Methods

        private Thread _timeProgressScaner;

        private void LoadDataFromMCU()
        {
            Thread.Sleep(33);

            LoadMarkers();
            _messengerWorker.LoadTimeSpan();
            _messengerWorker.LoadSystemStateFlags();

            _timeProgressScaner = new Thread(() =>
            {
                while (_messengerWorker.IsRunning)
                {
                    if (_messengerWorker.TaskCount < 3)
                        _messengerWorker.LoadTimeProgress();
                    Thread.Sleep(1);
                }
            });
            _timeProgressScaner.Start();
        }

        private void LoadMarkers()
        {
            _loadMarkersAfterMarkerCountLoaded = true;
            _messengerWorker.LoadMarkerCount();
        }

        private void SetCurrentMarker()
        {
            if (_messengerWorker.TaskCount < 4)
            {
                _messengerWorker.SetMarker((byte)SelectedMarkerIndex, (byte)(_loadedMarkers[SelectedMarkerIndex].Position * 255), Color.FromRgb(R, G, B));
                _messengerWorker.LoadMarker((byte)SelectedMarkerIndex);
                if (SelectedMarkerIndex == 0)
                    _messengerWorker.LoadMarker((byte)(_markerCount - 1));
                else if (SelectedMarkerIndex == (byte)(_markerCount - 1))
                    _messengerWorker.LoadMarker(0);
            }
        }

        private void SetTimeSpan(ushort timeSpan)
        {
            if (_messengerWorker.TaskCount < 4)
            {
                _messengerWorker.SetTimeSpan(timeSpan);
                _messengerWorker.LoadTimeSpan();
            }
        }

        private void SetTimeProgress(ushort timeProgress)
        {
            if (_messengerWorker.TaskCount < 4)
            {
                _messengerWorker.SetTimeProgress(timeProgress);
                _messengerWorker.LoadTimeProgress();
            }
        }

        private void LoadCurrentColorScheme()
        {
            var selectedColorScheme = SelectedColorScheme;
            if (selectedColorScheme != null)
            {
                if (selectedColorScheme.Gradient != null)
                {
                    int diff = selectedColorScheme.Gradient.Length - MarkerCount;
                    int diffAbs = Math.Abs(diff);

                    if (diff > 0)
                        AddColorMarkers(diffAbs);
                    else if (diff < 0)
                        RemColorMarkers(diffAbs);

                    //for (int mIx = 0; mIx < selectedColorScheme.Gradient.Length; ++mIx)
                    //{
                    //    SelectedMarkerIndex = mIx;
                    //    R = selectedColorScheme.Gradient[mIx].R;
                    //    G = selectedColorScheme.Gradient[mIx].G;
                    //    B = selectedColorScheme.Gradient[mIx].B;
                    //}
                }
                else
                {
                    UpdateCurrentColorScheme();
                }
            }
        }

        private void UpdateCurrentColorScheme()
        {
            var selectedColorScheme = SelectedColorScheme;
            if (selectedColorScheme != null)
            {
                selectedColorScheme.Gradient = new ColorMarker[MarkerCount];

                if (MarkerCount >= 2)
                {
                    for (int mIx = 0; mIx < MarkerCount; ++mIx)
                    {
                        SCSColorMarker m = _loadedMarkers[mIx];
                        selectedColorScheme.Gradient[mIx] = new ColorMarker() { R = m.Color.R, G = m.Color.G, B = m.Color.B, TimePoint = m.TimePoint };
                    }
                    //selectedColorScheme.TimeSpan = TimeLineTimeSpan;
                }

                _colorSchemeResource.UpdateScheme(selectedColorScheme);
                NotifyPropertyChanged(vm => vm.SelectedColorScheme);
            }
        }

        private void AddColorMarkers(int count, Action continuation = null)
        {
            if (_messengerWorker.TaskCount < 5)
            {
                for (int i = 0; i < count; ++i)
                    _messengerWorker.AddMarker();

                LoadMarkers();
            }
        }

        private void RemColorMarkers(int count)
        {
            if (_messengerWorker.TaskCount < 5)
            {
                if (count > MarkerCount - __MIN_MARKER_COUNT && MarkerCount >= __MIN_MARKER_COUNT)
                    count = MarkerCount - __MIN_MARKER_COUNT;

                for (int i = 0; i < count; ++i)
                    _messengerWorker.RemMarker();

                LoadMarkers();
            }
        }

        private void InitCommands()
        {
            SwitchConnectionCommand = new DelegateCommand(SwitchConnectionAction);
            AddColorMarkerCommand = new DelegateCommand(AddColorMarkerAction);
            RemColorMarkerCommand = new DelegateCommand(RemColorMarkerAction);
            TmpSubmitColorCommand = new DelegateCommand(TmpSubmitColorAction);
            AddColorSchemeCommand = new DelegateCommand(AddColorSchemeAction);
            RemColorSchemeCommand = new DelegateCommand(RemColorSchemeAction);
        }

        #endregion
    }
}
