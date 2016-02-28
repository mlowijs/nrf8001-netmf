namespace Nrf8001Lib.Events
{
    public class DataCreditEvent : AciEvent
    {
        public byte DataCreditsAvailable
        {
            get { return Data[1]; }
        }

        public DataCreditEvent(byte[] data)
            : base(data)
        {
        }
    }
}
