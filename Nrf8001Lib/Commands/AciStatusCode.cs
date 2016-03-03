
namespace Nrf8001Lib.Commands
{
    public enum AciStatusCode : byte
    {
        Success = 0x00,
        TransactionContinue = 0x01,
        TransactionComplete = 0x02,

        BondRequired = 0x8D
    }
}
