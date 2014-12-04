﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization.Formatters.Binary;
using Environment;
namespace mltak2
{
    public partial class GridForm : Form
    {
        protected Timer __NearBySearchTimer { get; set; }
        Grid g { get; set; }
        Point __last_valid_grid_block;
        public GridForm()
        {
            InitializeComponent();
            /**
             * Draw the grid
             */
            Size gridSize = Properties.Settings.Default.GridSize;
            Size s = Grid.GetSizeOfGrid(gridSize);
            this.grid.Image = new Bitmap(s.Width + 1, s.Height + 1, this.grid.CreateGraphics());
            using (Graphics gfx = Graphics.FromImage(this.grid.Image))
            {
                g = new Grid(gridSize, gfx);
                g.Draw();
                this.Size = new Size(s.Width + 21, s.Height + 90);
            }
            /**
             * 
             */
            this.Load += new EventHandler((sender, e) =>
            {
                Timer t = new Timer();
                t.Interval = 100;
                t.Tick += new EventHandler((_sender, _e) =>
                {
                    t.Stop();
                    g.BlockBorders(this.grid);
                });
                t.Start();
            });
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler((sender, e) =>
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape: Application.Exit(); break;
                }
            });
            this.grid.MouseMove += new MouseEventHandler((sender, e) =>
            {
                if (this.__NearBySearchTimer != null) this.__NearBySearchTimer.Stop();
                else
                {
                    this.__NearBySearchTimer = new Timer();
                    this.__NearBySearchTimer.Interval = 15;
                    this.__NearBySearchTimer.Tick += new EventHandler((__sender, __e) =>
                    {
                        var p = (Point)this.__NearBySearchTimer.Tag;
                        if (g.GetNearByLineInfo(this.grid, p).Value.A != 0)
                            Cursor.Current = Cursors.Hand;
                        else Cursor.Current = Cursors.Default;
                    });
                }
                if (g.GetNearByLineInfo(this.grid, e.Location).Value.A != 0)
                    Cursor.Current = Cursors.Hand;
                this.__NearBySearchTimer.Tag = e.Location;
                this.__NearBySearchTimer.Start();
            });
            this.grid.MouseDown += new MouseEventHandler(grid_MouseDown);
            this.toolStripStatus.TextChanged += new EventHandler((sende, e) =>
            {
                bool internal_change = false;
                if (internal_change)
                {
                    internal_change = false;
                    return;
                }
                Timer t = new Timer();
                t.Interval = 4000;
                t.Tick += new EventHandler((_s, _e) =>
                {
                    t.Stop();
                    internal_change = true;
                    this.toolStripStatus.Text = "";
                });
                t.Start();
            });
            if(System.IO.File.Exists("config.dat"))
                try
                {
                    loadConfigurationToolStripMenuItem_Click(new object(), new EventArgs());
                }
                catch
                {
                    MessageBox.Show("Unable to load the saved configurations");
                    newConfigurationToolStripMenuItem_Click(new object(), new EventArgs());
                }
            else
                newConfigurationToolStripMenuItem_Click(new object(), new EventArgs());
        }
        public void grid_MouseDown(object sender, MouseEventArgs e)
        {
            var inf = g.GetNearByLineInfo(this.grid, e.Location);
            if (inf.Value.A != 0)
            {
                if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
                g.ToggleBlock(this.grid, inf.Key);
                this.Invalidate();
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                __last_valid_grid_block = e.Location;
                contextMenuStrip.Show(this, new Point(e.Location.X, e.Location.Y + 25));
                return;
            }
        }

        private void saveConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists("config.dat") && MessageBox.Show("Are you sure you want to overwrite the previously save configuration data?", "Attention!!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
            {
                this.toolStripStatus.Text = "Operation Abort...";
                return;
            }
            BinaryFormatter bf = new BinaryFormatter();
            {
                using (System.IO.FileStream fs = new System.IO.FileStream("config.dat", System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
                {
                    bf.Serialize(fs, g.GetStatus());
                    bf.Serialize(fs, g.AgentPoint);
                    bf.Serialize(fs, g.GoalPoint);
                }
            }
            this.toolStripStatus.Text = "Configuration Saved Sucessfully...";
        }

        private void loadConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!System.IO.File.Exists("config.dat"))
            {
                MessageBox.Show("No configuration file found!");
                this.toolStripStatus.Text = "Operation Abort...";
                return;
            }
            BinaryFormatter bf = new BinaryFormatter();
            {
                using (System.IO.FileStream fs = new System.IO.FileStream("config.dat", System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None))
                {
                    this.grid.CreateGraphics().Clear(Color.FromKnownColor(KnownColor.Control));
                    g = new Grid((Grid.BlockStatus[,])bf.Deserialize(fs), this.grid);
                    g.AgentPoint = (Point)bf.Deserialize(fs);
                    g.GoalPoint = (Point)bf.Deserialize(fs);
                    Timer t = new Timer();
                    t.Interval = 100;
                    t.Tick += new EventHandler((_sender, _e) =>
                    {
                        t.Stop();
                        g.BlockBorders(this.grid);
                        g.Draw();
                        __last_valid_grid_block = g.abs2grid(g.AgentPoint);
                        MarkStartPointGrid_Click(new object(), new EventArgs());
                        __last_valid_grid_block = g.abs2grid(g.GoalPoint);
                        MarkGoalPointGrid_Click(new object(), new EventArgs());
                    });
                    t.Start();
                }
            }
            this.toolStripStatus.Text = "Configuration Loaded Sucessfully...";
            //trainToolStripMenuItem_Click(sender, e);
        }

        private void newConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.grid.CreateGraphics().Clear(Color.FromKnownColor(KnownColor.Control));
            var s = Grid.GetSizeOfGrid(Properties.Settings.Default.GridSize);
            g = new Grid(Properties.Settings.Default.GridSize, this.grid.CreateGraphics());
            this.Size = new Size(s.Width + 21, s.Height + 90);
            Timer t = new Timer();
            t.Interval = 100;
            t.Tick += new EventHandler((_sender, _e) =>
            {
                t.Stop();
                g.BlockBorders(this.grid);
                g.Draw();
                __last_valid_grid_block = g.abs2grid(g.AgentPoint);
                MarkStartPointGrid_Click(new object(), new EventArgs());
                __last_valid_grid_block = g.abs2grid(g.GoalPoint);
                MarkGoalPointGrid_Click(new object(), new EventArgs());
            });
            t.Start();
        }

        private void MarkStartPointGrid_Click(object sender, EventArgs e)
        {
            using (var gfx = this.grid.CreateGraphics())
            {
                var p = g.abs2grid(g.AgentPoint);
                // clear previous flag
                gfx.FillEllipse(new SolidBrush(Color.FromKnownColor(KnownColor.Control)), p.X - 15, p.Y - 12, 25, 25);
                g.Write(" ", new Point(p.X - 10, p.Y - 10), gfx, Brushes.White);

                // write a new one
                p = g.abs2grid(g.grid2abs(__last_valid_grid_block));
                gfx.FillEllipse(Brushes.Red, p.X - 15, p.Y - 12, 25, 25);
                g.Write("A", new Point(p.X - 10, p.Y - 10), gfx, Brushes.White);

                // set the new point
                g.AgentPoint = g.grid2abs(p);
            }
        }

        private void MarkGoalPointGrid_Click(object sender, EventArgs e)
        {
            using (var gfx = this.grid.CreateGraphics())
            {
                var p = g.abs2grid(g.GoalPoint);
                // clear previous flag
                gfx.FillEllipse(new SolidBrush(Color.FromKnownColor(KnownColor.Control)), p.X - 15, p.Y - 15, 30, 30);
                g.Write(" ", new Point(p.X - 10, p.Y - 10), gfx, Brushes.White);

                // write a new one
                p = g.abs2grid(g.grid2abs(__last_valid_grid_block));
                gfx.FillEllipse(Brushes.Green, p.X - 15, p.Y - 15, 30, 30);
                g.Write("G", new Point(p.X - 10, p.Y - 10), gfx, Brushes.White);

                // set the new point
                g.GoalPoint = g.grid2abs(p);
            }
        }

        private void configurationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Configurations().Show() ;
        }

        private void trainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ReinforcementLearning.QLearning(
                this.g,
                new List<GridHelper.Directions>(Enum.GetValues(typeof(GridHelper.Directions)).Cast<GridHelper.Directions>()),
                0.9F,
                0.2F).Train(new Func<Grid, bool>((g) =>
                {
                    return false;
                }));
        }
    }
}