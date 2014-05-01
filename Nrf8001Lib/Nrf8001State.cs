
namespace Nrf8001Lib
{
    public enum Nrf8001State : byte
    {
        // Section 26.1.3
        Test = 0x01,
        Setup = 0x02,
        Standby = 0x03,

        // Custom to provide more information about the device state
        Unknown = 0x00,
        Resetting = 0xFF,

        Sleep = 0x11,
        Connected = 0x12
    }
}
