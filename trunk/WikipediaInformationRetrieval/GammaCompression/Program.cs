using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GammaCompression
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            BitStreamWriter bitStream = new BitStreamWriter();
            int k = 50;
            for (uint i = 0; i < k; i++)
            {
                GammaEncoding.CodeInt(i, bitStream);
                Console.WriteLine(bitStream.Length+" size "+bitStream.StreamSize);
            }

            byte[] bytes = bitStream.Bytes;

            BitStreamReader bitReader = new BitStreamReader(bytes);

            for( uint i = 0; i<k; i++)
            {
                uint val = GammaEncoding.DecodeInt(bitReader);
                Console.WriteLine(val);
            }
         
        }
    }
}
