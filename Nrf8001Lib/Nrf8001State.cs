
namespace Nrf8001Lib
{
    public enum Nrf8001State : byte
    {
        Test = 0x01,
        Setup = 0x02,
        Standby = 0x03,

        // Custom
        Unknown = 0x00,
        Resetting = 0xFF,
    }
}
