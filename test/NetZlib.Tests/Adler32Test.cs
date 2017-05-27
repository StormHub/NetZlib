// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NetZlib.Tests
{
    using System;
    using System.Collections.Generic;
    using Xunit;

    public sealed class Adler32Test
    {
        [Fact]
        public void Copy()
        {
            var random = new Random();

            var buf1 = new byte[1024];
            var buf2 = new byte[1024];

            random.NextBytes(buf1);
            random.NextBytes(buf2);

            var adler1 = new Adler32();
            adler1.Update(buf1, 0, buf1.Length);

            var adler2 = (Adler32)adler1.Copy();
            adler1.Update(buf2, 0, buf1.Length);
            adler2.Update(buf2, 0, buf1.Length);

            long expected = adler1.GetValue();
            long actual = adler2.GetValue();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Combine()
        {
            var random = new Random();

            var buf1 = new byte[1024];
            var buf2 = new byte[1024];

            random.NextBytes(buf1);
            random.NextBytes(buf2);

            var adler = new Adler32();
            long adler1 = GetValue(adler, new List<byte[]> { buf1 });
            long adler2 = GetValue(adler, new List<byte[]> { buf2 });
            long expected = GetValue(adler, new List<byte[]>{ buf1, buf2 });

            long actual = Adler32.Combine(adler1, adler2, buf2.Length);
            Assert.Equal(expected, actual);
        }

        static long GetValue(Adler32 adler, List<byte[]> bufs)
        {
            adler.Reset();
            foreach (byte[] b in bufs)
            {
                adler.Update(b, 0, b.Length);
            }

            return adler.GetValue();
        }
    }
}
