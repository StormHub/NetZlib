// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// ReSharper disable ArrangeThisQualifier
// ReSharper disable InconsistentNaming
namespace NetZlib
{
    // https://github.com/ymnk/jzlib/blob/master/src/main/java/com/jcraft/jzlib/JZlib.java
    public sealed class JZlib
    {
        const string VersionString = "1.1.0";
        public static string Version() => VersionString;

        public static readonly int MAX_WBITS = 15; // 32K LZ77 window
        public static readonly int DEF_WBITS = MAX_WBITS;

        public enum WrapperType
        {
            NONE,
            ZLIB,
            GZIP,
            ANY
        }

        public static readonly WrapperType W_NONE = WrapperType.NONE;
        public static readonly WrapperType W_ZLIB = WrapperType.ZLIB;
        public static readonly WrapperType W_GZIP = WrapperType.GZIP;
        public static readonly WrapperType W_ANY = WrapperType.ANY;

        // compression levels
        public static readonly int Z_NO_COMPRESSION = 0;
        public static readonly int Z_BEST_SPEED = 1;
        public static readonly int Z_BEST_COMPRESSION = 9;
        public static readonly int Z_DEFAULT_COMPRESSION = (-1);

        // compression strategy
        public static readonly int Z_FILTERED = 1;
        public static readonly int Z_HUFFMAN_ONLY = 2;
        public static readonly int Z_DEFAULT_STRATEGY = 0;

        public static readonly int Z_NO_FLUSH = 0;
        public static readonly int Z_PARTIAL_FLUSH = 1;
        public static readonly int Z_SYNC_FLUSH = 2;
        public static readonly int Z_FULL_FLUSH = 3;
        public static readonly int Z_FINISH = 4;

        public static readonly int Z_OK = 0;
        public static readonly int Z_STREAM_END = 1;
        public static readonly int Z_NEED_DICT = 2;
        public static readonly int Z_ERRNO = -1;
        public static readonly int Z_STREAM_ERROR = -2;
        public static readonly int Z_DATA_ERROR = -3;
        public static readonly int Z_MEM_ERROR = -4;
        public static readonly int Z_BUF_ERROR = -5;
        public static readonly int Z_VERSION_ERROR = -6;

        // The three kinds of block type
        public static readonly byte Z_BINARY = 0;
        public static readonly byte Z_ASCII = 1;
        public static readonly byte Z_UNKNOWN = 2;

        public static long Adler32_combine(long adler1, long adler2, long len2) =>
            Adler32.Combine(adler1, adler2, len2);
    }
}
