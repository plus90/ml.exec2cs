using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace mltak2
{
    public partial class output : Form
    {
        public output(string s = "")
        {
            InitializeComponent();
            this.richTextBox1.Clear();
            this.richTextBox1.Text = s;
        }
        public void appendText(string s = "")
        {
            this.richTextBox1.AppendText(s);
        }
        public void clearText() { this.richTextBox1.Clear(); }
        public String getText { get { return this.richTextBox1.Text; } }
    }
}
