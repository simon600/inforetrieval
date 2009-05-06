using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GammaCompression
{
    public class GammaEncoding
    {
        public static void CodeInt(uint value, BitStream stream)
        {
            string binary_value = Convert.ToString(value+1, 2);
            
            //unary code for length of binary_value
            for (int i = 1; i < binary_value.Length; i++)
                stream.SetNextBit(false);

            for (int i = 0; i < binary_value.Length; i++)
                stream.SetNextBit(binary_value[i] == '1');

        }

        public static uint DecodeInt(BitStream stream)
        {
            int len = 0;

            while (stream.GetNextBit() == false)
                len += 1;

            string binary_value = "1";
            
            for (int i = 0; i < len; i++)
                if (stream.GetNextBit())
                    binary_value += '1';
                else binary_value += '0';

            return Convert.ToUInt32(binary_value, 2) -1;
        }

    }
}
