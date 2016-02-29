using System;

namespace Nrf8001Lib.Events
{
    public class DataReceivedEvent : AciEvent
    {
        public byte ServicePipeId
        {
            get { return Content[1]; }
        }
        public byte[] Data { get; private set; }

        public DataReceivedEvent(byte[] content)
            : base(content)
        {
            Data = new byte[content.Length - 2];

            Array.Copy(content, 2, Data, 0, Data.Length);
        }
    }
}
