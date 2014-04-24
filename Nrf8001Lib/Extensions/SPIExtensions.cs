using Microsoft.SPOT.Hardware;

namespace Nrf8001Lib.Extensions
{
    public static class SPIExtensions
    {
        public static void WriteLsb(this SPI spi, byte[] writeBuffer)
        {
            for (var i = 0; i < writeBuffer.Length; i++)
                writeBuffer[i] = InvertByte(writeBuffer[i]);

            spi.Write(writeBuffer);
        }

        public static void WriteReadLsb(this SPI spi, byte[] writeBuffer, byte[] readBuffer)
        {
            for (var i = 0; i < writeBuffer.Length; i++)
                writeBuffer[i] = InvertByte(writeBuffer[i]);

            spi.WriteRead(writeBuffer, readBuffer);

            for (var i = 0; i < readBuffer.Length; i++)
                readBuffer[i] = InvertByte(readBuffer[i]);
        }

        private static byte InvertByte(byte input)
        {
            if (input == 0)
                return input;

            byte output = 0;

            for (var i = 0; i < 8; i++)
            {
                output <<= 1;
                output |= (byte)(input & 0x01);
                input >>= 1;
            }

            return output;
        }
    }
}
