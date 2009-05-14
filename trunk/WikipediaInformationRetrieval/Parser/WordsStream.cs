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
        /// <param name="bufferSize">A stream from which
        /// WordsStream reads dats.</param>
        /// <param name="baseStream">Size of the buffer</param>
        public WordsStream(Stream baseStream, int bufferSize)
        {
            mBaseStream = baseStream;
            mCharReader = new CharReader(baseStream, bufferSize);
            mCharReader.Position = 0;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="baseStream">A stream from which
        /// WordsStream reads dats.</param>
        public WordsStream(Stream baseStream)
        {
            mBaseStream = baseStream;
            mCharReader = new CharReader(baseStream);
            mCharReader.Position = 0;
        }

        /// <summary>
        /// Reads next string from stream.
        /// </summary>
        /// <returns>Read string.</returns>
        public String Read()
        {                                   
            string read_string = "";
            char c;
            
            c = (char)mCharReader.Read();
            //UpdatePosition(c);            

            while (msSeparators.Contains(c))
            {
                c = (char)mCharReader.Read();
                //UpdatePosition(c);
            }

            do
            {
                read_string += c;
                c = (char)mCharReader.Read();
                //UpdatePosition(c);
            } while (!EndOfStream &&
                !msSeparators.Contains(c));

            while (msSeparators.Contains((char)mCharReader.Peek()))
            {
                mCharReader.Read();
                //UpdatePosition((char));
            }

            return read_string;
        }

        /// <summary>
        /// Closes WordsStream.
        /// </summary>
        public void Close()
        {
            mCharReader.Close();
        }

        /// <summary>
        /// True if end of stream.
        /// </summary>
        public bool EndOfStream
        {
            get
            {
                return mCharReader.EndOfStream;
            }
        }

        /// <summary>
        /// Gets current position in the underlying stream.
        /// </summary>
        public long Position
        {
            get
            {
                return mCharReader.Position;
            }
            set
            {
                mCharReader.Position = value;
            }
        }

        //private void UpdatePosition(char c)
        //{
        //    //mPosition++;
        //    char[] chars = new char[1];
        //    chars[0] = c;               
        //    mPosition += UnicodeEncoding.UTF8.GetByteCount(chars);            
        //}

        private CharReader mCharReader;
        
        private Stream mBaseStream;
        private static string msSeparators = " \n\t\r";
        //private long mPosition;
    }
}
