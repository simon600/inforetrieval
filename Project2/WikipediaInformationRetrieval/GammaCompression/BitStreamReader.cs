using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GammaCompression
{
    public class BitStreamReader
    {

        /// <summary>
        /// Create bit stream from array of bytes
        /// </summary>
        /// <param name="bytes"></param>
        public BitStreamReader(byte[] bytes)
        {
            mBytes = bytes;

            mIndex = 0;
            mMaskIndex = 0;
        }

        /// <summary>
        /// Read next bit from tape
        /// Throw IndexOutOfBound Exception if its end of stream
        /// </summary>
        /// <returns>True if readed bit is 1
        ///          False if readed bit is 0</returns>
        public bool GetNextBit()
        {
            int result = mBytes[mIndex] & msMasks[mMaskIndex];

            mMaskIndex++;
            if (mMaskIndex == 8)
            {
                mMaskIndex = 0;
                mIndex++;
            }

            return result > 0;
        }

        /// <summary>
        /// Set new source stream of bits
        /// </summary>
        public void ResetStream(byte[] bytes)
        {
            mIndex = 0;
            mMaskIndex = 0;

            mBytes = bytes;
        }

        /// <summary>
        /// True if we read already all of written bits
        /// </summary>
        public bool EndOfStream
        {
            get
            {
                return ((mIndex == mBytes.Length - 1) && (mMaskIndex == 7));
            }
        }

        public long SizeInBytes
        {
            get
            {
                return 3 * sizeof(int) + mBytes.Length * sizeof(byte);
            }
        }

        private byte[] mBytes;
        private int mIndex;
        private int mMaskIndex;
        private int mBitLength;

        private static byte[] msMasks = { 128, 64, 32, 16, 8, 4, 2, 1 };
    }
}
