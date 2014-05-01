
namespace Nrf8001Lib.Events
{
    public class AciEvent
    {
        public Nrf8001EventType EventType { get; protected set; }
        public byte[] Data { get; protected set; }

        public AciEvent(byte[] data)
        {
            EventType = (Nrf8001EventType)data[0];
            Data = data;
        }
    }
}
