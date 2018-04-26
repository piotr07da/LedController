
namespace LedController2Client
{
    public enum MessageType : byte
    {
        Undefined = 0,
        AddMarker = 1,
        RemMarker = 2,
        SetMarker = 3,
        GetMarker = 4,
        GetMarkerCount = 5,
        SetTimeSpan = 6,
        GetTimeSpan = 7,
        SetTimeProgress = 8,
        GetTimeProgress = 9,
        PauseOn = 10,
        PauseOff = 11,
        SoundOn = 12,
        SoundOff = 13,
        TurnOn = 14,
        TurnOff = 15,
        GetSystemStateFlags = 16,
    }
}
