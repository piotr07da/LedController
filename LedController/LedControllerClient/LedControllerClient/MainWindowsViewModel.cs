using LedControllerClient.SerialCommunication;
using LedControllerClient.Services;
using SimpleMvvmToolkit;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Media;

namespace LedControllerClient
{
    public class MainWindowsViewModel : ViewModelBase<MainWindowsViewModel>
    {
        public MainWindowsViewModel()
        {
            _sliderCanvasService = new SliderCanvasService();

            InitCommands();

            _transiver = new SerialTransiver();
            _transiver.Init("COM5", 4800);
            _transiver.DataReceived += _transiver_DataReceived;
        }

        #region Attributes

        SerialTransiver _transiver = new SerialTransiver();

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

        private int _r;
        public int R
        {
            get { return _r; }
            set
            {
                _r = value;
                NotifyPropertyChanged(vm => vm.R);
            }
        }

        private int _g;
        public int G
        {
            get { return _g; }
            set
            {
                _g = value;
                NotifyPropertyChanged(vm => vm.G);
            }
        }

        private int _b;
        public int B
        {
            get { return _b; }
            set
            {
                _b = value;
                NotifyPropertyChanged(vm => vm.B);
            }
        }

        private int _tmpMarkerIndex;
        public int TmpMarkerIndex
        {
            get { return _tmpMarkerIndex; }
            set
            {
                _tmpMarkerIndex = value;
                NotifyPropertyChanged(vm => vm.TmpMarkerIndex);
            }
        }

        private int _timeLineTimeSpan;
        public int TimeLineTimeSpan
        {
            get { return _timeLineTimeSpan; }
            set
            {
                _timeLineTimeSpan = value;
                NotifyPropertyChanged(vm => vm.TimeLineTimeSpan);

                byte[] data = new byte[] { (byte)'<', (byte)MessageType.SetTimeSpan, 2, (byte)(_timeLineTimeSpan & 0xFF), (byte)((_timeLineTimeSpan >> 8) & 0xFF) };
                SendData(data);
            }
        }

        private int _timeLineTimeProgress;
        public int TimeLineTimeProgress
        {
            get { return _timeLineTimeProgress; }
            set
            {
                _timeLineTimeProgress = value;
                NotifyPropertyChanged(vm => vm.TimeLineTimeProgress);

                byte[] data = new byte[] { (byte)'<', (byte)MessageType.SetTimeProgress, 2, (byte)(_timeLineTimeProgress & 0xFF), (byte)((_timeLineTimeProgress >> 8) & 0xFF) };
                SendData(data);
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

        #endregion

        #region Commands

        public DelegateCommand AddColorMarkerCommand { get; set; }
        public DelegateCommand RemColorMarkerCommand { get; set; }
        public DelegateCommand TmpSubmitColorCommand { get; set; }

        #endregion

        #region Commands actions

        private void AddColorMarkerAction()
        {
            _sliderCanvasService.AddMarker();

            byte[] data = new byte[] { (byte)'<', 1, 1, 0 };
            SendData(data);
        }

        private void RemColorMarkerAction()
        {
            _sliderCanvasService.RemoveMarker();

            byte[] data = new byte[] { (byte)'<', 2, 1, 0 };
            SendData(data);
        }

        private void TmpSubmitColorAction()
        {
            Color color = Color.FromArgb(255, (byte)R, (byte)G, (byte)B);
            _sliderCanvasService.SetMarkerColor(TmpMarkerIndex, color);

            List<SCSColorMarker> markers = _sliderCanvasService.GetMarkers();
            SCSColorMarker marker = markers[TmpMarkerIndex];

            byte[] data = new byte[] { (byte)'<', 3, 5, (byte)TmpMarkerIndex, (byte)(marker.Position * 255), (byte)R, (byte)G, (byte)B };
            SendData(data);

        }

        private void SendData(byte[] data)
        {
            DateTime dt0;
            for (int bIx = 0; bIx < data.Length; ++bIx)
            {
                _responseReceived = false;
                dt0 = DateTime.Now;
                _transiver.Send(new byte[] { data[bIx] });
                while (!_responseReceived)
                {
                    TimeSpan timeElapsed = DateTime.Now - dt0;
                    if (timeElapsed.TotalMilliseconds > 3000)
                        break;
                }

                if (!_responseReceived)
                    break;
            }
        }

        #endregion

        #region Event handling

        private bool _responseReceived;

        void _transiver_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            _responseReceived = true;
            UCStatus += (sender as SerialPort).ReadExisting();
        }

        #endregion

        #region Methods

        public void InitCommands()
        {
            AddColorMarkerCommand = new DelegateCommand(AddColorMarkerAction);
            RemColorMarkerCommand = new DelegateCommand(RemColorMarkerAction);
            TmpSubmitColorCommand = new DelegateCommand(TmpSubmitColorAction);
        }

        #endregion
    }
}
