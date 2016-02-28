using Nrf8001Lib.Commands;

namespace Nrf8001Lib.Events
{
    public class AciEvent
    {
        public AciEventType EventType { get; private set; }
        public byte[] Data { get; private set; }

        public AciEvent(byte[] data)
        {
            EventType = (AciEventType)data[0];
            Data = data;
        }
    }
}
