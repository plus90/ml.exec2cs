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
                    this.grid.CreateGraphics().Clear(Color.FromKnownColor(KnownColor.Control));
                    g = new Grid((Grid.BlockStatus[,])bf.Deserialize(fs), this.grid);
                    g.AgentPoint = (Point)bf.Deserialize(fs);
                    g.GoalPoint = (Point)bf.Deserialize(fs);
                    Timer t = new Timer();
                    t.Interval = 100;
                    t.Tick += new EventHandler((_sender, _e) =>
                    {
                        t.Stop();
                        try
                        {
                            g.BlockBorders(this.grid);
                            g.Draw(this.grid.CreateGraphics());
                        }
                        catch
                        {
                            MessageBox.Show("Unable to load the saved configurations");
                            newConfigurationToolStripMenuItem_Click(new object(), new EventArgs());
                        }
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
        Hashtable optimalPath = new Hashtable();
        private void trainQLearningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                var max_q_table_size = Enum.GetValues(typeof(GridHelper.Directions)).Length * this.g.Size.Height * this.g.Size.Width;
                const int max_iter = 1;
                long totall_step_counter = 0;
                ReinforcementLearning.QLearning ql = null;
                for (int i = 0; i < max_iter; i++)
                {
                    ql = new ReinforcementLearning.QLearning(
                         this.g,
                         new List<GridHelper.Directions>(Enum.GetValues(typeof(GridHelper.Directions)).Cast<GridHelper.Directions>()),
                         0.9F,
                         0.2F, ql == null ? null : ql.QSA);
                    ql.Train(
                        new Func<Grid, long, bool>((g, step_counter) =>
                        {
                            if (step_counter % 40 == 0)
                                this.toolStripStatus.Text = String.Format("{0}% Of learning process passed....", (step_counter + totall_step_counter + i + 1) * 100 / (max_q_table_size * 40 * max_iter));
                            return 40 * max_q_table_size <= step_counter;
                        }));
                    totall_step_counter += ql.StepCounter;
                }
                this.toolStripStatus.Text = "The model has learned...";
                output o = new output();
                StringBuilder sb = new StringBuilder();
                Hashtable hs = new Hashtable();
                optimalPath.Clear();
                foreach (KeyValuePair<Point, GridHelper.Directions> s in ql.QSA.Keys)
                {
                    if (hs.Contains(s.Key))
                    {
                        var a = hs[s.Key] as List<KeyValuePair<GridHelper.Directions, float>>;
                        a.Add(new KeyValuePair<GridHelper.Directions, float>(s.Value, (float)ql.QSA[s]));
                    }
                    else
                    {
                        hs.Add(s.Key, new List<KeyValuePair<GridHelper.Directions, float>>() { new KeyValuePair<GridHelper.Directions, float>(s.Value, (float)ql.QSA[s]) });
                    }
                    if (optimalPath.Contains(s.Key))
                    {
                        if ((float)ql.QSA[s] > ((KeyValuePair<float, GridHelper.Directions>)optimalPath[s.Key]).Key)
                            optimalPath[s.Key] = new KeyValuePair<float, GridHelper.Directions>((float)ql.QSA[s], s.Value);
                    }
                    else optimalPath.Add(s.Key, new KeyValuePair<float, GridHelper.Directions>((float)ql.QSA[s], s.Value));
                }
                var margin = 23;
                using (var gfx = this.grid.CreateGraphics())
                {
                    foreach (Point cell in hs.Keys)
                    {
                        foreach (KeyValuePair<GridHelper.Directions, float> dir in hs[cell] as List<KeyValuePair<GridHelper.Directions, float>>)
                        {

                            var p = g.abs2grid(cell);
                            switch (dir.Key)
                            {
                                case GridHelper.Directions.NORTH:
                                    gfx.FillPolygon(Brushes.LightBlue, new Point[] { new Point(p.X - margin, p.Y - margin), new Point(p.X + margin, p.Y - margin), new Point(p.X, p.Y - 2 * margin) });
                                    this.g.Write(dir.Value.ToString("F1"), new Point(p.X - margin + 7, p.Y - 2 * margin + 7), gfx, Brushes.DarkBlue, new Font("Arial", 10, FontStyle.Bold));
                                    break;
                                case GridHelper.Directions.EAST:
                                    gfx.FillPolygon(Brushes.LightBlue, new Point[] { new Point(p.X + margin, p.Y - margin), new Point(p.X + margin, p.Y + margin), new Point(p.X + 2 * margin, p.Y) });
                                    this.g.Write(dir.Value.ToString("F1"), new Point(p.X + margin - 4, p.Y - margin / 2), gfx, Brushes.DarkBlue, new Font("Arial", 10, FontStyle.Bold));
                                    break;
                                case GridHelper.Directions.SOUTH:
                                    gfx.FillPolygon(Brushes.LightBlue, new Point[] { new Point(p.X - margin, p.Y + margin), new Point(p.X + margin, p.Y + margin), new Point(p.X, p.Y + 2 * margin) });
                                    this.g.Write(dir.Value.ToString("F1"), new Point(p.X - margin + 10, p.Y + margin), gfx, Brushes.DarkBlue, new Font("Arial", 10, FontStyle.Bold));
                                    break;
                                case GridHelper.Directions.WEST:
                                    gfx.FillPolygon(Brushes.LightBlue, new Point[] { new Point(p.X - margin, p.Y - margin), new Point(p.X - margin, p.Y + margin), new Point(p.X - 2 * margin, p.Y) });
                                    this.g.Write(dir.Value.ToString("F1"), new Point(p.X - 2 * margin, p.Y - margin / 2), gfx, Brushes.DarkBlue, new Font("Arial", 10, FontStyle.Bold));
                                    break;
                                case GridHelper.Directions.HOLD:
                                    this.g.Write(dir.Value.ToString("F1"), new Point(p.X - 2 * margin, p.Y - 2 * margin), gfx, Brushes.Brown, new Font("Arial", 10, FontStyle.Bold));
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    hs.Clear();
                    hs = new Hashtable();
                    foreach (KeyValuePair<Point, GridHelper.Directions> cell in ql.VisitedSA.Keys)
                    {
                        long count = (long)ql.VisitedSA[cell];
                        if (hs.Contains(cell.Key))
                            hs[cell.Key] = (long)hs[cell.Key] + count;
                        else
                            hs.Add(cell.Key, count);
                    }
                    foreach (Point cell in hs.Keys)
                    {
                        var p = g.abs2grid(cell);
                        this.g.Write("#" + hs[cell].ToString(), new Point(p.X + 2 * margin / 3, p.Y - 2 * margin), gfx, Brushes.Brown, new Font("Arial", 10, FontStyle.Bold));
                    }
                }
                this.exameToolStripMenuItem.GetCurrentParent().Invoke(new Action(() =>
                {
                    this.exameToolStripMenuItem.Enabled = true;
                }));
            }));
            t.Start();
            this.toolStripStatus.Text = "Start learning...";
        }

        private void exameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                if (this.optimalPath == null || this.optimalPath.Count == 0)
                {
                    MessageBox.Show("There no stored optimal values...");
                }
                var gh = new GridHelper(this.g);
                while (this.g.AgentPoint != this.g.GoalPoint)
                {
                    var s = (KeyValuePair<float, GridHelper.Directions>)this.optimalPath[this.g.AgentPoint];
                    var m = gh.Move(this.g.AgentPoint, s.Value);
                    __last_valid_grid_block = g.abs2grid(m.NewPoint);
                    MarkStartPointGrid_Click(sender, e);
                    this.g.AgentPoint = m.NewPoint;
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(500);
                }
                MessageBox.Show("Has reached the goal...", "Horray!!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
            }));
            t.Start();
        }
    }
}