using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace WikipediaSearchEngine
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
           // Application.SetCompatibleTextRenderingDefault(false);
           // Application.Run(new Form1());

            //StreamReader read = new StreamReader(new FileStream("D:\\ZAJECIA\\WyszukiwanieInformacji\\projekt1\\wikipedia_dla_wyszukiwarek.txt", FileMode.Open));
            //StreamWriter writer = new StreamWriter(new FileStream("D:\\ZAJECIA\\WyszukiwanieInformacji\\projekt1\\wikipedia2.txt", FileMode.Create), read.CurrentEncoding);

            //long lines = 0;
            //long articles = 0;
            //long max = 7000000;

            //while (!read.EndOfStream && lines < max)
            //{
            //    string line = read.ReadLine();
               
            //    if(line.Contains("##TITLE##"))
            //        articles++;

            //    writer.WriteLine(line);
               
            //    lines++;
            //}
            
            //read.Close();
            //writer.Close();

            //Console.WriteLine("Artykulow "+articles);
            //Console.WriteLine("linii "+lines);

           
        }
    }
}
