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
    public partial class GridForm
    {
        Grid g { get; set; }
        Hashtable optimalPath = new Hashtable();
        Hashtable TDLambdaUtilityProgress = new Hashtable();
        Hashtable ADPUtilityProgress = new Hashtable();
        List<KeyValuePair<Hashtable, Hashtable>> policyHistory = null;
        ReinforcementLearning.RLearning ql = null;
        ReinforcementLearning.TDLambda tdl = null;
        ReinforcementLearning.ADP adp = null;
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
        private void __kill_threads() { lock (this.ThreadsPool) foreach (var t in this.ThreadsPool) if (t.IsAlive) t.Abort(); }
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
        private void __plot_policy(ReinforcementLearning.RLearning ql)
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
                foreach (KeyValuePair<Point, GridHelper.Directions> cell in ql.VisitedStateActions.Keys)
                {
                    long count = (long)ql.VisitedStateActions[cell];
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
        private void __learn_policy(object sender)
        {
            int max_iter = Properties.Settings.Default.MaxLearningIteration;
            long totall_step_counter = 0;
            var Actions = new List<GridHelper.Directions>(Enum.GetValues(typeof(GridHelper.Directions)).Cast<GridHelper.Directions>());
            tdl = null;
            this.policyHistory = new List<KeyValuePair<Hashtable, Hashtable>>();
            for (int i = 0; i < max_iter; i++)
            {
                // if the Q-Learning has been invoked?
                if (sender == this.qLearningToolStripMenuItem)
                    // init the Q-learning instance
                    ql = new ReinforcementLearning.QLearning(
                         this.g,
                         Actions,
                         Properties.Settings.Default.Gamma,
                         Properties.Settings.Default.Alpha,
                         ql == null ? null : ql.QTable);
                // if the SARSA-Learning has been invoked?
                else if (sender == this.SARSAToolStripMenuItem)
                    // init the SARSA-learning instance
                    ql = new ReinforcementLearning.SarsaLearning(
                         this.g,
                         Actions,
                         Properties.Settings.Default.Gamma,
                         Properties.Settings.Default.Alpha,
                         ql == null ? null : ql.QTable);
                else if (sender == this.sARSALambdaToolStripMenuItem)
                    ql = new ReinforcementLearning.SarsaLambdaLearning(
                         this.g,
                         Actions,
                         Properties.Settings.Default.Gamma,
                         Properties.Settings.Default.Alpha,
                         Properties.Settings.Default.Lambda,
                         ql == null ? null : ql.QTable);
                else if (sender == this.qLambdaToolStripMenuItem)
                    ql = new ReinforcementLearning.QLambdaLearning(
                         this.g,
                         Actions,
                         Properties.Settings.Default.Gamma,
                         Properties.Settings.Default.Alpha,
                         Properties.Settings.Default.Lambda,
                         ql == null ? null : ql.QTable);
                // fail-safe
                else { MessageBox.Show("Invalid learning invoke ...", "Ops!!", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                // learn the grid
                ql.Learn(new Func<Grid, Point, long, bool>((g, s, step_counter) => { return s == g.GoalPoint; }));
                // clone the QTbale
                policyHistory.Add(new KeyValuePair<Hashtable, Hashtable>(ql.QTable.Clone() as System.Collections.Hashtable, ql.VisitedState.Clone() as System.Collections.Hashtable));
                // sum-up the steps' counters
                totall_step_counter += ql.StepCounter;
                // indicate the results
                this.toolStripStatus.Text = String.Format("{0}% Of {1} episodes passed - Last episode's steps#: {2} - Totall episodes' step#: {3} ", (i + 1) * 100 / (max_iter), ql.GetType().Name, ql.StepCounter, totall_step_counter);
            }
            this.toolStripStatus.Text = String.Format("The model has learned by {0} with total# {1} of steps...", ql.GetType().Name, totall_step_counter);
            this.__plot_policy(ql);
            this.__build_UTable(this.policyHistory);
        }

        private void __plot_utility(ReinforcementLearning.IUtility tdl, ReinforcementLearning.IUtility adp)
        {
            var margin = 23;
            var i = 0;
            using (var gfx = this.grid.CreateGraphics())
            {
                foreach (var util in new List<ReinforcementLearning.IUtility> { tdl, adp })
                {
                    foreach (Point cell in util.UTable.Keys)
                    {
                        var p = g.abs2grid(cell);
                        var f = (float)util.UTable[cell];
                        var txt = f.ToString("0.##");
                        if (i == 0)
                        {
                            p = new Point(p.X + 2 * margin / 3, p.Y + margin + 7);
                        }
                        else
                        {
                            p = new Point(p.X - 2 * margin, p.Y + margin + 7);
                        }
                        this.g.Write((i == 0 ? "T" : "A") + txt, p, gfx, Brushes.Brown, new Font("Arial", 8, FontStyle.Bold));
                    }
                    i += 1;
                }
            }
        }

        private void __build_UTable(List<KeyValuePair<Hashtable, Hashtable>> policyHistory)
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart((thread) =>
            {
                var origin_txt = this.toolStripStatus.Text;
                this.toolStripStatus.Text = "Calculating utility values...";
                List<Hashtable> tdlist = new List<Hashtable>();
                tdl = new ReinforcementLearning.TDLambda(ql, Properties.Settings.Default.Lambda);
                TDLambdaUtilityProgress = new Hashtable();
                float c = 0;
                foreach (var epic in policyHistory)
                {
                    //tdl.QTable = epic.Key;
                    foreach (Point state in epic.Value.Keys)
                    {
                        tdl.InitialState = state;
                        tdl.Learn(new Func<Grid, Point, long, bool>((g, s, step_counter) => { return s == g.GoalPoint; }));
                        // store td-lambda utility progress for the state
                        if (TDLambdaUtilityProgress.Contains(state))
                            (TDLambdaUtilityProgress[state] as List<float>).Add((float)tdl.UTable[state]);
                        else
                            TDLambdaUtilityProgress.Add(state, new List<float>() { (float)tdl.UTable[state] });
                    }
                    this.toolStripStatus.Text = String.Format("[ {0:F1}% ] Calculating utility values...", (++c / (2 * policyHistory.Count)) * 100);
                    tdlist.Add(tdl.UTable);
                }
                adp = new ReinforcementLearning.ADP(ql);
                List<Hashtable> adplist = new List<Hashtable>();
                ADPUtilityProgress = new Hashtable();
                foreach (var epic in policyHistory)
                {
                    //tdl.QTable = epic.Key;
                    foreach (Point state in epic.Value.Keys)
                    {
                        adp.InitialState = state;
                        adp.Learn(new Func<Grid, Point, long, bool>((g, s, step_counter) => { return s == g.GoalPoint; }));
                        // store td-lambda utility progress for the state
                        if (ADPUtilityProgress.Contains(state))
                            (ADPUtilityProgress[state] as List<float>).Add((float)adp.UTable[state]);
                        else
                            ADPUtilityProgress.Add(state, new List<float>() { (float)adp.UTable[state] });
                    }
                    this.toolStripStatus.Text = String.Format("[ {0:F1}% ] Calculating utility values...", (++c / (2 * policyHistory.Count)) * 100);
                    adplist.Add(adp.UTable);
                }
                __plot_utility(tdl, adp);
                this.toolStripStatus.Text = origin_txt;
                ThreadsPool.Remove(thread as System.Threading.Thread);
            }));
            t.Start(t);
            ThreadsPool.Add(t);

        }
    }
}
