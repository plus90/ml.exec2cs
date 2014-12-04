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
    public partial class Configurations : Form
    {
        public Configurations()
        {
            InitializeComponent();
            this.gridWidth.SelectedIndex = Properties.Settings.Default.GridSize.Width - 2;
            this.gamma.Text = Properties.Settings.Default.Gamma.ToString();
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler((sender, e) =>
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape: this.cancel_Click(new object(), new EventArgs()); break;
                }
            });
        }

        public Size GetGridSize() { return Properties.Settings.Default.GridSize;}

        public float GetGammaValue() { return Properties.Settings.Default.Gamma; }

        public void SetGridSize(Size size)
        {
            if (size.Height < 2 || size.Height > 10 || size.Width < 2 || size.Width > 10)
                throw new ArgumentOutOfRangeException("The size is not in ranges of { [4..10], [4..10] }");
            Properties.Settings.Default.GridSize = size;
            Properties.Settings.Default.Save();
        }

        public void SetGammaValue(float gamma)
        {
            if (gamma < 0 || gamma > 1)
                throw new ArgumentOutOfRangeException("The gamma should be in range of [0..1]");
            Properties.Settings.Default.Gamma = gamma;
            Properties.Settings.Default.Save();
        }

        private void save_Click(object sender, EventArgs e)
        {
            try
            {
                var s = new Size(int.Parse(this.gridWidth.SelectedItem.ToString()), int.Parse(this.gridWidth.SelectedItem.ToString()));
                this.SetGammaValue(float.Parse(this.gamma.Text));
                if (s != Properties.Settings.Default.GridSize)
                {
                    this.SetGridSize(s);
                    System.IO.File.Delete("config.dat");
                    if (MessageBox.Show("Application is going to restart in order to apply the new configurations!", "Notice...", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.OK)
                        Application.Restart();
                }
            }
            catch (ArgumentOutOfRangeException aore)
            {
                if (MessageBox.Show(aore.Message, "Not saved!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Cancel)
                    this.cancel_Click(sender, e);
            }
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
