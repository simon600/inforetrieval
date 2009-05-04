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
using System.Threading;
using Morphologic;


namespace WikipediaIndexCreator
{
    public partial class MainWindow : Form
    {                
        private delegate void FinishLoadingDelegate();
        bool morphologicRead;
        Stream sourceIndexStream;
        Stream outputIndexStream;

        public MainWindow()
        {
            morphologicRead = false;
            InitializeComponent();
        }

        private void otwórzPlikDoZindeksowaniaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toIndexOpenFileDialog.ShowDialog();
        }

        private void otwórzSłownikMorfologicznyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            morphologicOpenFileDilalog.ShowDialog();
        }

        private void morphologicOpenFileDilalog_FileOk(object sender, CancelEventArgs e)
        {
            string filename = morphologicOpenFileDilalog.FileName;

            if (File.Exists(filename))
            {
                BeginLoading("Wczytywanie słownika morfologicznego");

                Thread thread = new Thread(
                    LoadMorphologic);
                thread.Start();
            }
        }

        private void LoadMorphologic()
        {            
            try
            {                
                MorphologicDictionary morphologic =
                    MorphologicDictionary.Get();

                morphologic.ReadFromFile(
                    morphologicOpenFileDilalog.FileName);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    exception.Message,
                    "Błąd",
                    MessageBoxButtons.OK);
            }

            BeginInvoke(new FinishLoadingDelegate(
                FinishLoading));
        }

        private void CreateIndex()
        {            
            try
            {
                IndexCreator.Get().CreateIndexFromStream(
                    sourceIndexStream,
                    outputIndexStream);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    exception.Message,
                    "Błąd",
                    MessageBoxButtons.OK);
            }

            BeginInvoke(new FinishLoadingDelegate(
                FinishLoading));
        }

        private void BeginLoading(string labelText)
        {
            progressBar.Visible = true;
            progressBarLabel.Visible = true;
            progressBarLabel.Text = labelText;
            timer.Enabled = true;
            timer.Start();
        }

        private void FinishLoading()
        {            
            morphologicRead = true;
            progressBar.Visible = false;
            progressBarLabel.Visible = false;
            progressBar.Value = progressBar.Minimum;
            timer.Stop();
            timer.Enabled = false;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            progressBar.Value++;
            if (progressBar.Value >= progressBar.Maximum)
            {
                progressBar.Value = progressBar.Minimum;
            }
        }

        private void zapszPlikIndeksuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveIndexFileDialog.ShowDialog();
        }

        private void saveIndexFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            if (!morphologicRead)
            {
                MessageBox.Show(
                    "Najpierw należy wczytać słownik morfologiczny");
                return;
            }

            string sourceFilename = toIndexOpenFileDialog.FileName;

            if (!File.Exists(sourceFilename))
            {
                MessageBox.Show("Należy wybrać plik do zindeksowania");
                return;
            }

            string destFilename = saveIndexFileDialog.FileName;

            //if (File.Exists(destFilename))
            //{
            //    if (MessageBox.Show(
            //        "Plik istnieje. Nadpisać?",
            //        "Plik istnieje", MessageBoxButtons.YesNo)
            //        == DialogResult.No)
            //    {
            //        return;
            //    }
            //}

            outputIndexStream =
                new FileStream(destFilename, FileMode.Create);
            sourceIndexStream =
                new FileStream(sourceFilename, FileMode.Open);

            if (sourceIndexStream.CanRead && outputIndexStream.CanWrite)
            {
                IndexCreator creator = IndexCreator.Get();

                string separator = delimiterTextBox.Text.Trim();
                if (separator.Length > 0)
                {
                    creator.ArticleSeparator = separator;
                }
                creator.PerformLematization = lematyzationCheckBox.Checked;
                creator.PerformStemming = stemmingCheckBox.Checked;
                creator.PerformStopWordsRemoval = stopWordsCheckBox.Checked;

                BeginLoading("Tworzenie indeksu");
                Thread thread = new Thread(CreateIndex);
                thread.Start();                
            }
        }
    }
}
