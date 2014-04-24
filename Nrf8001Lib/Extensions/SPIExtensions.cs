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
            InvertBytes(writeBuffer);

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
            InvertBytes(writeBuffer);

            spi.WriteRead(writeBuffer, readBuffer);

            InvertBytes(readBuffer);
        }

        private static void InvertBytes(byte[] bytes)
        {
            // Iterate over all bytes
            for (var i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0 || bytes[i] == 0xFF)
                    continue;

                byte output = 0;

                for (var j = 0; j < 8; j++)
                {
                    output <<= 1;
                    output |= (byte)(bytes[i] & 0x01);
                    bytes[i] >>= 1;
                }

                bytes[i] = output;
            }
        }
    }
}
