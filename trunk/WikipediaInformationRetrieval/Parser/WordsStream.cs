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
        /// <param name="baseStream">A stream from which
        /// WordsStream reads dats.</param>
        public WordsStream(Stream baseStream)
        {
            mBaseStream = baseStream;
            mReader = new StreamReader(mBaseStream);
        }

        /// <summary>
        /// Reads next string from stream.
        /// </summary>
        /// <returns>Read string.</returns>
        public String Read()
        {                                   
            string read_string = "";
            char c;
        
            c = (char)mReader.Read();
            read_string += c;
            
            while (msSeparators.Contains(c))
            {
                c = (char)mReader.Read();
                read_string += c;
            }

            do
            {                
                c = (char)mReader.Read();
                read_string += c;                
            } while (!EndOfStream &&
                !msSeparators.Contains(c));

            while (msSeparators.Contains((char)mReader.Peek()))
            {
                c = (char)mReader.Read();
                read_string += c;
            }

            UpdatePosition(read_string);

            return read_string.Trim(msSeparators.ToCharArray());
        }

        /// <summary>
        /// Closes WordsStream.
        /// </summary>
        public void Close()
        {
            mReader.Close();
        }

        /// <summary>
        /// True if end of stream.
        /// </summary>
        public bool EndOfStream
        {
            get
            {
                return mReader.EndOfStream;
            }
        }

        /// <summary>
        /// Gets current position in the underlying stream.
        /// </summary>
        public long Position
        {
            get
            {
              //  mPosition += Encoding.UTF8.GetByteCount(mReadedString.ToString().ToCharArray());
              //  mReadedString.Remove(0, mReadedString.Length);
              //  return mPosition;
                return mPosition;
            }
            //set
            //{
            //    mReader.BaseStream.Position = value;
            //}
        }

        private void UpdatePosition(string s)
        {
            char[] chars = s.ToCharArray();            
            mPosition += UnicodeEncoding.UTF8.GetByteCount(chars);
        }

     //   private CharReader mReader;
        private StreamReader mReader; 
        private Stream mBaseStream;
        private static string msSeparators = " \n\t\r";
        long mPosition;
    }
}
