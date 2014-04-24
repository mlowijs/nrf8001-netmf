
namespace Nrf8001Lib.Commands
{
    public enum Nrf8001AciStatusCode : byte
    {
        Success = 0x00,
        TransactionContinue = 0x01,
        TransactionComplete = 0x02
    }
}
