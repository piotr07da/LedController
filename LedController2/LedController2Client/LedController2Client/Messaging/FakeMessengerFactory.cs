
namespace LedController2Client
{
    public class FakeMessengerFactory : IMessengerFactory
    {
        public IMessenger CreateMessenger(SerialCommunication.ITransiver transiver)
        {
            return new FakeMessenger();
        }
    }
}
