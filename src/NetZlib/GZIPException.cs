// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NetZlib
{
    using System.IO;

    // https://github.com/ymnk/jzlib/blob/master/src/main/java/com/jcraft/jzlib/GZIPException.java
    public class GZIPException : IOException
    {
        public GZIPException()
        {
        }

        public GZIPException(string s) : base(s)
        {
        }
    }
}
