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

            BitStream bitStream = new BitStream();

            for (uint i = 0; i < 100; i++)
            {

                GammaEncoding.CodeInt(i, bitStream);
                Console.WriteLine(bitStream.Length+" size "+bitStream.StreamSize);
            }

            bitStream.SetOnStart();
           
         

            while (!bitStream.EndOfStream)
            {
                uint val = GammaEncoding.DecodeInt(bitStream);
                Console.WriteLine(val);
            }
         
        }
    }
}
