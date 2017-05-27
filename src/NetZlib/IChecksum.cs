// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NetZlib
{
    public interface IChecksum
    {
        void Update(byte[] buf, int index, int len);

        void Reset();

        void Reset(long init);

        long GetValue();

        IChecksum Copy();
    }
}
