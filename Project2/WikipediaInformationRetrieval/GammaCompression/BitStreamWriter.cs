﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GammaCompression
{
    /// <summary>
    /// Represents tape of bits to write on.
    /// It can be write from left to right.
    /// </summary>
    public class BitStreamWriter
    {
        /// <summary>
        /// Create empty bit stream
        /// </summary>
        public BitStreamWriter()
        {
            mByteTape = new List<byte>();
            mByteTape.Add(0);

            mIndex = 0;
            mMaskIndex = 0;
        }


        /// <summary>
        /// Write bit on tape.
        /// </summary>
        /// <param name="value">True means bit will be set on 1
        /// False means bit doesn't change</param>
        public void SetNextBit(bool value)
        {
            if (value)
                mByteTape[mIndex] |= msMasks[mMaskIndex];

            mMaskIndex++;
            if(mMaskIndex == 8)
            {
                mMaskIndex = 0;
                mIndex++;
                mByteTape.Add(0);
            }
        }
     
        /// <summary>
        /// Set all bits to 0 and go to the begining of tape
        /// </summary>
        public void ResetStream()
        {
            mIndex = 0;
            mMaskIndex = 0;

            mByteTape.Clear();
            mByteTape.Add(0);
        }

        /// <summary>
        /// Number of bits write to stream
        /// </summary>
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

        /// <summary>
        /// Number of bytes that stream takes
        /// </summary>
        public int StreamSize
        {
            get
            {
                return mByteTape.Count;
            }
        }

        /// <summary>
        /// Array of bytes which coded bit stream
        /// </summary>
        public byte[] Bytes
        {
            get
            {
                return mByteTape.ToArray();
            }
        }

        /// <summary>
        /// True if we read already all of written bits
        /// </summary>
        //public bool EndOfStream
        //{
        //    get
        //    {
        //        return Length == (mIndex << 3) + mMaskIndex;
        //    }
        //}

        public long SizeInBytes
        {
            get
            {
                return 3 * sizeof(int) + mByteTape.Count * sizeof(byte);
            }
        }

        private List<byte> mByteTape;
        private int mIndex;
        private int mMaskIndex;
        private int mBitLength;

        private static byte[] msMasks = { 128, 64, 32, 16, 8, 4, 2, 1 };
    }
}
