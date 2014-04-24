
namespace Nrf8001Lib.Events
{
    public class Nrf8001Event
    {
        public Nrf8001EventType EventType { get; protected set; }
        public byte[] Data { get; protected set; }

        public Nrf8001Event(byte[] data)
        {
            EventType = (Nrf8001EventType)data[0];
            Data = data;
        }
    }
}
