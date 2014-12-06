using System;
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
using System.Collections;
namespace mltak2
{
    public partial class GridForm : Form
    {
        Point __last_valid_grid_block;
        protected Timer __NearBySearchTimer { get; set; }
        List<System.Threading.Thread> ThreadsPool = new List<System.Threading.Thread>();
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
                g = new Grid(gridSize);
                g.Draw(gfx);
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
                    case Keys.Escape: System.Diagnostics.Process.GetCurrentProcess().Kill(); break;
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
                if (this.toolStripStatus.Tag != null)
                    (this.toolStripStatus.Tag as Timer).Stop();
                Timer t = new Timer();
                t.Interval = 4000;
                t.Tick += new EventHandler((_s, _e) =>
                {
                    t.Stop();
                    internal_change = true;
                    this.toolStripStatus.Tag = null;
                    this.toolStripStatus.Text = "";
                });
                t.Start();
                this.toolStripStatus.Tag = t;
            });
            if (System.IO.File.Exists("config.dat"))
                loadConfigurationToolStripMenuItem_Click(new object(), new EventArgs());

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
                    g = new Grid((Grid.BlockStatus[,])bf.Deserialize(fs), this.grid);
                    g.AgentPoint = (Point)bf.Deserialize(fs);
                    g.GoalPoint = (Point)bf.Deserialize(fs);
                }
                __reload_grid();
            }
            this.toolStripStatus.Text = "Configuration Loaded Sucessfully...";
        }
        private void newConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.grid.CreateGraphics().Clear(Color.FromKnownColor(KnownColor.Control));
            var s = Grid.GetSizeOfGrid(Properties.Settings.Default.GridSize);
            g = new Grid(Properties.Settings.Default.GridSize);
            this.Size = new Size(s.Width + 21, s.Height + 90);
            Timer t = new Timer();
            t.Interval = 100;
            t.Tick += new EventHandler((_sender, _e) =>
            {
                t.Stop();
                g.BlockBorders(this.grid);
                g.Draw(this.grid.CreateGraphics());
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
        private void MarkStartGoalPoints(Point ABS_Start, Point ABS_Goal)
        {
            __last_valid_grid_block = g.abs2grid(ABS_Goal);
            MarkGoalPointGrid_Click(new object(), new EventArgs());
            __last_valid_grid_block = g.abs2grid(ABS_Start);
            MarkStartPointGrid_Click(new object(), new EventArgs());
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
            new Configurations().Show();
        }
        private void DrawDirection(Point point, GridHelper.Directions direction)
        {
            using (var gfx = this.grid.CreateGraphics())
            {
                var p = g.abs2grid(point);
                var pen = new Pen(Color.Black, 4);
                var margin = 10;
                switch (direction)
                {
                    case GridHelper.Directions.NORTH:
                        gfx.DrawLine(pen, new Point(p.X, p.Y - margin), new Point(p.X, p.Y + margin));
                        gfx.FillPolygon(new SolidBrush(pen.Color), new Point[] { new Point(p.X - margin + 5, p.Y - margin + 5), new Point(p.X + margin - 5, p.Y - margin + 5), new Point(p.X, p.Y - 2 * margin) });
                        break;
                    case GridHelper.Directions.EAST:
                        gfx.DrawLine(pen, new Point(p.X - margin - 5, p.Y), new Point(p.X + margin - 5, p.Y));
                        gfx.FillPolygon(new SolidBrush(pen.Color), new Point[] { new Point(p.X + margin - 5, p.Y - margin + 5), new Point(p.X + margin - 5, p.Y + margin - 5), new Point(p.X + 2 * (margin), p.Y) });
                        break;
                    case GridHelper.Directions.SOUTH:
                        gfx.DrawLine(pen, new Point(p.X, p.Y - margin - 5), new Point(p.X, p.Y + margin));
                        gfx.FillPolygon(new SolidBrush(Color.Black), new Point[] { new Point(p.X - margin + 5, p.Y + margin - 5), new Point(p.X + margin - 5, p.Y + margin - 5), new Point(p.X, p.Y + 2 * margin) });
                        break;
                    case GridHelper.Directions.WEST:
                        gfx.DrawLine(pen, new Point(p.X - margin + 5, p.Y), new Point(p.X + margin + 5, p.Y));
                        gfx.FillPolygon(new SolidBrush(pen.Color), new Point[] { new Point(p.X - margin + 5, p.Y - margin + 5), new Point(p.X - margin + 5, p.Y + margin - 5), new Point(p.X - 2 * margin, p.Y) });
                        break;
                    case GridHelper.Directions.HOLD:
                        gfx.FillEllipse(new SolidBrush(Color.Black), p.X - 5, p.Y - 5, 10, 10);
                        break;
                }
            }
        }
        private void learnGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.learnToolStripMenuItem.Enabled) return;
            __enable_all_menus(false);
            this.__kill_threads();
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart((thread) =>
            {
                __learn_policy(sender);
            __END_LEARNING:
                this.examToolStripMenuItem.GetCurrentParent().Invoke(new Action(() =>
                {
                    __enable_all_menus(true);
                }));
                lock (this.ThreadsPool)
                    this.ThreadsPool.Remove(thread as System.Threading.Thread);
            }));
            t.Start(t);
            lock (this.ThreadsPool)
                this.ThreadsPool.Add(t);
            this.toolStripStatus.Text = "Start learning...";
        }
        private void examToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.examToolStripMenuItem.Enabled) return;
            __enable_all_menus(false);
            this.__kill_threads();
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart((thread) =>
            {
                if (this.optimalPath == null || this.optimalPath.Count == 0)
                {
                    this.examToolStripMenuItem.GetCurrentParent().Invoke(new Action(() =>
                    {
                        __enable_all_menus(true);
                        this.examToolStripMenuItem.Enabled = false;
                    }));
                    return;
                }
                var gh = new GridHelper(this.g);
                var rand = new Random();
                while (this.g.AgentPoint != this.g.GoalPoint)
                {
                    var l = (List<KeyValuePair<float, GridHelper.Directions>>)this.optimalPath[this.g.AgentPoint];
                    var s = l[rand.Next(0, l.Count - 1)];
                    var m = gh.Move(this.g.AgentPoint, s.Value);
                    MarkStartGoalPoints(m.NewPoint, g.GoalPoint);
                    this.DrawDirection(m.OldPoint, s.Value);
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(500);
                }
                MessageBox.Show("Has reached the goal...", "Horray!!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                this.examToolStripMenuItem.GetCurrentParent().Invoke(new Action(() =>
                {
                    __enable_all_menus(true);
                }));
                lock (this.ThreadsPool)
                    this.ThreadsPool.Remove(thread as System.Threading.Thread);
            }));
            t.Start(t);
            lock (this.ThreadsPool)
                this.ThreadsPool.Add(t);
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ql == null || ql.QTable.Count == 0)
            {
                this.saveToolStripMenuItem.GetCurrentParent().Invoke(new Action(() =>
                {
                    __enable_all_menus(true);
                    this.saveToolStripMenuItem.Enabled = false;
                }));
                return;
            }
            __enable_all_menus(false);
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.DefaultExt = "dat";
                sfd.AddExtension = true;
                sfd.OverwritePrompt = true;
                sfd.Filter = "Data files (*.dat)|*.dat";
                var res = sfd.ShowDialog(this);
                if (res == System.Windows.Forms.DialogResult.OK)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    {
                        using (var fs = sfd.OpenFile())
                        {
                            bf.Serialize(fs, g.GetStatus());
                            bf.Serialize(fs, ql.QTable);
                            bf.Serialize(fs, ql.VisitedStateActions);
                            bf.Serialize(fs, ql.StepCounter);
                            bf.Serialize(fs, ReinforcementLearning.TDLambda.VTable);
                        }
                    }
                    this.toolStripStatus.Text = "The QTable saved successfully....";
                }
            }
            __enable_all_menus(true);
        }
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            __enable_all_menus(false);
            using (OpenFileDialog sfd = new OpenFileDialog())
            {
                sfd.DefaultExt = "dat";
                sfd.AddExtension = true;
                sfd.Filter = "Data files (*.dat)|*.dat";
                var res = sfd.ShowDialog(this);
                if (res == System.Windows.Forms.DialogResult.OK)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    {
                        using (var fs = sfd.OpenFile())
                        {
                            g = new Grid((Grid.BlockStatus[,])bf.Deserialize(fs), this.grid);
                            if (ql == null)
                                ql = new ReinforcementLearning.QLearning(
                                    this.g,
                                    new List<GridHelper.Directions>(Enum.GetValues(typeof(GridHelper.Directions)).Cast<GridHelper.Directions>()),
                                    Properties.Settings.Default.Gamma,
                                    Properties.Settings.Default.Alpha);
                            if (tdl == null)
                                tdl = new ReinforcementLearning.TDLambda(
                                    ql,
                                    Properties.Settings.Default.Lambda);
                            ql.QTable = (Hashtable)bf.Deserialize(fs);
                            ql.VisitedStateActions = (Hashtable)bf.Deserialize(fs);
                            ql.StepCounter = (long)bf.Deserialize(fs);
                            // support for non-VTable contain files
                            if(fs.Position <  fs.Length)
                                ReinforcementLearning.TDLambda.VTable = (Hashtable)bf.Deserialize(fs);
                        }
                    }
                    __reload_grid();
                    __plot_policy(ql, tdl);
                    this.toolStripStatus.Text = "The QTable saved successfully....";
                }
            }
            __enable_all_menus(true);
        }
    }
}