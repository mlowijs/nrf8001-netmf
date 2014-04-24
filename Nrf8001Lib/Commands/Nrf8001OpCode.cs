
namespace Nrf8001Lib.Commands
{
    public enum Nrf8001OpCode : byte
    {
        Test = 0x01,
        Echo = 0x02,
        Setup = 0x06,
        Connect = 0x0F
    }
}
