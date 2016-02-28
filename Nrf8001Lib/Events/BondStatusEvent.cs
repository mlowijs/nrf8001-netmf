using System;
using Microsoft.SPOT;
using Nrf8001Lib.Commands;

namespace Nrf8001Lib.Events
{
    public class BondStatusEvent : AciEvent
    {
        public BondStatusCode StatusCode
        {
            get { return (BondStatusCode)Data[1]; }
        }

        public BondStatusEvent(byte[] data)
            : base(data)
        {
        }
    }
}
