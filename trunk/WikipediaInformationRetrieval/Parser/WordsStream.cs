using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Parser
{
    /// <summary>
    /// An abstract class representing stream of words.
    /// Reads data from stream and returns separate words in it.
    /// </summary>
    public class WordsStream
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="UnderlyingStream">A stream from which
        /// WordsStream reads dats.</param>
        public WordsStream(Stream UnderlyingStream)
        {
            mStreamReader = new StreamReader(UnderlyingStream);
        }

        /// <summary>
        /// Reads next string from stream.
        /// </summary>
        /// <returns>Read string.</returns>
        public String Read()
        {
            string read_string = "";
            char c;

            c = (char)mStreamReader.Peek();

            while (!msSeparators.Contains(c))
            {
                mStreamReader.Read();
                c = (char)mStreamReader.Peek();
            }

            while (!mStreamReader.EndOfStream &&
                !msSeparators.Contains(c))
            {
                read_string += c;
                c = (char)mStreamReader.Read();
            }

            return read_string;
        }

        /// <summary>
        /// Closes WordsStream.
        /// </summary>
        public void Close()
        {
            mStreamReader.Close();
        }

        /// <summary>
        /// True if end of stream.
        /// </summary>
        public bool EndOfStream
        {
            get
            {
                return mStreamReader.EndOfStream;
            }
        }

        private StreamReader mStreamReader;
        private static string msSeparators = " \n\t";
    }
}
