
namespace LedController2Client.SerialCommunication
{
    public class FakeTransiverFactory : ITransiverFactory
    {
        public ITransiver CreateTransiver()
        {
            return new FakeTransiver();
        }
    }
}
