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
        Hashtable optimalPath = new Hashtable();
        List<System.Threading.Thread> ThreadsPool = new List<System.Threading.Thread>();
        ReinforcementLearning.TDLearning ql = null;
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

        private void __reload_grid()
        {
            this.grid.CreateGraphics().Clear(Color.FromKnownColor(KnownColor.Control));
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
        private void __kill_threads()
        {
            lock (this.ThreadsPool)
                foreach (var t in this.ThreadsPool) if (t.IsAlive) t.Abort();
        }

        private void __plotPolicy(ReinforcementLearning.TDLearning ql)
        {
            /**
             * Draw the result POLICY!!!
             */
            StringBuilder sb = new StringBuilder();
            Hashtable hs = new Hashtable();
            optimalPath.Clear();
            /**
             * Normalize the POLICY
             */
            foreach (KeyValuePair<Point, GridHelper.Directions> s in ql.QTable.Keys)
            {
                if (hs.Contains(s.Key))
                {
                    var a = hs[s.Key] as List<KeyValuePair<GridHelper.Directions, float>>;
                    a.Add(new KeyValuePair<GridHelper.Directions, float>(s.Value, (float)ql.QTable[s]));
                }
                else
                {
                    hs.Add(s.Key, new List<KeyValuePair<GridHelper.Directions, float>>() { new KeyValuePair<GridHelper.Directions, float>(s.Value, (float)ql.QTable[s]) });
                }
                if (optimalPath.Contains(s.Key))
                {
                    if ((float)ql.QTable[s] > ((List<KeyValuePair<float, GridHelper.Directions>>)optimalPath[s.Key])[0].Key)
                        optimalPath[s.Key] = new List<KeyValuePair<float, GridHelper.Directions>>() { new KeyValuePair<float, GridHelper.Directions>((float)ql.QTable[s], s.Value) };
                    else if ((float)ql.QTable[s] == ((List<KeyValuePair<float, GridHelper.Directions>>)optimalPath[s.Key])[0].Key)
                        ((List<KeyValuePair<float, GridHelper.Directions>>)optimalPath[s.Key]).Add(new KeyValuePair<float, GridHelper.Directions>((float)ql.QTable[s], s.Value));
                }
                else optimalPath.Add(s.Key, new List<KeyValuePair<float, GridHelper.Directions>>() { new KeyValuePair<float, GridHelper.Directions>((float)ql.QTable[s], s.Value) });
            }
            var margin = 23;
            /**
             * Draw the triangles and POLICY values upon them
             */
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
                                this.g.Write(dir.Value.ToString("F1"), new Point(p.X - 2 * margin, p.Y - 2 * margin), gfx, Brushes.DarkBlue, new Font("Arial", 10, FontStyle.Bold));
                                break;
                            default:
                                break;
                        }
                    }
                }
                hs.Clear();
                hs = new Hashtable();
                /**
                 * Normalize the visited states count
                 */
                foreach (KeyValuePair<Point, GridHelper.Directions> cell in ql.VisitedStates.Keys)
                {
                    long count = (long)ql.VisitedStates[cell];
                    if (hs.Contains(cell.Key))
                        hs[cell.Key] = (long)hs[cell.Key] + count;
                    else
                        hs.Add(cell.Key, count);
                }
                /**
                 * Plot the visited states
                 */
                foreach (Point cell in hs.Keys)
                {
                    var p = g.abs2grid(cell);
                    this.g.Write("#" + hs[cell].ToString(), new Point(p.X + 2 * margin / 3, p.Y - 2 * margin), gfx, Brushes.Brown, new Font("Arial", 10, FontStyle.Bold));
                }
            }
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
                int max_iter = Properties.Settings.Default.MaxLearningIteration;
                long totall_step_counter = 0;
                for (int i = 0; i < max_iter; i++)
                {
                    // if the Q-Learning has been invoked?
                    if (sender == this.qLearningToolStripMenuItem)
                        // init the Q-learning instance
                        ql = new ReinforcementLearning.QLearning(
                             this.g,
                             new List<GridHelper.Directions>(Enum.GetValues(typeof(GridHelper.Directions)).Cast<GridHelper.Directions>()),
                             Properties.Settings.Default.Gamma,
                             Properties.Settings.Default.Alpha,
                             ql == null ? null : ql.QTable);
                    // if the SARSA-Learning has been invoked?
                    else if (sender == this.SARSAToolStripMenuItem)
                        // init the SARSA-learning instance
                        ql = new ReinforcementLearning.SarsaLearning(
                             this.g,
                             new List<GridHelper.Directions>(Enum.GetValues(typeof(GridHelper.Directions)).Cast<GridHelper.Directions>()),
                             Properties.Settings.Default.Gamma,
                             Properties.Settings.Default.Alpha,
                             ql == null ? null : ql.QTable);
                    else if (sender == this.sARSALambdaToolStripMenuItem)
                        ql = new ReinforcementLearning.SarsaLambdaLearning(
                             this.g,
                             new List<GridHelper.Directions>(Enum.GetValues(typeof(GridHelper.Directions)).Cast<GridHelper.Directions>()),
                             Properties.Settings.Default.Gamma,
                             Properties.Settings.Default.Alpha,
                             Properties.Settings.Default.Lambda,
                             ql == null ? null : ql.QTable);
                    else if(sender == this.qLambdaToolStripMenuItem)
                        ql = new ReinforcementLearning.QLambdaLearning(
                             this.g,
                             new List<GridHelper.Directions>(Enum.GetValues(typeof(GridHelper.Directions)).Cast<GridHelper.Directions>()),
                             Properties.Settings.Default.Gamma,
                             Properties.Settings.Default.Alpha,
                             Properties.Settings.Default.Lambda,
                             ql == null ? null : ql.QTable);
                    // fail-safe
                    else { MessageBox.Show("Invalid learning invoke ...", "Ops!!", MessageBoxButtons.OK, MessageBoxIcon.Error); goto __END_LEARNING; }
                    // learn the grid
                    ql.Learn(new Func<Grid, Point, long, bool>((g, s, step_counter) => { return s == g.GoalPoint; }));
                    // sum-up the steps' counters
                    totall_step_counter += ql.StepCounter;
                    // indicate the results
                    this.toolStripStatus.Text = String.Format("{0}% Of {1} episodes passed - Last episode's steps#: {2} - Totall episodes' step#: {3} ", (i + 1) * 100 / (max_iter), ql.GetType().Name, ql.StepCounter, totall_step_counter);
                }
                this.toolStripStatus.Text = String.Format("The model has learned by {0} with total# {1} of steps...", ql.GetType().Name, totall_step_counter);
                this.__plotPolicy(ql);
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

        private void __enable_all_menus(bool p)
        {
            foreach (var c in this.menuStrip1.Items)
            {
                if (c is ToolStripMenuItem)
                {
                    (c as ToolStripMenuItem).Enabled = p;
                    foreach (var sc in (c as ToolStripMenuItem).DropDownItems)
                    {
                        if (sc is ToolStripMenuItem)
                            (sc as ToolStripMenuItem).Enabled = p;
                    }
                }
            }
            Application.DoEvents();
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
                    this.g.AgentPoint = m.NewPoint;
                    this.DrawDirection(m.OldPoint, s.Value);
                    l.Remove(s);
                    if (l.Count == 0)
                        this.optimalPath.Remove(l);
                    else
                        this.optimalPath[this.optimalPath] = l;
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
                            bf.Serialize(fs, ql.VisitedStates);
                            bf.Serialize(fs, ql.StepCounter);
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
                            ql.QTable = (Hashtable)bf.Deserialize(fs);
                            ql.VisitedStates = (Hashtable)bf.Deserialize(fs);
                            ql.StepCounter = (long)bf.Deserialize(fs);
                        }
                    }
                    __reload_grid();
                    __plotPolicy(ql);
                    this.toolStripStatus.Text = "The QTable saved successfully....";
                }
            }
            __enable_all_menus(true);
        }
    }
}