using LedController2Client.SerialCommunication;

namespace LedController2Client
{
    public interface IMessengerFactory
    {
        IMessenger CreateMessenger(ITransiver transiver);
    }
}
