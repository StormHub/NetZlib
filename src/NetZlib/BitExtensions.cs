// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NetZlib
{
    using System.Runtime.CompilerServices;

    static class BitExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RightUShift(this long value, int shift) => unchecked ((long)(((ulong)value) >> shift));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RightUShift(this ulong value, int shift) => unchecked((long)(value >> shift));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RightUShift(this int value, int shift) => unchecked((int)(((uint)value) >> shift));
    }
}
