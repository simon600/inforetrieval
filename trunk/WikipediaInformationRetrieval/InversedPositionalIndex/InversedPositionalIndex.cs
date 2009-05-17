using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace InversedIndex
{
    /// <summary>
    /// Class holding 
    /// </summary>
    public class InversedPositionalIndex
    {
        /// <summary>
        /// Gets the only instance of inversed positional index.
        /// </summary>
        /// <returns>The instance of inversed positional index.</returns>
        public static InversedPositionalIndex Get()
        {
            if (msInversedPositionalIndex == null)
            {
                msInversedPositionalIndex = new InversedPositionalIndex();
            }

            return msInversedPositionalIndex;
        }

        /// <summary>
        /// Gets posting list for a specified word.
        /// </summary>
        /// <param name="word">Word to find posting list for.</param>
        /// <returns>A posting list.</returns>
        public PositionalPostingList PostingList(string word)
        {
            int index = Array.BinarySearch(mWords, word);

            if (index >= 0)
            {
                PositionalPostingList posting = mPostingLists[index];
                posting.Decompress();

                return posting;
            }
            else
            {                
                return new PositionalPostingList();
            }
        }

        public long VocabularySize
        {
            get
            {
                if (mWords == null)
                    return 0;
                return mWords.Length;
            }
        }

        public void CompressPostings()
        {
            for(int i = 0; i<mPostingLists.Length; i++)
                mPostingLists[i] = mPostingLists[i].Compress();

            GC.Collect();
        }

        /// <summary>
        /// Reads inversed index from stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        public void ReadFromStream(Stream stream)
        {

            BinaryReader reader = new BinaryReader(stream);

            // get size of the index
            int size = reader.ReadInt32();

            mPerformedLematization = reader.ReadBoolean();
            mPerformedStemming = reader.ReadBoolean();
            mPerformedStopWordsRemoval = reader.ReadBoolean();
            mPerformCompression = reader.ReadBoolean();

            mWords = new string[size];
            mPostingLists = new PositionalPostingList[size];
            
            // read from stream
            for (int i = 0; i < size; i++)
            {
                mWords[i] = reader.ReadString();
            
                PositionalPostingList postingList =
                    mPostingLists[i] = ReadPostingList(reader);

            }

            int positionsSize = reader.ReadInt32();

            mDocumentsPositions = new long[positionsSize];

            for (int i = 0; i < positionsSize; i++)
                mDocumentsPositions[i] = reader.ReadInt64();

        }

        /// <summary>
        /// Gets documents.
        /// </summary>
        //public Dictionary<uint, Document> Documents
        //{
        //    get
        //    {
        //        return mDocuments;
        //    }
        //}

        public long[] Positions
        {
            get
            {
                return mDocumentsPositions;
            }
        }

        public bool PerformedCompression
        {
            get
            {
                return mPerformCompression;
            }
        }

        public bool PerformedStemming
        {
            get
            {
                return mPerformedStemming;
            }
        }

        public bool PerformedLematization
        {
            get
            {
                return mPerformedLematization;
            }
        }

        public bool PerformedStopWordsRemoval
        {
            get
            {
                return mPerformedStopWordsRemoval;
            }
        }

        public long PostingSizeInBytes
        {
            get
            {
                long size = 0;
                foreach (PositionalPostingList p in mPostingLists)
                    size += p.SizeInBytes;

                return size;
            }

        }

        public long VocabularySizeInBytes
        {
            get
            {
                Encoding enc = Encoding.UTF8;
                long size = 0;
                foreach (string word in mWords)
                    size += enc.GetByteCount(word.ToCharArray());

                return size;
            }

        }

        //////////////////////////////// PRIVATE /////////////////////////////

        /// <summary>
        /// Default constructor.
        /// </summary>
        private InversedPositionalIndex()
        {
           //mDocuments = new Dictionary<uint, Document>();
        }

        private PositionalPostingList ReadPostingList(BinaryReader reader)
        {
            if (mPerformCompression)
                return ReadCompressedPostingList(reader);


            int posting_length;
            int positions_length;

            uint[] doc_ids = null;
            ushort[][] positions = null;

            posting_length = reader.ReadInt32();

            doc_ids = new uint[posting_length];
            positions = new ushort[posting_length][];

            for (int i = 0; i < posting_length; i++)
            { 
                doc_ids[i] = reader.ReadUInt32();
                positions_length = reader.ReadInt32();
                positions[i] = new ushort[positions_length];

                for (int j = 0; j < positions_length; j++)
                {
                    positions[i][j] = reader.ReadUInt16();
                }
            }

            return new PositionalPostingList(doc_ids, positions);
        }

        private CompressedPositionalPostingList ReadCompressedPostingList(BinaryReader reader)
        {
            int posting_length;
            int stream_size;
            byte[] stream;

            posting_length = reader.ReadInt32();
            stream_size = reader.ReadInt32();

            stream = reader.ReadBytes(stream_size);

            return new CompressedPositionalPostingList(posting_length, stream);
        }

        public void ResetIndex()
        {
           // mDocuments = new Dictionary<uint,Document>();
            mWords = null;
            mPostingLists = null;
        }


        private static InversedPositionalIndex msInversedPositionalIndex;        

        private string[] mWords;
        private PositionalPostingList[] mPostingLists;

        /// <summary>
        /// Mapis document identifiers to documents.
        /// </summary>
      //  private Dictionary<uint, Document> mDocuments;
        private long[] mDocumentsPositions;

        private bool mPerformedStemming;
        private bool mPerformedStopWordsRemoval;
        private bool mPerformedLematization;
        private bool mPerformCompression;
    }
}
