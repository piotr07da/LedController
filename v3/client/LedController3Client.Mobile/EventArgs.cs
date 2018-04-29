using System;
using System.Collections.Generic;
using System.Text;

namespace LedController3Client.Mobile
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
