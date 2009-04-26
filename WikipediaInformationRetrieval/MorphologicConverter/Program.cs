using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace MorphologicConverter
{
    class Program
    {                
        static void Main(string[] args)
        {            
            string output_file;            

            if (args.Length < 1)
            {
                return;
            }

            if (args.Length >= 2)
            {
                output_file = args[1];
            }
            else
            {
                output_file = "converted.txt";
            }
                                    
            MakeDictionary(args[0]);
            WriteMorphologic(output_file);
            
        }// Main

        //static void MakeWordsMap(string filename)
        //{
        //    ulong word_nb = 0;
        //    StreamReader reader = new StreamReader(filename);
        //    string[] line;

        //    while (!reader.EndOfStream)
        //    {
        //        line = reader.ReadLine().Split(' ');
        //        if (line.Length >= 2)
        //        {
        //            foreach (string word in line)
        //            {
        //                if (!words_map.ContainsKey(word))
        //                {
        //                    words_map[word] = word_nb;
        //                    word_nb++;
        //                }                            
        //            }
        //        }
        //    }
        //    reader.Close();
        //}// MakeWrodsMap

        static void MakeDictionary(string filename)
        {
            StreamReader reader = new StreamReader(filename);
            string[] line;
            string word;
            string[] bases;

            while (!reader.EndOfStream)
            {
                line = reader.ReadLine().Split(' ');
                if (line.Length >= 2)
                {
                    word = line[0];
                    bases = new string[line.Length - 1];
                    for (int i = 1; i < line.Length; i++)
                    {
                        bases[i - 1] = line[i];
                    }                    
                    foreach (string base_word in bases)
                    {
                        if (!morphologic.ContainsKey(base_word))
                        {
                            morphologic[base_word] = new List<string>();
                        }
                        if (!morphologic[base_word].Contains(word))
                        {
                            morphologic[base_word].Add(word);
                        }
                    }
                }
            }

            reader.Close();
        }// MakeDictionary

        static void WriteMorphologic(string filename)
        {
            StreamWriter writer = new StreamWriter(filename);

            foreach (KeyValuePair<string, List<string>> pair in morphologic)
            {
                writer.Write(pair.Key);
                foreach (string word in pair.Value)
                {
                    writer.Write(" ");
                    writer.Write(word);
                }
                writer.WriteLine();
            }

            writer.Close();
        }// WriteMorphologic

        static Dictionary<string, List<string>> morphologic =
            new Dictionary<string, List<string>>();// maps words to bases
    }
}
