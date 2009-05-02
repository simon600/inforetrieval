using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Security.Cryptography;


namespace WikipediaIndexCreator
{
    //class Pair
    //{
    //    public string first;
    //    public bool second;
    //}

    public partial class Form1 : Form
    {
        MemoryStream memstream = new MemoryStream();
        MemoryStream memstr = new MemoryStream();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GZipStream gzip;
            memstr.Seek(0, SeekOrigin.Begin);
            BinaryReader reader = new BinaryReader(memstr);
            MemoryStream mem = new MemoryStream();
            mem.Write(reader.ReadBytes((int)memstr.Length), 0, (int)memstr.Length);            
            mem.Seek(0, SeekOrigin.Begin);
            gzip = new GZipStream(mem, CompressionMode.Decompress);
            List<uint> list = new List<uint>();
            reader = new BinaryReader(gzip);
            byte[] bytes;
            bytes = reader.ReadBytes((int)mem.Length);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MemoryStream mem = new MemoryStream();
            GZipStream gzip = new GZipStream(mem, CompressionMode.Compress, true);
            MemoryStream mem2 = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(mem2);
            
            
            for (uint i = 0; i < 100; i++)
            {
                writer.Write(i);
            }

            writer = new BinaryWriter(gzip);
            mem2.Seek(0, SeekOrigin.Begin);
            BinaryReader re = new BinaryReader(mem2);
            writer.Write(re.ReadBytes((int)mem2.Length));

            writer = new BinaryWriter(memstr);
            //writer.Write(mem.Length);
            mem.Seek(0, SeekOrigin.Begin);
            BinaryReader reader = new BinaryReader(mem);
            
            writer.Write(reader.ReadBytes((int)mem.Length));
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Stream output = new FileStream("t.bin", FileMode.OpenOrCreate);
            Stream memstr = new MemoryStream();
            GZipStream gzip = new GZipStream(memstr, CompressionMode.Compress, true);

            List<int> list = new List<int>();
            for (int i = 0; i < 10000; i++)
            {
                list.Add(i % 1000);
            }

            
            MemoryStream mem = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(mem); 

            foreach (int i in list)
            {
                writer.Write(i);
            }

            writer = new BinaryWriter(gzip);
            mem.Seek(0, SeekOrigin.Begin);
            BinaryReader reader = new BinaryReader(mem);

            writer.Write(reader.ReadBytes((int)mem.Length));


            writer.Close();

            memstr.Seek(0, SeekOrigin.Begin);

            writer = new BinaryWriter(output);
            reader = new BinaryReader(memstr);
            writer.Write(reader.ReadBytes((int)memstr.Length));

            writer.Close();

            Stream input = new FileStream("t.bin", FileMode.Open);

            reader = new BinaryReader(input);            

            MemoryStream mem2 = new MemoryStream();
            writer = new BinaryWriter(mem2);
            writer.Write(reader.ReadBytes((int)input.Length));
            mem2.Seek(0, SeekOrigin.Begin);
            gzip = new GZipStream(mem2, CompressionMode.Decompress);
            reader = new BinaryReader(gzip);

            List<int> inlist = new List<int>();

            for (int i = 0; i < 10000; i++)
            {
                inlist.Add(reader.ReadInt32());
            }                    
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Morphologic.MorphologicDictionary morph = Morphologic.MorphologicDictionary.Get();
            morph.ReadFromFile("E:\\Studia\\Wyszukiwanie Informacji\\Pracownia\\Projekt\\morphologic.bin");

            FileStream fstream = new FileStream("E:\\Studia\\Wyszukiwanie Informacji\\Pracownia\\Projekt\\wikipedia.txt", FileMode.Open);
            FileStream output = new FileStream("E:\\Studia\\Wyszukiwanie Informacji\\Pracownia\\Projekt\\index.bin", FileMode.OpenOrCreate);
            //MessageBox.Show(morph["zaklinaczkę"][0]);

            IndexCreator.Get().CreateIndexFromStream(fstream, output);
        }
    }
}
