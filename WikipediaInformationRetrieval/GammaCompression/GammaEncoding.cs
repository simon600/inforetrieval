using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GammaCompression
{
    /// <summary>
    /// Elias gamma coding for positive integers
    /// </summary>
    public class GammaEncoding
    {
        /// <summary>
        /// Write value in Elias gamma code on stream
        /// </summary>
        /// <param name="value">Integer to compressed</param>
        /// <param name="stream">Place to write compressed value</param>
        public static void CodeInt(uint value, BitStreamWriter stream)
        {
            string binary_value = Convert.ToString(value+1, 2);
            
            //unary code for length of binary_value
            for (int i = 1; i < binary_value.Length; i++)
                stream.SetNextBit(false);

            for (int i = 0; i < binary_value.Length; i++)
                stream.SetNextBit(binary_value[i] == '1');
        }

        /// <summary>
        /// Decompressed integer from bit stream
        /// </summary>
        /// <param name="stream">Bit stream to read value from</param>
        /// <returns>Next integer coded on stream</returns>
        public static uint DecodeInt(BitStreamReader stream)
        {
            int len = 0; 
            uint value = 1;

            while (stream.GetNextBit() == false)
                len += 1;

            //przeczytalismy 1

            for (int i = 0; i < len; i++)
                if (stream.GetNextBit())
                    value = (value<<1) + 1;         //binary_value += '1';//msBinaryValue.Append('1');
                else value <<= 1;                   //binary_value += '0'; // msBinaryValue.Append('0');

            return value - 1;                       //Convert.ToUInt32(binary_value, 2) - 1;
        }

        private static StringBuilder msBinaryValue = new StringBuilder();
    }
}
