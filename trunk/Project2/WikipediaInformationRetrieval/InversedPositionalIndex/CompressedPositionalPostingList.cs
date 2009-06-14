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
        public CompressedPositionalPostingList(int size, byte[] byte_stream)
        {
            mPositions = null;
            mDocIds = null;
            mSizeOfDocIds = size;
            mCompressedPosting = byte_stream;
        }

        public CompressedPositionalPostingList(int size, BitStreamWriter bitstream)
        {
            mPositions = null;
            mDocIds = null;
            mSizeOfDocIds = size;
            mCompressedPosting = bitstream.Bytes;
        }

        public override CompressedPositionalPostingList Compress()
        {
            mDocIds = null;
            mPositions = null;
            return this;
        }

        public override void Decompress()
        {
            if (mDocIds != null)
                return;

            msBitStreamReader.ResetStream(mCompressedPosting);

            uint gap = 0;
            uint current_id = 0;
            ushort current_position = 0;
            
            uint length_of_positions = 0;

            mDocIds = new uint[mSizeOfDocIds];
            mPositions = new ushort[mSizeOfDocIds][];

            for(int k = 0; k < mSizeOfDocIds; k++)
            {
                gap = GammaEncoding.DecodeInt(msBitStreamReader);

                current_id += gap;
                mDocIds[k] = current_id;

                length_of_positions = GammaEncoding.DecodeInt(msBitStreamReader);

                mPositions[k] = new ushort[length_of_positions];
                current_position = 0;

                for (int i = 0; i < length_of_positions; i++)
                {
                    gap = GammaEncoding.DecodeInt(msBitStreamReader);

                    current_position += (ushort)gap;
                    mPositions[k][i] = current_position;
                }
            }

            //achtung!!! if it's here you can't use CompressPostings method from InversedPositionalIndex
            //mCompressedPosting = null;
        }

        public override long SizeInBytes
        {
            get
            {
                return sizeof(int) + mCompressedPosting.Length;
            }
        }

        private int mSizeOfDocIds;
        private byte[] mCompressedPosting;
        
        //used to decompress postings
        private static BitStreamReader msBitStreamReader = new BitStreamReader(new byte[0]);
    }
}
