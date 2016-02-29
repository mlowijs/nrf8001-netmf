using Nrf8001Lib.Commands;

namespace Nrf8001Lib.Events
{
    public class AciEvent
    {
        public AciEventType EventType { get; private set; }

        /// <summary>
        /// The event content without the length.
        /// </summary>
        public byte[] Content { get; private set; }

        public AciEvent(byte[] content)
        {
            EventType = (AciEventType)content[0];
            Content = content;
        }
    }
}
