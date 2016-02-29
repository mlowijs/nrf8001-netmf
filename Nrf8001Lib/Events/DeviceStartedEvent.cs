
namespace Nrf8001Lib.Events
{
    public class DeviceStartedEvent : AciEvent
    {
        public Nrf8001State State
        {
            get { return (Nrf8001State)Content[1]; }
        }

        public byte DataCreditsAvailable
        {
            get { return Content[3]; }
        }

        public DeviceStartedEvent(byte[] content)
            : base(content)
        {
        }
    }
}
