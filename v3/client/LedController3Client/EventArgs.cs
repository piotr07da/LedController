
namespace LedController3Client
{
    public class EventArgs<TData>
    {
        public EventArgs()
        {

        }

        public EventArgs(TData data)
        {
            Data = data;
        }

        public TData Data { get; set; }
    }
}
