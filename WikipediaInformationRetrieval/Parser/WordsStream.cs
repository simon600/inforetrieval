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
        public WordsStream(Stream underlyingStream)
        {
            mStreamReader = new StreamReader(underlyingStream);
            mPosition = underlyingStream.Position;
        }

        /// <summary>
        /// Reads next string from stream.
        /// </summary>
        /// <returns>Read string.</returns>
        public String Read()
        {
            string read_string = "";
            char c;
            
            c = (char)mStreamReader.Read();
            UpdatePosition(c);            

            while (msSeparators.Contains(c))
            {
                c = (char)mStreamReader.Read();
                UpdatePosition(c);
            }

            do
            {
                read_string += c;
                c = (char)mStreamReader.Read();
                UpdatePosition(c);
            } while (!mStreamReader.EndOfStream &&
                !msSeparators.Contains(c));

            while (msSeparators.Contains((char)mStreamReader.Peek()))
            {                
                UpdatePosition((char)mStreamReader.Read());
            }

            return read_string;
        }

        /// <summary>
        /// Closes WordsStream.
        /// </summary>
        public void Close()
        {
            mStreamReader.Close();
            mPosition = 0;
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

        /// <summary>
        /// Gets current position in the underlying stream.
        /// </summary>
        public long Position
        {
            get
            {
                return mPosition;
            }
            set
            {
                mPosition = value;
                mStreamReader.BaseStream.Seek(mPosition, SeekOrigin.Begin);
            }
        }

        private void UpdatePosition(char c)
        {
            //mPosition++;
            char[] chars = new char[1];
            chars[0] = c;   
            mPosition += UnicodeEncoding.UTF8.GetByteCount(chars);            
        }

        private StreamReader mStreamReader;
        private static string msSeparators = " \n\t\r";
        private long mPosition;
    }
}
