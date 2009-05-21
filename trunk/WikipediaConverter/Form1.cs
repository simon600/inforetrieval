using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WikipediaConverter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int b;
            FileStream in_file = new FileStream("D:\\Studia\\Wyszukiwanie Informacji\\Pracownia\\wikipediaFrance.txt", FileMode.Open);
            FileStream out_file = new FileStream("D:\\Studia\\Wyszukiwanie Informacji\\Pracownia\\wikipediaConv.txt", FileMode.Create);
            //StreamReader reader = new StreamReader(in_file, Encoding.UTF8, false);            
            //BinaryReader reader = new BinaryReader(in_file);
            //BinaryWriter writer = new BinaryWriter(out_file);
            
        
            //writer.BaseStream.SetLength(reader.BaseStream.Length);            
            int b1, b2, b3;            

            b1 = in_file.ReadByte();

            if (b1 == 239)
            {
                b2 = in_file.ReadByte();
                if (b2 == 187)
                {
                    b3 = in_file.ReadByte();
                    if (b3 == 191)
                    {
                    }
                    else
                    {
                        out_file.WriteByte((byte)b1);
                        out_file.WriteByte((byte)b2);
                        out_file.WriteByte((byte)b3);
                    }
                }
                else
                {
                    out_file.WriteByte((byte)b1);
                    out_file.WriteByte((byte)b2);
                    out_file.WriteByte((byte)in_file.ReadByte());
                }
            }
            else
            {
                if (b1 < 128)
                {
                    out_file.WriteByte((byte)b1);
                }
                else if (b1 >= 194 && b1 < 224)
                {
                    out_file.WriteByte((byte)b1);
                    out_file.WriteByte((byte)in_file.ReadByte());
                }
                else
                {
                    out_file.WriteByte((byte)b1);
                    out_file.WriteByte((byte)in_file.ReadByte());
                    out_file.WriteByte((byte)in_file.ReadByte());
                }
            }

            while (in_file.Position < in_file.Length)
            {
                b = in_file.ReadByte();
                if (b < 128)
                {
                    out_file.WriteByte((byte)b);
                }
                else if (b >= 194 && b < 224)
                {
                    out_file.WriteByte((byte)b);
                    out_file.WriteByte((byte)in_file.ReadByte());
                }
                else
                {
                    out_file.WriteByte((byte)b);
                    out_file.WriteByte((byte)in_file.ReadByte());
                    out_file.WriteByte((byte)in_file.ReadByte());
                }
            }
        }
    }
}
