using System;
using System.Text;
using DotNetty.Buffers;

namespace ClashRoyale.Utilities.Netty
{
    /// <summary>
    ///     This implements a few extensions for games from supercell
    /// </summary>
    public static class Reader
    {
        /// <summary>
        ///     Decodes a string based on the length
        /// </summary>
        /// <param name="byteBuffer"></param>
        /// <returns></returns>
        public static string ReadScString(this IByteBuffer byteBuffer)
        {
            var length = byteBuffer.ReadInt();

            if (length <= 0 || length > 900000)
                return string.Empty;

            return byteBuffer.ReadString(length, Encoding.UTF8);
        }

        /// <summary>
        ///     Decodes a VInt (Variable Length Integer) - special greets to nameless who made this way smaller
        /// </summary>
        /// <param name="byteBuffer"></param>
        /// <returns></returns>
        public static int ReadVInt(this IByteBuffer byteBuffer)
        {
            if (byteBuffer.ReadableBytes < 1)
                throw new InvalidOperationException("VInt incompleto: no hay bytes disponibles.");

            int b = byteBuffer.ReadByte();
            int sign = (b >> 6) & 1;
            int i = b & 0x3F;
            int offset = 6;

            for (var j = 0; j < 4 && (b & 0x80) != 0; j++, offset += 7)
            {
                if (byteBuffer.ReadableBytes < 1)
                    throw new InvalidOperationException("VInt incompleto: byte faltante en continuación.");

                b = byteBuffer.ReadByte();
                i |= (b & 0x7F) << offset;
            }

            if ((b & 0x80) != 0)
                return -1;

            if (sign == 1 && offset < 32)
                return i | (int)(0xFFFFFFFF << offset);

            return i;
        }
    }
}