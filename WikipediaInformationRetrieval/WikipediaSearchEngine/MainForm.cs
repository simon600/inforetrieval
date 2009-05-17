using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace WikipediaSearchEngine
{
    public partial class MainForm : Form
    {
        private SearchEngine searcher;
        private List<string> results;
        private WaitForm waitform;
        private PrepareForm prepareform;
        private delegate void FinishLoadingDelegate();
        private string fileWithQueryPath;
        private string resultFilePath;

        private string query ="";

        public MainForm()
        {
            InitializeComponent();
            
            prepareform = new PrepareForm();
            prepareform.ShowDialog(this);

            if (prepareform.DialogResult == DialogResult.OK)
                PrepareSearcher();

            else throw new NotPreparedException();
        }

        private void PrepareSearcher()
        {
            waitform = new WaitForm("Trwa wczytywanie");
            
            Thread thread = new Thread(Loading);
            thread.Start();

            waitform.ShowDialog();
        }

        private void Loading()
        {
            try
            {
               searcher = new SearchEngine(prepareform.SourcePath,
               prepareform.MorphologicPath, prepareform.IndexPath, true);

               prepareform = null;
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    exception.Message,
                    "Błąd",
                    MessageBoxButtons.OK);
            }

            BeginInvoke(new FinishLoadingDelegate(FinishLoading));
        }

        private void ProcessFileWithQueries()
        {
            try
            {
                searcher.QueriesFromFile(fileWithQueryPath, resultFilePath);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    exception.Message,
                    "Błąd",
                    MessageBoxButtons.OK);
            }

            BeginInvoke(new FinishLoadingDelegate(FinishLoading));
        }

        private void FinishLoading()
        {
            waitform.Close();
            waitform.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DoSearching();
        }

        private void DoSearching()
        {
            if (query == queryTextBox.Text.Trim())
                return;

            resultTextBox.Text = "";

            query = queryTextBox.Text.Trim();
            if (query == null || query.Length == 0)
                return;

            this.UseWaitCursor = true;

            results = searcher.SearchFor(query);

            foreach (string result in results)
                resultTextBox.Text += result + "\r\n\r\n";

            resultsCount.Text = "znaleziono dokumentów: " + results.Count;
            responseTimeLabel.Text = "czas odpowiedzi: " + Decimal.Round((decimal)searcher.ResponseTimeInSeconds, 3).ToString();

            this.UseWaitCursor = false;
        }

        private void queryTextBox_Enter(object sender, EventArgs e)
        {
            DoSearching();
        }

        private void otwórzPlikZZapytaniamiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                fileWithQueryPath = openFileDialog.FileName;  
        }

        private void wskażPlikWynikowyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                resultFilePath = saveFileDialog.FileName;

                if (fileWithQueryPath == null || fileWithQueryPath.Length == 0)
                {
                    MessageBox.Show(
                       "Nie wybrano pliku z zapytaniami.",
                       "Błąd",
                       MessageBoxButtons.OK);

                    return;
                }

                waitform = new WaitForm("Trwa wyszukiwanie");

                Thread thread = new Thread(ProcessFileWithQueries);
                thread.Start();

                waitform.ShowDialog();
            }
        }
    }
}
