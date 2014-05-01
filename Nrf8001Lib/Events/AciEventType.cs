
namespace Nrf8001Lib.Events
{
    public enum Nrf8001EventType : byte
    {
        DeviceStarted = 0x81,
        Echo = 0x82,
        CommandResponse = 0x84,
        Connected = 0x85,
        Disconnected = 0x86,
        BondStatus = 0x87,
        PipeStatus = 0x88,
        DataCredit = 0x8A,
        DataReceived = 0x8C,
        PipeError = 0x8D
    }
}
