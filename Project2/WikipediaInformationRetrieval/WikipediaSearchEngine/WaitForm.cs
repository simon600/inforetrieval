using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WikipediaSearchEngine
{
    public partial class WaitForm : Form
    {
        public WaitForm(string name)
        {
            InitializeComponent();

            this.Text = name;
            
        }
    }
}
