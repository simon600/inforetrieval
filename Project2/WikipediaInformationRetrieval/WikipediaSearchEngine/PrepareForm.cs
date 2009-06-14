using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WikipediaSearchEngine
{
    public partial class PrepareForm : Form
    {
        public PrepareForm()
        {
            InitializeComponent();
        }

        public string SourcePath
        {
            get
            {
                return sourceTextBox.Text.Trim();
            }
        }

        public string IndexPath
        {
            get
            {
                return indexTextBox.Text.Trim();
            }
        }

        public string MorphologicPath
        {
            get
            {
                return morphologicTextBox.Text.Trim();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!File.Exists(sourceTextBox.Text))
            {
                MessageBox.Show("Podano niepoprawną ścieżkę do pliku źródłowego.",
                    "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (!File.Exists(morphologicTextBox.Text))
            {
                MessageBox.Show("Podano niepoprawną ścieżkę do słownika morfologicznego.",
                   "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (!File.Exists(indexTextBox.Text))
            {
                MessageBox.Show("Podano niepoprawną ścieżkę do pliku z indeksem.",
                   "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
        }

        private void sourceButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                sourceTextBox.Text = openFileDialog1.FileName;
        }

        private void morphButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                morphologicTextBox.Text = openFileDialog1.FileName;
        }

        private void indexButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                indexTextBox.Text = openFileDialog1.FileName;

        }
    }
}
