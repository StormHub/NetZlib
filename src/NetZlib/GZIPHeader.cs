// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// ReSharper disable ArrangeThisQualifier
// ReSharper disable InconsistentNaming
namespace NetZlib
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    // https://github.com/ymnk/jzlib/blob/master/src/main/java/com/jcraft/jzlib/GZIPHeader.java
    // http://www.ietf.org/rfc/rfc1952.txt
    class GZIPHeader
    {
        static readonly Encoding ISOEncoding = Encoding.GetEncoding("ISO-8859-1");
        static readonly byte Platform;

        public static readonly byte OS_MSDOS   = 0x00;
        public static readonly byte OS_AMIGA   = 0x01;
        public static readonly byte OS_VMS     = 0x02;
        public static readonly byte OS_UNIX    = 0x03;
        public static readonly byte OS_ATARI   = 0x05;
        public static readonly byte OS_OS2     = 0x06;
        public static readonly byte OS_MACOS   = 0x07;
        public static readonly byte OS_TOPS20  = 0x0a;
        public static readonly byte OS_WIN32   = 0x0b;
        public static readonly byte OS_VMCMS   = 0x04;
        public static readonly byte OS_ZSYSTEM = 0x08;
        public static readonly byte OS_CPM     = 0x09;
        public static readonly byte OS_QDOS    = 0x0c;
        public static readonly byte OS_RISCOS  = 0x0d;
        public static readonly byte OS_UNKNOWN = 0xff;

        bool text = false;
        bool fhcrc = false;
        internal long time;
        internal int xflags;
        internal int os;
        internal byte[] extra;
        internal byte[] name;
        internal byte[] comment;
        internal int hcrc;
        internal long crc;
        //bool done = false;
        long mtime;

        static GZIPHeader()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Platform = OS_WIN32;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Platform = OS_UNIX;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Platform = OS_MACOS;
            }
            else
            {
                Platform = OS_UNKNOWN;
            }
        }

        internal GZIPHeader()
        {
            this.os = Platform;
        }

        public void SetModifiedTime(long value) => this.mtime = value;

        public long GetModifiedTime() => this.mtime;

        public void SetOS(int value)
        {
            if ((0 <= value && value <= 13) || value == 255)
                this.os = value;
            else
                throw new ArgumentException(nameof(value));
        }

        public int GetOS() => this.os;

        public void SetName(string value) => this.name = ISOEncoding.GetBytes(value);

        public string GetName() => this.name == null ? string.Empty : ISOEncoding.GetString(this.name);

        public void SetComment(string value) => this.comment = ISOEncoding.GetBytes(value);

        public string GetComment() => this.comment == null ? string.Empty : ISOEncoding.GetString(this.comment);

        public void SetCRC(long value) => this.crc = value;

        public long GetCRC() => this.crc;

        internal void Put(Deflate d)
        {
            int flag = 0;
            if (text)
            {
                flag |= 1;     // FTEXT
            }
            if (fhcrc)
            {
                flag |= 2;     // FHCRC
            }
            if (extra != null)
            {
                flag |= 4;     // FEXTRA
            }
            if (name != null)
            {
                flag |= 8;    // FNAME
            }
            if (comment != null)
            {
                flag |= 16;   // FCOMMENT
            }
            int xfl = 0;
            if (d.level == JZlib.Z_BEST_SPEED)
            {
                xfl |= 4;
            }
            else if (d.level == JZlib.Z_BEST_COMPRESSION)
            {
                xfl |= 2;
            }

            d.Put_short(unchecked((short)0x8b1f));  // ID1 ID2
            d.Put_byte(8);         // CM(Compression Method)
            d.Put_byte((byte)flag);
            d.Put_byte((byte)mtime);
            d.Put_byte((byte)(mtime >> 8));
            d.Put_byte((byte)(mtime >> 16));
            d.Put_byte((byte)(mtime >> 24));
            d.Put_byte((byte)xfl);
            d.Put_byte((byte)os);

            if (extra != null)
            {
                d.Put_byte((byte)extra.Length);
                d.Put_byte((byte)(extra.Length >> 8));
                d.Put_byte(extra, 0, extra.Length);
            }

            if (name != null)
            {
                d.Put_byte(name, 0, name.Length);
                d.Put_byte(0);
            }

            if (comment != null)
            {
                d.Put_byte(comment, 0, comment.Length);
                d.Put_byte(0);
            }
        }

        public GZIPHeader Clone()
        {
            var gheader = new GZIPHeader();
            byte[] tmp;
            if (gheader.extra != null)
            {
                tmp = new byte[gheader.extra.Length];
                Array.Copy(gheader.extra, 0, tmp, 0, tmp.Length);
                gheader.extra = tmp;
            }

            if (gheader.name != null)
            {
                tmp = new byte[gheader.name.Length];
                Array.Copy(gheader.name, 0, tmp, 0, tmp.Length);
                gheader.name = tmp;
            }

            if (gheader.comment != null)
            {
                tmp = new byte[gheader.comment.Length];
                Array.Copy(gheader.comment, 0, tmp, 0, tmp.Length);
                gheader.comment = tmp;
            }

            return gheader;
        }
    }
}
