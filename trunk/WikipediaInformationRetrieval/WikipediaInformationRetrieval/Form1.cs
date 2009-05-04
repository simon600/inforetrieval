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
            FileStream f = new FileStream("D:\\ZAJECIA\\WyszukiwanieInformacji\\projekt1\\index2.bin", FileMode.Open);
            InversedIndex.InversedPositionalIndex index = InversedIndex.InversedPositionalIndex.Get();
            index.ReadFromStream(f);

            f.Close();

            int i = index.Documents.Count;
            index.PostingList("dziesiąty");

            MorphologicDictionary morphologic =
                    MorphologicDictionary.Get();

            morphologic.ReadFromFile("D:\\ZAJECIA\\WyszukiwanieInformacji\\projekt1\\morphologic.bin");
            List<string> l = morphologic["dzisiąty"];
            l = morphologic["astronomia"];

           
        }

        private void button2_Click(object sender, EventArgs e)
        {

            BooleanQuery query = new BooleanQuery(this.textBox1.Text);
            this.textBox2.Text = query.QueryNormalForm;
            PositionalPostingList posting = query.ProcessQuery(); 


        }
    }
}
