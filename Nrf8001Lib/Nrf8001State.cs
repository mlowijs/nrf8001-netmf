
namespace Nrf8001Lib
{
    public enum Nrf8001State : byte
    {
        Test = 1,
        Setup = 2,
        Standby = 3,

        // Custom
        Unknown = 0,
        Resetting = 255,
    }
}
