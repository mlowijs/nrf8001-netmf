using System;
using Microsoft.SPOT;
using Nrf8001Lib.Commands;

namespace Nrf8001Lib.Events
{
    public class CommandResponseEvent : AciEvent
    {
        public AciOpCode Command
        {
            get { return (AciOpCode)Data[1]; }
        }

        public AciStatusCode StatusCode
        {
            get { return (AciStatusCode)Data[2]; }
        }

        public CommandResponseEvent(byte[] data)
            : base(data)
        {
        }
    }
}
