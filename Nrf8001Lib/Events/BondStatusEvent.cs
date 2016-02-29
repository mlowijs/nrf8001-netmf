using System;
using Microsoft.SPOT;
using Nrf8001Lib.Commands;

namespace Nrf8001Lib.Events
{
    public class BondStatusEvent : AciEvent
    {
        public BondStatusCode StatusCode
        {
            get { return (BondStatusCode)Content[1]; }
        }

        public BondStatusEvent(byte[] content)
            : base(content)
        {
        }
    }
}
