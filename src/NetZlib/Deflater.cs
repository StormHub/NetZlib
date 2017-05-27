// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// ReSharper disable ArrangeThisQualifier
// ReSharper disable InconsistentNaming
namespace NetZlib
{
    // https://github.com/ymnk/jzlib/blob/master/src/main/java/com/jcraft/jzlib/Deflater.java
    sealed class Deflater : ZStream
    {
        const int MAX_WBITS = 15; // 32K LZ77 window
        //const int DEF_WBITS = MAX_WBITS;

        //const int Z_NO_FLUSH = 0;
        //const int Z_PARTIAL_FLUSH = 1;
        //const int Z_SYNC_FLUSH = 2;
        //const int Z_FULL_FLUSH = 3;
        //const int Z_FINISH = 4;

        //const int MAX_MEM_LEVEL = 9;

        const int Z_OK = 0;
        const int Z_STREAM_END = 1;
        //const int Z_NEED_DICT = 2;
        //const int Z_ERRNO = -1;
        const int Z_STREAM_ERROR = -2;
        //const int Z_DATA_ERROR = -3;
        //const int Z_MEM_ERROR = -4;
        //const int Z_BUF_ERROR = -5;
        //const int Z_VERSION_ERROR = -6;

        bool finished;

        public Deflater()
        {
        }

        public Deflater(int level)
            : this(level, MAX_WBITS)
        {
        }

        public Deflater(int level, bool nowrap)
            : this(level, MAX_WBITS, nowrap)
        {
        }

        public Deflater(int level, int bits)
            : this(level, bits, false)
        {
        }

        public Deflater(int level, int bits, bool nowrap)
        {
            int ret = Init(level, bits, nowrap);
            if (ret != Z_OK) throw new GZIPException(ret + ": " + msg);
        }

        public Deflater(int level, int bits, int memlevel, JZlib.WrapperType wrapperType)
        {
            int ret = Init(level, bits, memlevel, wrapperType);
            if (ret != Z_OK) throw new GZIPException(ret + ": " + msg);
        }

        public Deflater(int level, int bits, int memlevel)
        {
            int ret = Init(level, bits, memlevel);
            if (ret != Z_OK) throw new GZIPException(ret + ": " + msg);
        }

        public int Init(int level) => Init(level, MAX_WBITS);

        public int Init(int level, bool nowrap) => Init(level, MAX_WBITS, nowrap);

        public int Init(int level, int bits) => Init(level, bits, false);

        public int Init(int level, int bits, int memlevel, JZlib.WrapperType wrapperType)
        {
            if (bits < 9 || bits > 15)
            {
                return Z_STREAM_ERROR;
            }
            if (wrapperType == JZlib.W_NONE)
            {
                bits *= -1;
            }
            else if (wrapperType == JZlib.W_GZIP)
            {
                bits += 16;
            }
            else if (wrapperType == JZlib.W_ANY)
            {
                return Z_STREAM_ERROR;
            }
            else if (wrapperType == JZlib.W_ZLIB)
            {
            }
            return Init(level, bits, memlevel);
        }

        public int Init(int level, int bits, int memlevel)
        {
            finished = false;
            dstate = new Deflate(this);
            return dstate.DeflateInit(level, bits, memlevel);
        }

        public int Init(int level, int bits, bool nowrap)
        {
            finished = false;
            dstate = new Deflate(this);
            return dstate.DeflateInit(level, nowrap ? -bits : bits);
        }

        public int Deflate(int flush)
        {
            if (dstate == null)
            {
                return Z_STREAM_ERROR;
            }
            int ret = dstate.Deflate_D(flush);
            if (ret == Z_STREAM_END)
                finished = true;
            return ret;
        }

        public override int End()
        {
            finished = true;
            if (dstate == null)
                return Z_STREAM_ERROR;
            int ret = dstate.DeflateEnd();
            dstate = null;
            Free();
            return ret;
        }

        public int Params(int level, int strategy)
        {
            if (dstate == null) return Z_STREAM_ERROR;
            return dstate.DeflateParams(level, strategy);
        }

        public int SetDictionary(byte[] dictionary, int dictLength)
        {
            if (dstate == null) return Z_STREAM_ERROR;
            return dstate.DeflateSetDictionary(dictionary, dictLength);
        }

        public override bool Finished() => finished;

        public int Copy(Deflater src)
        {
            this.finished = src.finished;
            return NetZlib.Deflate.DeflateCopy(this, src);
        }
    }
}
