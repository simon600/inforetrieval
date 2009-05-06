using System;
using System.Collections.Generic;
using System.Text;
using GammaCompression;

namespace InversedIndex
{
    /// <summary>
    /// 
    /// </summary>
    public class CompressedPositionalPostingList: PositionalPostingList
    {
        public CompressedPositionalPostingList(int size)
            : base()
        {
            mSizeOfDocIds = size;
        }

        public CompressedPositionalPostingList(int size, byte[] byte_stream)
            : base()
        {
            mSizeOfDocIds = size;
            mCompressedPosting = new BitStream(byte_stream);
        }

        public CompressedPositionalPostingList(int size, BitStream bitstream)
            : base()
        {
            mSizeOfDocIds = size;
            mCompressedPosting = bitstream;
        }

        public override CompressedPositionalPostingList Compress()
        {
            mDocIds = null;
            mPositions = null;
            return this;
        }

        public override void Decompress()
        {
            mCompressedPosting.SetOnStart();

            uint gap = 0;
            uint current_id = 0;
            ushort current_position = 0;
            
            uint length_of_positions = 0;

            mDocIds = new uint[mSizeOfDocIds];
            mPositions = new ushort[mSizeOfDocIds][];

            for(int k = 0; k < mSizeOfDocIds; k++)
            {
                gap = GammaEncoding.DecodeInt(mCompressedPosting);

                current_id += gap;
                mDocIds[k] = current_id;

                length_of_positions = GammaEncoding.DecodeInt(mCompressedPosting);

                mPositions[k] = new ushort[length_of_positions];
                current_position = 0;

                for (int i = 0; i < length_of_positions; i++)
                {
                    gap = GammaEncoding.DecodeInt(mCompressedPosting);

                    current_position += (ushort)gap;
                    mPositions[k][i] = current_position;
                }

            }
        }

        private int mSizeOfDocIds;
        private BitStream mCompressedPosting;

    }
}
