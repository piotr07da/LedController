using System;
using System.Windows.Media;

namespace LedController2Client
{
    public interface IMessenger
    {
        event Action<IMessenger, byte> ByteReceived;
        event Action<IMessenger> AddMarkerCompleted;
        event Action<IMessenger> RemMarkerCompleted;
        event Action<IMessenger> SetMarkerCompleted;
        event Action<IMessenger> SetTimeSpanCompleted;
        event Action<IMessenger> SetTimeProgressCompleted;
        event Action<IMessenger, ColorMarkerResponseMessage> ReadMarkerCompleted;
        event Action<IMessenger, ColorMarkerCountResponseMessage> ReadMarkerCountCompleted;
        event Action<IMessenger, TimeSpanResponseMessage> ReadTimeSpanCompleted;
        event Action<IMessenger, TimeProgressResponseMessage> ReadTimeProgressCompleted;
        event Action<IMessenger> TurnOnCompleted;
        event Action<IMessenger> TurnOffCompleted;
        event Action<IMessenger> SoundOnCompleted;
        event Action<IMessenger> SoundOffCompleted;
        event Action<IMessenger, SystemStateFlagsResponseMessage> ReadSystemStateFlagsCompleted;


        void AddMarker();
        void RemMarker();
        void SetMarker(byte markerIndex, byte timePoint, Color color);
        void SetTimeSpan(UInt16 timeSpan);
        void SetTimeProgress(UInt16 timeProgress);
        void LoadMarker(byte markerIndex);
        void LoadMarkerCount();
        //async Task<ColorMarkerCountResponseMessage> LoadMarkerCount();
        void LoadTimeSpan();
        void LoadTimeProgress();
        void TurnOn();
        void TurnOff();
        void SoundOn();
        void SoundOff();
        void LoadSystemStateFlags();
    }
}
