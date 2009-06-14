using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;

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
                output_file = "converted.bin";
            }
                                    
            MakeDictionary("D:\\ZAJECIA\\WyszukiwanieInformacji\\projekt1\\morfologik_do_wyszukiwarek.txt");
            WriteMorphologic("D:\\ZAJECIA\\WyszukiwanieInformacji\\projekt1\\morfologik.bin");
            
        }// Main

        static void MakeDictionary(string filename)
        {
            StreamReader reader = new StreamReader(filename);
            String[] line;
            Dictionary<string, uint> word_map = new Dictionary<string, uint>();

            uint number = 0;

            while (!reader.EndOfStream)
            {
                line = reader.ReadLine().Split(' ');
                if (line.Length >= 2)
                {
                    for (int i = 1; i < line.Length; i++)
                    {
                        if (!word_map.ContainsKey(line[i]))
                        {
                            word_map.Add(line[i], number);
                            words_list.Add(line[i]);
                            number++;
                        }
                    }
                    
                }
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            List<uint> list;

            while (!reader.EndOfStream)
            {
                line = reader.ReadLine().Split(' ');
                if (line.Length >= 2)
                {
                    if (!morphologic.ContainsKey(line[0]))
                    {
                        morphologic[line[0]] = new List<uint>();
                    }
                    list = morphologic[line[0]];
                    
                    for (int i = 1; i < line.Length; i++)
                    {
                        if (!list.Contains(word_map[line[i]]))
                        {
                            list.Add(word_map[line[i]]);
                        }
                    }
                }
            }

            reader.Close();
        }// MakeDictionary

        static void WriteMorphologic(string filename)
        {            

            FileStream fstream = new FileStream(filename, FileMode.OpenOrCreate);
            BinaryWriter binary_writer = new BinaryWriter(fstream);
            //BinaryFormatter formatter = new BinaryFormatter();
            
            // serialize words_list
            //formatter.Serialize(fstream, words_list.ToArray());
            binary_writer.Write(words_list.Count); // write length of words list
            foreach (string word in words_list)
            {
                binary_writer.Write(word);
            }

            
            //formatter.Serialize(fstream, morphologic.Keys.ToArray<string>());

            //List<uint>[] lists = morphologic.Values.ToArray<List<uint>>();

            //uint[][] base_form_numbers = new uint[lists.Length][];

            //binary_writer.Write(lists.Length);
            //for (uint i = 0; i < lists.Length; i++)
            //{
            //    formatter.Serialize(fstream,lists[i].ToArray());
            //}

            //formatter.Serialize(fstream, base_form_numbers);

            // serialize dictionary
            binary_writer.Write(morphologic.Count); // number of enries
            foreach (KeyValuePair<string, List<uint>> pair in morphologic)
            {
                binary_writer.Write(pair.Key); // write word
                // serialize base forms numbers                
                binary_writer.Write(pair.Value.Count);
                // write base forms
                foreach (uint num in pair.Value)
                {
                    binary_writer.Write(num);
                }                
            }

            //binary_writer.Close();
        }// WriteMorphologic

        static SortedDictionary<string, List<uint>> morphologic =
            new SortedDictionary<string, List<uint>>();// maps words to bases        

        static List<string> words_list = new List<string>();
    }
}
