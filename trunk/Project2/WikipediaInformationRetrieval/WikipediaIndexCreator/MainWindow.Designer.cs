namespace WikipediaIndexCreator
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.stemmingCheckBox = new System.Windows.Forms.CheckBox();
            this.stopWordsCheckBox = new System.Windows.Forms.CheckBox();
            this.lematyzationCheckBox = new System.Windows.Forms.CheckBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.plikToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.otwórzPlikDoZindeksowaniaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.otwórzSłownikMorfologicznyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zapszPlikIndeksuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.progressBarLabel = new System.Windows.Forms.Label();
            this.toIndexOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.delimiterTextBox = new System.Windows.Forms.TextBox();
            this.delimiterLabel = new System.Windows.Forms.Label();
            this.morphologicOpenFileDilalog = new System.Windows.Forms.OpenFileDialog();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.saveIndexFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.compressCheckBox = new System.Windows.Forms.CheckBox();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 231);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(268, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 0;
            this.progressBar.Visible = false;
            // 
            // stemmingCheckBox
            // 
            this.stemmingCheckBox.AutoSize = true;
            this.stemmingCheckBox.Checked = true;
            this.stemmingCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.stemmingCheckBox.Location = new System.Drawing.Point(12, 51);
            this.stemmingCheckBox.Name = "stemmingCheckBox";
            this.stemmingCheckBox.Size = new System.Drawing.Size(72, 17);
            this.stemmingCheckBox.TabIndex = 1;
            this.stemmingCheckBox.Text = "Stemming";
            this.stemmingCheckBox.UseVisualStyleBackColor = true;
            // 
            // stopWordsCheckBox
            // 
            this.stopWordsCheckBox.AutoSize = true;
            this.stopWordsCheckBox.Checked = true;
            this.stopWordsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.stopWordsCheckBox.Location = new System.Drawing.Point(12, 97);
            this.stopWordsCheckBox.Name = "stopWordsCheckBox";
            this.stopWordsCheckBox.Size = new System.Drawing.Size(79, 17);
            this.stopWordsCheckBox.TabIndex = 1;
            this.stopWordsCheckBox.Text = "Stop words";
            this.stopWordsCheckBox.UseVisualStyleBackColor = true;
            // 
            // lematyzationCheckBox
            // 
            this.lematyzationCheckBox.AutoSize = true;
            this.lematyzationCheckBox.Checked = true;
            this.lematyzationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.lematyzationCheckBox.Location = new System.Drawing.Point(12, 74);
            this.lematyzationCheckBox.Name = "lematyzationCheckBox";
            this.lematyzationCheckBox.Size = new System.Drawing.Size(85, 17);
            this.lematyzationCheckBox.TabIndex = 1;
            this.lematyzationCheckBox.Text = "Lematyzacja";
            this.lematyzationCheckBox.UseVisualStyleBackColor = true;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.plikToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(292, 24);
            this.menuStrip.TabIndex = 2;
            this.menuStrip.Text = "menuStrip1";
            // 
            // plikToolStripMenuItem
            // 
            this.plikToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.otwórzPlikDoZindeksowaniaToolStripMenuItem,
            this.otwórzSłownikMorfologicznyToolStripMenuItem,
            this.zapszPlikIndeksuToolStripMenuItem});
            this.plikToolStripMenuItem.Name = "plikToolStripMenuItem";
            this.plikToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.plikToolStripMenuItem.Text = "Plik";
            // 
            // otwórzPlikDoZindeksowaniaToolStripMenuItem
            // 
            this.otwórzPlikDoZindeksowaniaToolStripMenuItem.Name = "otwórzPlikDoZindeksowaniaToolStripMenuItem";
            this.otwórzPlikDoZindeksowaniaToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.otwórzPlikDoZindeksowaniaToolStripMenuItem.Text = "Otwórz plik do zindeksowania";
            this.otwórzPlikDoZindeksowaniaToolStripMenuItem.Click += new System.EventHandler(this.otwórzPlikDoZindeksowaniaToolStripMenuItem_Click);
            // 
            // otwórzSłownikMorfologicznyToolStripMenuItem
            // 
            this.otwórzSłownikMorfologicznyToolStripMenuItem.Name = "otwórzSłownikMorfologicznyToolStripMenuItem";
            this.otwórzSłownikMorfologicznyToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.otwórzSłownikMorfologicznyToolStripMenuItem.Text = "Otwórz słownik morfologiczny";
            this.otwórzSłownikMorfologicznyToolStripMenuItem.Click += new System.EventHandler(this.otwórzSłownikMorfologicznyToolStripMenuItem_Click);
            // 
            // zapszPlikIndeksuToolStripMenuItem
            // 
            this.zapszPlikIndeksuToolStripMenuItem.Name = "zapszPlikIndeksuToolStripMenuItem";
            this.zapszPlikIndeksuToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.zapszPlikIndeksuToolStripMenuItem.Text = "Zapisz plik indeksu";
            this.zapszPlikIndeksuToolStripMenuItem.Click += new System.EventHandler(this.zapszPlikIndeksuToolStripMenuItem_Click);
            // 
            // progressBarLabel
            // 
            this.progressBarLabel.AutoSize = true;
            this.progressBarLabel.Location = new System.Drawing.Point(12, 215);
            this.progressBarLabel.Name = "progressBarLabel";
            this.progressBarLabel.Size = new System.Drawing.Size(70, 13);
            this.progressBarLabel.TabIndex = 3;
            this.progressBarLabel.Text = "Wczytywanie";
            this.progressBarLabel.Visible = false;
            // 
            // delimiterTextBox
            // 
            this.delimiterTextBox.Location = new System.Drawing.Point(159, 74);
            this.delimiterTextBox.Name = "delimiterTextBox";
            this.delimiterTextBox.Size = new System.Drawing.Size(119, 20);
            this.delimiterTextBox.TabIndex = 4;
            this.delimiterTextBox.Text = "##TITLE##";
            // 
            // delimiterLabel
            // 
            this.delimiterLabel.AutoSize = true;
            this.delimiterLabel.Location = new System.Drawing.Point(156, 51);
            this.delimiterLabel.Name = "delimiterLabel";
            this.delimiterLabel.Size = new System.Drawing.Size(124, 13);
            this.delimiterLabel.TabIndex = 3;
            this.delimiterLabel.Text = "Dziel dokumenty według";
            // 
            // morphologicOpenFileDilalog
            // 
            this.morphologicOpenFileDilalog.FileOk += new System.ComponentModel.CancelEventHandler(this.morphologicOpenFileDilalog_FileOk);
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // saveIndexFileDialog
            // 
            this.saveIndexFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveIndexFileDialog_FileOk);
            // 
            // compressCheckBox
            // 
            this.compressCheckBox.AutoSize = true;
            this.compressCheckBox.Location = new System.Drawing.Point(12, 121);
            this.compressCheckBox.Name = "compressCheckBox";
            this.compressCheckBox.Size = new System.Drawing.Size(109, 17);
            this.compressCheckBox.TabIndex = 5;
            this.compressCheckBox.Text = "Kompresuj indeks";
            this.compressCheckBox.UseVisualStyleBackColor = true;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.compressCheckBox);
            this.Controls.Add(this.delimiterTextBox);
            this.Controls.Add(this.delimiterLabel);
            this.Controls.Add(this.progressBarLabel);
            this.Controls.Add(this.lematyzationCheckBox);
            this.Controls.Add(this.stopWordsCheckBox);
            this.Controls.Add(this.stemmingCheckBox);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainWindow";
            this.Text = "Kreator indeksu odwróconego";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.CheckBox stemmingCheckBox;
        private System.Windows.Forms.CheckBox stopWordsCheckBox;
        private System.Windows.Forms.CheckBox lematyzationCheckBox;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem plikToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem otwórzPlikDoZindeksowaniaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem otwórzSłownikMorfologicznyToolStripMenuItem;
        private System.Windows.Forms.Label progressBarLabel;
        private System.Windows.Forms.OpenFileDialog toIndexOpenFileDialog;
        private System.Windows.Forms.TextBox delimiterTextBox;
        private System.Windows.Forms.Label delimiterLabel;
        private System.Windows.Forms.OpenFileDialog morphologicOpenFileDilalog;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStripMenuItem zapszPlikIndeksuToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveIndexFileDialog;
        private System.Windows.Forms.CheckBox compressCheckBox;


    }
}

