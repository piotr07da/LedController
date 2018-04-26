using LedController2Client.SerialCommunication;

namespace LedController2Client
{
    public class MessengerFactory : IMessengerFactory
    {
        public IMessenger CreateMessenger(ITransiver transiver)
        {
            return new Messenger(transiver);
        }
    }
}
