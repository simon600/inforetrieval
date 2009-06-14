using System;
using System.Collections.Generic;
using System.Text;

namespace InversedIndex
{
    /// <summary>
    /// Describes a single article in file.
    /// </summary>
    [Serializable]
    public class Document : Identifyable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filePosition">File position of a document.</param>
        public Document(long filePosition)
            : base()
        {
            mFilePosition = filePosition;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Id of the document.</param>
        /// <param name="filePosition">File position of a document.</param>
        public Document(uint id, long filePosition)
            : base(id)
        {
            mFilePosition = filePosition;
        }

        /// <summary>
        /// Gets file position.
        /// </summary>
        public long FilePosition
        {
            get
            {
                return mFilePosition;
            }
        }


        private long mFilePosition;
    }
}
