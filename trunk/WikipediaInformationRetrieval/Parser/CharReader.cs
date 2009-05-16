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
            mEncoding = Encoding.UTF8;
            mBufferSize = bufferSize;
            mBaseStream = baseStream;
            mEndOfStream = false;
            mByteNumber = 0;
            mBytes = new byte[mBufferSize];
            Position = 0;
        }

        public CharReader(Stream baseStream)            
        {
            mEncoding = Encoding.UTF8;
            mBufferSize = 1024;
            mBaseStream = baseStream;
            mByteNumber = 0;
            mEndOfStream = false;
            mBytes = new byte[mBufferSize];
            Position = 0;
        }

        public int Read()
        {            
            char[] c = null;
            bool sth = true;

            if (mByteNumber >= mBytesRead - 5)
            {
                if (mBaseStream.Position >= mBaseStream.Length)
                {
                    sth = false;
                    if (mByteNumber >= mBytesRead)
                    {
                        mEndOfStream = true;
                        return -1;
                    }
                    if (mBytes[mByteNumber] < 194)
                    {
                        c = mEncoding.GetChars(mBytes, mByteNumber, 1);
                        mByteNumber++;
                        mPosition++;
                    }
                    else if (mBytes[mByteNumber] < 224)
                    {
                        c = mEncoding.GetChars(mBytes, mByteNumber,
                            Math.Min(2, mBytesRead - mByteNumber));
                        mByteNumber += Math.Min(2, mBytesRead - mByteNumber);
                        mPosition += Math.Min(2, mBytesRead - mByteNumber);
                    }
                    else
                    {
                        c = mEncoding.GetChars(mBytes, mByteNumber,
                            Math.Min(3, mBytesRead - mByteNumber));
                        mByteNumber += Math.Min(3, mBytesRead - mByteNumber);
                        mPosition += Math.Min(3, mBytesRead - mByteNumber);
                    }
                }
                else
                {
                    for (int i = 0; i < mBytesRead - mByteNumber; i++)
                    {
                        mBytes[i] = mBytes[mByteNumber + i];
                    }
                    mBaseStream.Read(mBytes, mBytesRead - mByteNumber, mBufferSize - (mBytesRead - mByteNumber));
                    mByteNumber = 0;
                }
            }

            if (sth)
            {
                if (mBytes[mByteNumber] < 194)
                {
                    c = mEncoding.GetChars(mBytes, mByteNumber, 1);
                    mByteNumber++;
                    mPosition++;
                }
                else if (mBytes[mByteNumber] < 224)
                {
                    c = mEncoding.GetChars(mBytes, mByteNumber, 2);
                    mByteNumber += 2;
                    mPosition += 2;
                }
                else
                {
                    c = mEncoding.GetChars(mBytes, mByteNumber, 3);
                    mByteNumber += 3;
                    mPosition += 3;
                }

                if (c.Length > 1)
                {
                    throw new Exception("Błąd");
                }
            }

            return c[0];
        }

        public int Peek()
        {
            char[] c = null;
            bool sth = true;

            if (mByteNumber >= mBytesRead - 5)
            {
                if (mBaseStream.Position >= mBaseStream.Length)
                {
                    sth = false;
                    if (mByteNumber >= mBytesRead)
                    {
                        return -1;
                    }
                    if (mBytes[mByteNumber] < 194)
                    {
                        c = mEncoding.GetChars(mBytes, mByteNumber, 1);
                    }
                    else if (mBytes[mByteNumber] < 224)
                    {
                        c = mEncoding.GetChars(mBytes, mByteNumber,
                            Math.Min(2, mBytesRead - mByteNumber));
                    }
                    else
                    {
                        c = mEncoding.GetChars(mBytes, mByteNumber,
                            Math.Min(3, mBytesRead - mByteNumber));
                    }
                }
                else
                {
                    for (int i = 0; i < mBytesRead - mByteNumber; i++)
                    {
                        mBytes[i] = mBytes[mByteNumber + i];
                    }
                    mBaseStream.Read(mBytes, mBytesRead - mByteNumber, mBufferSize - (mBytesRead - mByteNumber));
                    mByteNumber = 0;
                }
            }

            if (sth)
            {
                if (mBytes[mByteNumber] < 194)
                {
                    c = mEncoding.GetChars(mBytes, mByteNumber, 1);
                }
                else if (mBytes[mByteNumber] < 224)
                {
                    c = mEncoding.GetChars(mBytes, mByteNumber, 2);
                }
                else
                {
                    c = mEncoding.GetChars(mBytes, mByteNumber, 3);
                }

                if (c.Length > 1)
                {
                    throw new Exception("Błąd");
                }
            }

            return c[0];
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
                mPosition = value;
                mBaseStream.Position = mPosition;
                mBytesRead = mBaseStream.Read(mBytes, 0, mBufferSize);
                mByteNumber = 0;
            }
        }

        private Encoding mEncoding;
        private long mPosition;
        private int mBufferSize;
        private int mByteNumber;
        private int mBytesRead;
        private byte[] mBytes;
        private bool mEndOfStream;
        private Stream mBaseStream;
    }
}
