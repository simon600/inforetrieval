using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Parser
{
    public class CharReader
    {
        public CharReader(Stream baseStream, int bufferSize)
        {
            mBufferSize = bufferSize;
            mBaseStream = baseStream;
            mEndOfStream = false;
            mChars = new char[mBufferSize];
            mBytes = new byte[mBufferSize];
            Position = 0;
        }

        public CharReader(Stream baseStream)            
        {
            mBufferSize = 1024;
            mBaseStream = baseStream;
            mEndOfStream = false;
            mChars = new char[mBufferSize];
            mBytes = new byte[mBufferSize];
            Position = 0;
        }

        public int Read()
        {
            char c;

            if (mCharNumber >= mNumberOfChars - 1)
            {
                if (mBaseStream.Position >= mBaseStream.Length)
                {
                    if (mCharNumber < mNumberOfChars)
                    {
                        c = mChars[mCharNumber];
                        mPosition += Encoding.UTF8.GetByteCount(mChars, mCharNumber, 1);
                        mCharNumber++;
                        return c;
                    }
                    else
                    {
                        mEndOfStream = true;
                        return -1;
                    }
                }
                else
                {
                    int bytes_read;
                    int size = Encoding.UTF8.GetByteCount(mChars, 0, mNumberOfChars - 1);
                    
                    
                    if (size > mBufferSize)
                        size = mBufferSize;

                    for (int i = 0; i < mBufferSize - size; i++)
                    {
                        mBytes[i] = mBytes[size + i];
                    }
                    bytes_read = mBaseStream.Read(mBytes, mBufferSize - size, size);
                    mNumberOfChars = Encoding.UTF8.GetChars(mBytes, 0, bytes_read + mBufferSize - size, mChars, 0);
                    mCharNumber = 0;
                }
            }

            c = mChars[mCharNumber];
            mPosition += Encoding.UTF8.GetByteCount(mChars, mCharNumber, 1);
             

            mCharNumber++;
            return c;
        }

        public int Peek()
        {
            char c;
            if (mCharNumber >= mNumberOfChars - 1)
            {
                if (mBaseStream.Position >= mBaseStream.Length)
                {
                    if (mCharNumber < mNumberOfChars)
                    {
                        c = mChars[mCharNumber];
                        return c;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    int bytes_read;
                    int size = Encoding.UTF8.GetByteCount(mChars, 0, mNumberOfChars - 1);
                    for (int i = 0; i < mBufferSize - size; i++)
                    {
                        mBytes[i] = mBytes[size + i];
                    }
                    bytes_read = mBaseStream.Read(mBytes, mBufferSize - size, size);
                    mNumberOfChars = Encoding.UTF8.GetChars(mBytes, 0, bytes_read + mBufferSize - size, mChars, 0);
                    mCharNumber = 0;
                }
            }

            c = mChars[mCharNumber];
            return c;
        }

        public string ReadLine()
        {
            if (EndOfStream)
            {
                return null;
            }

            string line = "";
            int c;
            while ((c = Read()) != -1 && (char)c != '\n')
            {
                line += (char)c;
            }

            return line;
        }

        public void Close()
        {
            mBaseStream.Close();
        }

        public bool EndOfStream
        {
            get
            {
                return mEndOfStream;
            }
        }

        public long Position
        {
            get
            {
                return mPosition;
            }
            set
            {
                int bytes_read;
                mPosition = value;
                mBaseStream.Position = mPosition;
                bytes_read = mBaseStream.Read(mBytes, 0, mBufferSize);
                mNumberOfChars = Encoding.UTF8.GetChars(mBytes, 0, bytes_read, mChars, 0);
                mCharNumber = 0;
            }
        }



        private long mPosition;
        private int mBufferSize;
        private byte[] mBytes;
        private int mCharNumber;
        private int mNumberOfChars;
        private char[] mChars;
        private bool mEndOfStream;
        private Stream mBaseStream;
    }
}
