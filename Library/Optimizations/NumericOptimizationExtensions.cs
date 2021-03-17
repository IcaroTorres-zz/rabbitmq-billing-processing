using System;

namespace Library.Optimizations
{
    public static class NumericOptimizationExtensions
    {
        private static long Parse(this ReadOnlySpan<char> readOnlySpan, int bytes)
        {
            var loop = Math.Min(readOnlySpan.Length, bytes);

            long result = 0;
            for (short i = 0; i < loop; i++)
            {
                result = (result * 10) + (readOnlySpan[i] - '0');
            }
            return result;
        }

        private static bool TryParse(this ReadOnlySpan<char> readOnlySpan, int bytes, out long value)
        {
            try
            {
                value = readOnlySpan.Parse(bytes);
                return true;
            }
            catch (Exception)
            {
                value = 0;
                return false;
            }
        }

        public static ulong ParseUlong(this ReadOnlySpan<char> readOnlySpan)
        {
            return (ulong)readOnlySpan.Parse(64);
        }

        public static bool TryParseUlong(this ReadOnlySpan<char> readOnlySpan, out ulong value)
        {
            var result = readOnlySpan.TryParse(64, out long parseValue);
            value = (ulong)parseValue;
            return result;
        }

        public static ushort ParseUshort(this ReadOnlySpan<char> readOnlySpan)
        {
            return (ushort)readOnlySpan.Parse(16);
        }

        public static bool TryParseUshort(this ReadOnlySpan<char> readOnlySpan, out ushort value)
        {
            var result = readOnlySpan.TryParse(16, out long parseValue);
            value = (ushort)parseValue;
            return result;
        }

        public static byte ParseByte(this ReadOnlySpan<char> readOnlySpan)
        {
            return (byte)readOnlySpan.Parse(2);
        }

        public static bool TryParseByte(this ReadOnlySpan<char> readOnlySpan, out byte value)
        {
            var result = readOnlySpan.TryParse(2, out long parseValue);
            value = (byte)parseValue;
            return result;
        }
    }
}
