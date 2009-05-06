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
        private SearchEngine searcher;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            searcher =
                new SearchEngine("D:\\ZAJECIA\\WyszukiwanieInformacji\\projekt1\\wikipedia.txt",
                "D:\\ZAJECIA\\WyszukiwanieInformacji\\projekt1\\morfologik.bin",
                "D:\\ZAJECIA\\WyszukiwanieInformacji\\projekt1\\compressedIndex.bin",
                false);

        }

        private void button2_Click(object sender, EventArgs e)
        {
          //  searcher.ReadTitles();
            //BooleanQuery query = new BooleanQuery(this.textBox1.Text);
            //this.textBox2.Text = query.QueryNormalForm;
            //PositionalPostingList posting = query.ProcessQuery();
            //if(posting!= null)
            //    this.textBox2.Text += posting.DocumentIds.Length.ToString();

            //PhraseQuery query = new PhraseQuery(this.textBox1.Text);
            //this.textBox2.Text = query.QueryNormalForm;

            //PositionalPostingList result = query.ProcessQuery();

            List<string> answer = searcher.SearchFor(this.textBox1.Text);
           
            Console.WriteLine(searcher.ResponseTime);
            Console.WriteLine(searcher.ResponseTimeInSeconds);
          
        }

        private void button3_Click(object sender, EventArgs e)
        {
          
        }

        private void button4_Click(object sender, EventArgs e)
        {
           
          

            Console.WriteLine(GC.GetTotalMemory(false));

        }
    }
}
