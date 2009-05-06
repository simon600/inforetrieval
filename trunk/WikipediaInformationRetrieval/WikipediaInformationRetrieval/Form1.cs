using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using WikipediaSearchEngine;
using Morphologic;
using InversedIndex;

namespace WikipediaInformationRetrieval
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FileStream f = new FileStream("D:\\ZAJECIA\\WyszukiwanieInformacji\\projekt1\\compressedIndex.bin", FileMode.Open);
            InversedIndex.InversedPositionalIndex index = InversedIndex.InversedPositionalIndex.Get();
            index.ReadFromStream(f);

            f.Close();

            int i = index.Documents.Count;
            PositionalPostingList posting = index.PostingList("2006r");

            CompressedPositionalPostingList cposting = posting.Compress();


            cposting.Decompress();


            MorphologicDictionary morphologic =
                    MorphologicDictionary.Get();

            morphologic.ReadFromFile("D:\\ZAJECIA\\WyszukiwanieInformacji\\projekt1\\morfologik.bin");
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
            //BooleanQuery query = new BooleanQuery(this.textBox1.Text);
            //this.textBox2.Text = query.QueryNormalForm;
            //PositionalPostingList posting = query.ProcessQuery();
            //if(posting!= null)
            //    this.textBox2.Text += posting.DocumentIds.Length.ToString();

            PhraseQuery query = new PhraseQuery(this.textBox1.Text);
            this.textBox2.Text = query.QueryNormalForm;

            PositionalPostingList result = query.ProcessQuery();
        }
    }
}
