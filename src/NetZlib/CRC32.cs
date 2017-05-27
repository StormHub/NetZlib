// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// ReSharper disable ArrangeThisQualifier
// ReSharper disable InconsistentNaming
namespace NetZlib
{
    using System;
    using System.Runtime.CompilerServices;

    // https://github.com/ymnk/jzlib/blob/master/src/main/java/com/jcraft/jzlib/CRC32.java
    sealed class CRC32 : IChecksum
    {
        
        //  The following logic has come from RFC1952.
        int v;
        static readonly int[] crc_table;

        static CRC32()
        {
            crc_table = new int[256];
            for (int n = 0; n < 256; n++)
            {
                int c = n;
                for (int k = 8; --k >= 0;)
                {
                    if ((c & 1) != 0)
                        c = (int)(0xedb88320 ^ (c.RightUShift(1)));
                    else
                        c = c.RightUShift(1);
                }
                crc_table[n] = c;
            }
        }

        public void Update(byte[] buf, int index, int len)
        {
            int c = ~v;
            while (--len >= 0)
                c = crc_table[(c ^ buf[index++]) & 0xff] ^ (c.RightUShift(8));
            v = ~c;
        }

        public void Reset() => v = 0;

        public void Reset(long vv) => v = (int)(vv & 0xffffffffL);

        public long GetValue() => v & 0xffffffffL;

        // The following logic has come from zlib.1.2.
        const int GF2_DIM = 32;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long Combine(long crc1, long crc2, long len2)
        {
            var even = new long[GF2_DIM];
            var odd = new long[GF2_DIM];

            // degenerate case (also disallow negative lengths)
            if (len2 <= 0)
                return crc1;

            // put operator for one zero bit in odd
            odd[0] = 0xedb88320L; // CRC-32 polynomial
            long row = 1;
            for (int n = 1; n < GF2_DIM; n++)
            {
                odd[n] = row;
                row <<= 1;
            }

            // put operator for two zero bits in even
            gf2_matrix_square(even, odd);

            // put operator for four zero bits in odd
            gf2_matrix_square(odd, even);

            // apply len2 zeros to crc1 (first square will put the operator for one
            // zero byte, eight zero bits, in even)
            do
            {
                // apply zeros operator for this bit of len2
                gf2_matrix_square(even, odd);
                if ((len2 & 1) != 0)
                    crc1 = gf2_matrix_times(even, crc1);
                len2 >>= 1;

                // if no more bits set, then done
                if (len2 == 0)
                    break;

                // another iteration of the loop with odd and even swapped
                gf2_matrix_square(odd, even);
                if ((len2 & 1) != 0)
                    crc1 = gf2_matrix_times(odd, crc1);
                len2 >>= 1;

                // if no more bits set, then done
            }
            while (len2 != 0);

            /* return combined crc */
            crc1 ^= crc2;
            return crc1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static long gf2_matrix_times(long[] mat, long vec)
        {
            long sum = 0;
            int index = 0;
            while (vec != 0)
            {
                if ((vec & 1) != 0)
                    sum ^= mat[index];
                vec >>= 1;
                index++;
            }
            return sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void gf2_matrix_square(long[] square, long[] mat)
        {
            for (int n = 0; n < GF2_DIM; n++)
                square[n] = gf2_matrix_times(mat, mat[n]);
        }

        /*
        private java.util.zip.CRC32 crc32 = new java.util.zip.CRC32();
        public void update(byte[] buf, int index, int len){
          if(buf==null) {crc32.reset();}
          else{crc32.update(buf, index, len);}
        }
        public void reset(){
          crc32.reset();
        }
        public void reset(long init){
          if(init==0L){
            crc32.reset();
          }
          else{
            System.err.println("unsupported operation");
          }
        }
        public long getValue(){
          return crc32.getValue();
        }
        */

        public IChecksum Copy()
        {
            var foo = new CRC32();
            foo.v = this.v;
            return foo;
        }

        public static int[] getCRC32Table()
        {
            var tmp = new int[crc_table.Length];
            Array.Copy(crc_table, 0, tmp, 0, tmp.Length);
            return tmp;
        }
    }
}
