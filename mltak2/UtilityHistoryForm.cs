using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace mltak2
{
    public partial class UtilityHistoryForm : Form
    {
        Hashtable blocks;
        Hashtable UtilityProgress;
        public UtilityHistoryForm(Environment.Grid grid, Hashtable utilityProgress)
        {
            InitializeComponent();
            this.blocks = new Hashtable();
            this.UtilityProgress = utilityProgress;
            var margin = 10;
            Button btn = null;
            for (int i = 0; i < grid.Size.Width; i++)
            {
                for (int j = 0; j < grid.Size.Height; j++)
                {
                    btn = new Button();
                    btn.Size = new Size(40, 40);
                    btn.Location = new Point(i * btn.Size.Width + margin, j * btn.Size.Height + margin);
                    btn.Click += new EventHandler(btn_Click);
                    btn.Tag = null;
                    blocks.Add(btn, new Point(i, j));
                    this.Controls.Add(btn);
                }
            }
            var label = new Label();
            label.Text = "Select points to plot utility history...";
            label.Size = new Size(btn.Right, label.Height);
            label.Location = new Point(margin, btn.Bottom + margin);
            this.Controls.Add(label);
            var plotbtn = new Button();
            plotbtn.Text = "Plot";
            plotbtn.Location = new Point(margin, label.Bottom + margin);
            plotbtn.Size = new Size(btn.Right - margin, plotbtn.Height);
            plotbtn.Click += new EventHandler(plotbtn_Click);
            this.Controls.Add(plotbtn);
            this.Size = new Size(btn.Right + margin + 5, plotbtn.Bottom + margin + 30);
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler((sender, e) =>
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape: this.Close(); break;
                }
            });
        }

        void plotbtn_Click(object sender, EventArgs e)
        {
            var list = new List<List<float>>();
            var labels = new List<string>();
            foreach (var item in blocks.Keys)
            {
                if ((item as Button).Tag != null)
                {
                    var p = (Point)blocks[item];
                    labels.Add(p.ToString());
                    list.Add(this.UtilityProgress[p] as List<float>);
                }
            }
            var c = new ChartForm(list, labels);
            c.ShowDialog(this);
        }

        void btn_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn.Tag == null)
            {
                btn.Tag = true;
                btn.Text = "X";
            }
            else
            {
                btn.Tag = null;
                btn.Text = "";
            }
        }
    }
}