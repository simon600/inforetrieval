namespace WikipediaSearchEngine
{
    partial class MainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.zapytaniaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.otwórzPlikZZapytaniamiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wskażPlikWynikowyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.queryTextBox = new System.Windows.Forms.TextBox();
            this.searchButton = new System.Windows.Forms.Button();
            this.resultTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.responseTimeLabel = new System.Windows.Forms.Label();
            this.resultsCount = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zapytaniaToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(867, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // zapytaniaToolStripMenuItem
            // 
            this.zapytaniaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.otwórzPlikZZapytaniamiToolStripMenuItem,
            this.wskażPlikWynikowyToolStripMenuItem});
            this.zapytaniaToolStripMenuItem.Name = "zapytaniaToolStripMenuItem";
            this.zapytaniaToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.zapytaniaToolStripMenuItem.Text = "Plik";
            // 
            // otwórzPlikZZapytaniamiToolStripMenuItem
            // 
            this.otwórzPlikZZapytaniamiToolStripMenuItem.Name = "otwórzPlikZZapytaniamiToolStripMenuItem";
            this.otwórzPlikZZapytaniamiToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.otwórzPlikZZapytaniamiToolStripMenuItem.Text = "Wybierz plik z zapytaniami";
            this.otwórzPlikZZapytaniamiToolStripMenuItem.Click += new System.EventHandler(this.otwórzPlikZZapytaniamiToolStripMenuItem_Click);
            // 
            // wskażPlikWynikowyToolStripMenuItem
            // 
            this.wskażPlikWynikowyToolStripMenuItem.Name = "wskażPlikWynikowyToolStripMenuItem";
            this.wskażPlikWynikowyToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.wskażPlikWynikowyToolStripMenuItem.Text = "Zapisz wyniki zapytania do pliku";
            this.wskażPlikWynikowyToolStripMenuItem.Click += new System.EventHandler(this.wskażPlikWynikowyToolStripMenuItem_Click);
            // 
            // queryTextBox
            // 
            this.queryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.queryTextBox.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.queryTextBox.Location = new System.Drawing.Point(13, 46);
            this.queryTextBox.Name = "queryTextBox";
            this.queryTextBox.Size = new System.Drawing.Size(709, 25);
            this.queryTextBox.TabIndex = 1;
            // 
            // searchButton
            // 
            this.searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchButton.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.searchButton.Location = new System.Drawing.Point(724, 44);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(131, 27);
            this.searchButton.TabIndex = 2;
            this.searchButton.Text = "Szukaj";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // resultTextBox
            // 
            this.resultTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.resultTextBox.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.resultTextBox.Location = new System.Drawing.Point(12, 111);
            this.resultTextBox.Multiline = true;
            this.resultTextBox.Name = "resultTextBox";
            this.resultTextBox.ReadOnly = true;
            this.resultTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.resultTextBox.Size = new System.Drawing.Size(842, 534);
            this.resultTextBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(12, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 18);
            this.label1.TabIndex = 4;
            this.label1.Text = "Wyniki wyszukiwania:";
            // 
            // responseTimeLabel
            // 
            this.responseTimeLabel.AutoSize = true;
            this.responseTimeLabel.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.responseTimeLabel.Location = new System.Drawing.Point(584, 90);
            this.responseTimeLabel.Name = "responseTimeLabel";
            this.responseTimeLabel.Size = new System.Drawing.Size(0, 17);
            this.responseTimeLabel.TabIndex = 5;
            // 
            // resultsCount
            // 
            this.resultsCount.AutoSize = true;
            this.resultsCount.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.resultsCount.Location = new System.Drawing.Point(195, 90);
            this.resultsCount.Name = "resultsCount";
            this.resultsCount.Size = new System.Drawing.Size(0, 17);
            this.resultsCount.TabIndex = 6;
            // 
            // MainForm
            // 
            this.AcceptButton = this.searchButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(867, 657);
            this.Controls.Add(this.resultsCount);
            this.Controls.Add(this.responseTimeLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.resultTextBox);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.queryTextBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Szukacz";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem zapytaniaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem otwórzPlikZZapytaniamiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wskażPlikWynikowyToolStripMenuItem;
        private System.Windows.Forms.TextBox queryTextBox;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.TextBox resultTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label responseTimeLabel;
        private System.Windows.Forms.Label resultsCount;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}

