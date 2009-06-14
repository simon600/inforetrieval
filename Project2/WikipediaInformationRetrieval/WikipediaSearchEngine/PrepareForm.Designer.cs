namespace WikipediaSearchEngine
{
    partial class PrepareForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.sourceTextBox = new System.Windows.Forms.TextBox();
            this.morphologicTextBox = new System.Windows.Forms.TextBox();
            this.indexTextBox = new System.Windows.Forms.TextBox();
            this.sourceButton = new System.Windows.Forms.Button();
            this.morphButton = new System.Windows.Forms.Button();
            this.indexButton = new System.Windows.Forms.Button();
            this.sourceOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.morphOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.indexOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Plik źródłowy:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Słownik morfologiczny:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Plik z indeksem:";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(586, 120);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(91, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "Ok";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // sourceTextBox
            // 
            this.sourceTextBox.Location = new System.Drawing.Point(132, 16);
            this.sourceTextBox.Name = "sourceTextBox";
            this.sourceTextBox.Size = new System.Drawing.Size(447, 20);
            this.sourceTextBox.TabIndex = 4;
            // 
            // morphologicTextBox
            // 
            this.morphologicTextBox.Location = new System.Drawing.Point(132, 45);
            this.morphologicTextBox.Name = "morphologicTextBox";
            this.morphologicTextBox.Size = new System.Drawing.Size(447, 20);
            this.morphologicTextBox.TabIndex = 5;
            // 
            // indexTextBox
            // 
            this.indexTextBox.Location = new System.Drawing.Point(132, 73);
            this.indexTextBox.Name = "indexTextBox";
            this.indexTextBox.Size = new System.Drawing.Size(447, 20);
            this.indexTextBox.TabIndex = 6;
            // 
            // sourceButton
            // 
            this.sourceButton.Location = new System.Drawing.Point(585, 14);
            this.sourceButton.Name = "sourceButton";
            this.sourceButton.Size = new System.Drawing.Size(92, 23);
            this.sourceButton.TabIndex = 7;
            this.sourceButton.Text = "Wybierz";
            this.sourceButton.UseVisualStyleBackColor = true;
            this.sourceButton.Click += new System.EventHandler(this.sourceButton_Click);
            // 
            // morphButton
            // 
            this.morphButton.Location = new System.Drawing.Point(586, 43);
            this.morphButton.Name = "morphButton";
            this.morphButton.Size = new System.Drawing.Size(91, 23);
            this.morphButton.TabIndex = 8;
            this.morphButton.Text = "Wybierz";
            this.morphButton.UseVisualStyleBackColor = true;
            this.morphButton.Click += new System.EventHandler(this.morphButton_Click);
            // 
            // indexButton
            // 
            this.indexButton.Location = new System.Drawing.Point(586, 71);
            this.indexButton.Name = "indexButton";
            this.indexButton.Size = new System.Drawing.Size(91, 23);
            this.indexButton.TabIndex = 9;
            this.indexButton.Text = "Wybierz";
            this.indexButton.UseVisualStyleBackColor = true;
            this.indexButton.Click += new System.EventHandler(this.indexButton_Click);
            // 
            // sourceOpenFileDialog
            // 
            this.sourceOpenFileDialog.FileName = "\"\"";
            // 
            // morphOpenFileDialog
            // 
            this.morphOpenFileDialog.FileName = "\"\"";
            // 
            // indexOpenFileDialog
            // 
            this.indexOpenFileDialog.FileName = "\"\"";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // PrepareForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(689, 155);
            this.Controls.Add(this.indexButton);
            this.Controls.Add(this.morphButton);
            this.Controls.Add(this.sourceButton);
            this.Controls.Add(this.indexTextBox);
            this.Controls.Add(this.morphologicTextBox);
            this.Controls.Add(this.sourceTextBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "PrepareForm";
            this.Text = "Przygotuj wyszukiwarkę";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox sourceTextBox;
        private System.Windows.Forms.TextBox morphologicTextBox;
        private System.Windows.Forms.TextBox indexTextBox;
        private System.Windows.Forms.Button sourceButton;
        private System.Windows.Forms.Button morphButton;
        private System.Windows.Forms.Button indexButton;
        private System.Windows.Forms.OpenFileDialog sourceOpenFileDialog;
        private System.Windows.Forms.OpenFileDialog morphOpenFileDialog;
        private System.Windows.Forms.OpenFileDialog indexOpenFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}