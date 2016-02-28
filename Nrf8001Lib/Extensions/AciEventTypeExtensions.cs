using Nrf8001Lib.Events;

namespace Nrf8001Lib.Extensions
{
    public static class AciEventTypeExtensions
    {
        public static string GetName(this AciEventType eventType)
        {
            switch (eventType)
            {
                case AciEventType.BondStatus:
                    return "BondStatus";

                case AciEventType.CommandResponse:
                    return "CommandResponse";

                case AciEventType.Connected:
                    return "Connected";

                case AciEventType.DataCredit:
                    return "DataCredit";

                case AciEventType.DataReceived:
                    return "DataReceived";

                case AciEventType.DeviceStarted:
                    return "DeviceStarted";

                case AciEventType.Disconnected:
                    return "Disconnected";

                case AciEventType.Echo:
                    return "Echo";

                case AciEventType.PipeError:
                    return "PipeError";

                case AciEventType.PipeStatus:
                    return "PipeStatus";

                case AciEventType.TimingEvent:
                    return "TimingEvent";

                default:
                    return eventType.ToString();
            }
        }
    }
}
