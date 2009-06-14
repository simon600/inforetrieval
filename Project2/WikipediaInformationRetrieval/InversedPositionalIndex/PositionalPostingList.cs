using System;
using System.Collections.Generic;
using System.Text;
using GammaCompression;

namespace InversedIndex
{
    /// <summary>
    /// Describes positional posting list.
    /// </summary>
    public class PositionalPostingList
    {
        public PositionalPostingList()
        {
            mDocIds = new uint[0];
            mPositions = new ushort[0][];
        }

        public PositionalPostingList(uint[] DocIds, ushort[][] Positions)
        {
            mDocIds = DocIds;
            mPositions = Positions;
        }

        public uint[] DocumentIds
        {
            get
            {
                return mDocIds;
            }
        }

        public ushort[][] Positions
        {
            get
            {
                return mPositions;
            }
        }

        public virtual long SizeInBytes
        {
            get
            {
                long len = 0;
                foreach (ushort[] positions in mPositions)
                    len += positions.Length;
                
                return len * sizeof(ushort) + mDocIds.Length * sizeof(uint);
            }
        }

        public virtual CompressedPositionalPostingList Compress()
        {
            BitStreamWriter compressed_posting = new BitStreamWriter();

            uint gap = 0;
            uint current_id = 0;
            ushort current_position = 0;

            for (int k = 0; k < mDocIds.Length; k++)
            {
                gap = mDocIds[k] - current_id;
                current_id += gap;
               
                GammaEncoding.CodeInt(gap, compressed_posting);
       
                GammaEncoding.CodeInt((uint)mPositions[k].Length, compressed_posting);
                
                current_position = 0;

                for (int i = 0; i < mPositions[k].Length; i++)
                {
                    gap = (uint)(mPositions[k][i] - current_position);
                    current_position += (ushort)gap;

                    GammaEncoding.CodeInt(gap, compressed_posting); 
                }
            }
            
            return new CompressedPositionalPostingList(mDocIds.Length, compressed_posting);
        }

        public virtual void Decompress()
        {
        }

        /// <summary>
        /// Array of document ids.
        /// </summary>
        protected uint[] mDocIds;

        /// <summary>
        /// Array of arrays of positions for each doc id.
        /// </summary>
        protected ushort[][] mPositions;
    
    }
}
