using System;
using Microsoft.SPOT;

namespace Nrf8001Lib.Extensions
{
    public static class ByteArrayExtensions
    {
        public static ulong ToUnsignedLong(this byte[] bytes, int start, int length = 8)
        {
            ulong result = 0;

            for (int i = 0; i < length; i++)
            {
                result |= (ulong)((ulong)bytes[start + i] << i * 8);
            }

            return result;
        }
    }
}
