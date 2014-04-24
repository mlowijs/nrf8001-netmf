using Microsoft.SPOT.Hardware;

namespace Nrf8001Lib.Extensions
{
    public static class SPIExtensions
    {
        /// <summary>
        /// Writes a block of data to the interface, least significant bit first.
        /// </summary>
        /// <param name="spi">The SPI interface to write to.</param>
        /// <param name="writeBuffer">The block of data to write.</param>
        public static void WriteLsb(this SPI spi, byte[] writeBuffer)
        {
            for (var i = 0; i < writeBuffer.Length; i++)
                writeBuffer[i] = InvertByte(writeBuffer[i]);

            spi.Write(writeBuffer);
        }

        /// <summary>
        /// Writes a block of data to the interface, and reads a block of data from the interface into the read buffer, least significant bit first.
        /// </summary>
        /// <param name="spi">The SPI interface to write to.</param>
        /// <param name="writeBuffer">The block of data to write.</param>
        /// <param name="readBuffer">The buffer to read into.</param>
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
