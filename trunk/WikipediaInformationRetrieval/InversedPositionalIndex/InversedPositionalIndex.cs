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
                return mPostingLists[index];
            }
            else
            {                
                return new PositionalPostingList();
            }
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

            mWords = new string[size];
            mPostingLists = new PositionalPostingList[size];

            // read from stream
            for (int i = 0; i < size; i++)
            {
                mWords[i] = reader.ReadString();
                if (i > 88700)
                {
                    string word = mWords[i];
                }
                mPostingLists[i] = ReadPostingList(reader);
            }

            // read documents
            uint id;
            long position;

            while (stream.Position < stream.Length)
            {
                id = (uint)reader.ReadInt32();
                position = reader.ReadInt64();

                mDocuments.Add(id, new Document(id, position));
            }
        }        

        /// <summary>
        /// Gets documents.
        /// </summary>
        public Dictionary<uint, Document> Documents
        {
            get
            {
                return mDocuments;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        private InversedPositionalIndex()
        {
            mDocuments = new Dictionary<uint, Document>();
        }

        private PositionalPostingList ReadPostingList(BinaryReader reader)
        {
            int posting_length;
            int positions_length;

            uint[] doc_ids;
            uint[][] positions;

            posting_length = reader.ReadInt32();

            doc_ids = new uint[posting_length];
            positions = new uint[posting_length][];

            for (int i = 0; i < posting_length; i++)
            {
                doc_ids[i] = (uint)reader.ReadInt32();
                positions_length = reader.ReadInt32();
                positions[i] = new uint[positions_length];

                for (int j = 0; j < positions_length; j++)
                {
                    positions[i][j] = (uint)reader.ReadInt32();
                }
            }

            return new PositionalPostingList(doc_ids, positions);
        }

        private static InversedPositionalIndex msInversedPositionalIndex;        

        private string[] mWords;
        private PositionalPostingList[] mPostingLists;

        /// <summary>
        /// Mapis document identifiers to documents.
        /// </summary>
        private Dictionary<uint, Document> mDocuments;
    }
}
