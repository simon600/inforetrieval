using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GammaCompression
{
    public class BitStream
    {
        public BitStream()
        {
            mByteTape = new List<byte>();
            mByteTape.Add(0);

            mIndex = 0;
            mMaskIndex = 0;
        }

        public BitStream(byte[] bytes)
        {
            mByteTape = new List<byte>(bytes);

            mIndex = 0;
            mMaskIndex = 0;
        }

        public void SetNextBit(bool value)
        {
            if (value)
                mByteTape[mIndex] |= mMasks[mMaskIndex];

            mMaskIndex++;
            if(mMaskIndex == 8)
            {
                mMaskIndex = 0;
                mIndex++;
                mByteTape.Add(0);
            }

        }

        /// <summary>
        /// throw IndexOutOfBound Exception if its End of Stream
        /// </summary>
        /// <returns></returns>
        public bool GetNextBit()
        {
            int result = mByteTape[mIndex] & mMasks[mMaskIndex];

            mMaskIndex++;
            if (mMaskIndex == 8)
            {
                mMaskIndex = 0;
                mIndex++;
            }

            return result > 0;
        }

        public void SetOnStart()
        {
            mBitLength = (mIndex << 3) + mMaskIndex;

            mIndex = 0;
            mMaskIndex = 0;
        }

        public void GoToEnd()
        {
            mMaskIndex = mBitLength % 8;
            mIndex = mBitLength >> 3;
        }

        public void ResetStream()
        {
            mIndex = 0;
            mMaskIndex = 0;

            mByteTape.Clear();
            mByteTape.Add(0);
        }

        public int Length
        {
            get
            {
                int position = (mIndex << 3) + mMaskIndex;
                if (position < mBitLength)
                    return mBitLength;
                else return position;
            }
        }

        public int StreamSize
        {
            get
            {
                return mByteTape.Count;
            }
        }

        public byte[] Bytes
        {
            get
            {
                return mByteTape.ToArray();
            }
        }

        public bool EndOfStream
        {
            get
            {
                return Length == (mIndex << 3) + mMaskIndex; 
            }
        }

        private List<byte> mByteTape;
        private int mIndex;
        private int mMaskIndex;
        private int mBitLength;

        private byte[] mMasks = { 128, 64, 32, 16, 8, 4, 2, 1 };
    }
}
