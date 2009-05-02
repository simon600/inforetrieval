using System;
using System.Collections.Generic;
using System.Text;

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
            mPositions = new uint[0][];
        }

        public PositionalPostingList(uint[] DocIds, uint[][]Positions)
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

        public uint[][] Positions
        {
            get
            {
                return mPositions;
            }
        }

        /// <summary>
        /// Array of document ids.
        /// </summary>
        private uint[] mDocIds;

        /// <summary>
        /// Array of arrays of positions for each doc id.
        /// </summary>
        private uint[][] mPositions;
    }
}
