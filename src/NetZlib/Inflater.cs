// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// ReSharper disable ArrangeThisQualifier
// ReSharper disable InconsistentNaming
namespace NetZlib
{
    // https://github.com/ymnk/jzlib/blob/master/src/main/java/com/jcraft/jzlib/Inflater.java
    sealed class Inflater : ZStream
    {
        const int MAX_WBITS = 15; // 32K LZ77 window
        const int DEF_WBITS = MAX_WBITS;

        //const int Z_NO_FLUSH = 0;
        //const int Z_PARTIAL_FLUSH = 1;
        //const int Z_SYNC_FLUSH = 2;
        //const int Z_FULL_FLUSH = 3;
        //const int Z_FINISH = 4;

        //const int MAX_MEM_LEVEL = 9;

        const int Z_OK = 0;
        //const int Z_STREAM_END = 1;
        //const int Z_NEED_DICT = 2;
        //const int Z_ERRNO = -1;
        const int Z_STREAM_ERROR = -2;
        //const int Z_DATA_ERROR = -3;
        //const int Z_MEM_ERROR = -4;
        //const int Z_BUF_ERROR = -5;
        //const int Z_VERSION_ERROR = -6;

        public Inflater()
        {
            Init();
        }

        public Inflater(JZlib.WrapperType wrapperType) : this(DEF_WBITS, wrapperType)
        {
        }

        public Inflater(int w, JZlib.WrapperType wrapperType)
        {
            int ret = Init(w, wrapperType);
            if (ret != Z_OK) throw new GZIPException(ret + ": " + msg);
        }

        public Inflater(int w) : this(w, false)
        {
        }

        public Inflater(bool nowrap) : this(DEF_WBITS, nowrap)
        {
        }

        public Inflater(int w, bool nowrap)
        {
            int ret = Init(w, nowrap);
            if (ret != Z_OK) throw new GZIPException(ret + ": " + msg);
        }

        //bool finished = false;

        public int Init() => Init(DEF_WBITS);

        public int Init(JZlib.WrapperType wrapperType) => Init(DEF_WBITS, wrapperType);

        public int Init(int w, JZlib.WrapperType wrapperType)
        {
            bool nowrap = false;
            if (wrapperType == JZlib.W_NONE)
            {
                nowrap = true;
            }
            else if (wrapperType == JZlib.W_GZIP)
            {
                w += 16;
            }
            else if (wrapperType == JZlib.W_ANY)
            {
                w |= NetZlib.Inflate.INFLATE_ANY;
            }
            else if (wrapperType == JZlib.W_ZLIB)
            {
            }
            return Init(w, nowrap);
        }

        public int Init(bool nowrap) => Init(DEF_WBITS, nowrap);

        public int Init(int w) => Init(w, false);

        public int Init(int w, bool nowrap)
        {
            //finished = false;
            istate = new Inflate(this);
            return istate.InflateInit(nowrap ? -w : w);
        }

        public int Inflate(int f)
        {
            if (istate == null) return Z_STREAM_ERROR;
            int ret = istate.Inflate_I(f);
            //if (ret == Z_STREAM_END)
            //    finished = true;
            return ret;
        }

        public override int End()
        {
            // finished = true;
            if (istate == null) return Z_STREAM_ERROR;
            int ret = istate.InflateEnd();
            //    istate = null;
            return ret;
        }

        public int Sync()
        {
            if (istate == null) return Z_STREAM_ERROR;
            return istate.InflateSync();
        }
        public int SyncPoint()
        {
            if (istate == null) return Z_STREAM_ERROR;
            return istate.InflateSyncPoint();
        }

        public int SetDictionary(byte[] dictionary, int dictLength)
        {
            if (istate == null) return Z_STREAM_ERROR;
            return istate.InflateSetDictionary(dictionary, dictLength);
        }

        public override bool Finished() => istate.mode == 12 /*DONE*/;
    }
}
