
namespace Nrf8001Lib.Events
{
    public enum Nrf8001EventType : byte
    {
        DeviceStarted = 0x81,
        Echo = 0x82,
        CommandResponse = 0x84,
        Connected = 0x85,
        Disconnected = 0x86,
        PipeStatus = 0x88,
    }
}
