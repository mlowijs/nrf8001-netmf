
namespace Nrf8001Lib.Events
{
    public class DeviceStartedEvent : AciEvent
    {
        public Nrf8001State State
        {
            get { return (Nrf8001State)Data[1]; }
        }

        public byte DataCreditsAvailable
        {
            get { return Data[3]; }
        }

        public DeviceStartedEvent(byte[] data)
            : base(data)
        {
        }
    }
}
