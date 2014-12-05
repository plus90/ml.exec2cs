using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Environment;

namespace ReinforcementLearning
{
    using QVal = System.Single;
    using State = System.Drawing.Point;
    using Reward = Int16;
    using Action = Environment.GridHelper.Directions;
    using VVal = System.Single;
    public class TDLambda : RLambdaLearning
    {
        /// <summary>
        /// The V(s) table's container
        /// </summary>
        public Hashtable VTable { get; protected set; }
        /// <summary>
        /// Construct a Q-learner instance
        /// </summary>
        /// <param name="grid">The grid instance which trying to learn</param>
        /// <param name="A">The list of valid actions</param>
        /// <param name="gamma">The discount factor</param>
        /// <param name="alpha">The learning rate</param>
        /// <param name="lambda">The lambda rate</param>
        /// <param name="QSA">The initial Q-Table</param>
        /// <param name="QTable">The initial Q-table(Can be also `null`)</param>
        public TDLambda(Grid grid, List<Action> A, float gamma, float alpha, float lambda, Hashtable QSA) : base(grid, A, gamma, alpha, alpha, QSA) { }
        /// <summary>
        /// Construct a Q-learner instance
        /// </summary>
        /// <param name="grid">The grid instance which trying to learn</param>
        /// <param name="A">The list of valid actions</param>
        /// <param name="gamma">The discount factor</param>
        /// <param name="alpha">The learning rate</param>
        /// <param name="lambda">The lambda rate</param>
        public TDLambda(Grid grid, List<Action> A, float gamma, float alpha, float lambda) : base(grid, A, gamma, alpha, alpha) { }

        public void DoStep(RLearning pi, State s)
        {
            this.RefreshTimer.Start();
            List<Action> actionList = new List<Action>();
            var max = QVal.MinValue;
            foreach (var _a in pi.Actions)
            {
                var v = (QVal)pi.QTable[new KeyValuePair<State, Action>(s, _a)];
                if (v > max)
                {
                    actionList.Clear();
                    actionList.Add(_a);
                    max = v;
                }
                else if (v == max)
                    actionList.Add(_a);
            }
            var a = actionList[this.RandGen.Next(0, actionList.Count - 1)];

            var sprim = this.GridHelper.Move(s, a);

            var r = this.__get_reward(sprim.NewPoint);

            this.__update_v_value(pi, s, r, sprim.NewPoint);

            this.RefreshTimer.Stop();
        }
        /// <summary>
        /// Sets eligibility trace value
        /// </summary>
        /// <param name="s">The state</param>
        /// <param name="a">The action</param>
        /// <param name="e">The eligibility value to be updated</param>
        protected virtual void __set_v_value(State s, VVal e)
        {
            var sig = s;
            if (!this.VTable.Contains(sig))
                this.VTable.Add(sig, e);
            else this.VTable[sig] = e;
        }
        /// <summary>
        /// Gets VTable if any exists or initializes it by 0.
        /// </summary>
        /// <param name="s">The state</param>
        /// <param name="a">The action</param>
        /// <returns>The eligibility trace value</returns>
        protected virtual VVal __get_v_value(State s)
        {
            var sig = s;
            if (!this.VTable.Contains(sig))
                this.VTable.Add(sig, (VVal)0);
            return (VVal)this.VTable[sig];
        }
        /// <summary>
        /// Sets eligibility trace value
        /// </summary>
        /// <param name="s">The state</param>
        /// <param name="a">The action</param>
        /// <param name="e">The eligibility value to be updated</param>
        protected void __set_elig_value(State s, VVal e)
        {
            var sig = s;
            if (!this.VTable.Contains(sig))
                this.VTable.Add(sig, e);
            else this.VTable[sig] = e;
        }
        /// <summary>
        /// Gets VTable if any exists or initializes it by 0.
        /// </summary>
        /// <param name="s">The state</param>
        /// <param name="a">The action</param>
        /// <returns>The eligibility trace value</returns>
        protected VVal __get_elig_value(State s)
        {
            var sig = s;
            if (!this.VTable.Contains(sig))
                this.VTable.Add(sig, (VVal)0);
            return (VVal)this.VTable[sig];
        }

        protected VVal __update_v_value(RLearning pi, State st, Reward r, State stplus)
        {
            var delta = r + this.Gamma * this.__get_v_value(stplus) - this.__get_v_value(st);
            this.__set_elig_value(st, this.__get_elig_value(st) + 1);                                                                       // e(s, a) ← e(s, a) + 1
            var keys = this.QTable.Keys.Cast<KeyValuePair<State, Action>>().ToArray();
            for (int i = 0; i < keys.Length; i++)                                                                                           // for each s,a
            {
                var sa = (KeyValuePair<State, Action>)keys[i];
                this.__set_v_value(sa.Key,  (VVal)this.VTable[sa] + this.Alpha * delta * this.__get_elig_value(sa.Key));                    // Q(s, a) ← Q(s, a) + αδe(s, a) 
                this.__set_elig_value(sa.Key, this.Gamma * this.Lambda * this.__get_elig_value(sa.Key));                // e(s, a) ← γλe(s, a)
            }
            return this.__get_v_value(st);
        }

        protected override VVal __update_q_value(State st, Action a, Reward r, State stplus, params object[] o) { throw new NotSupportedException("This method is not supported for TDLambda instance"); }

        public override Hashtable Learn(Func<Grid, State, long, bool> termination_validtor = null) { throw new NotSupportedException("This method is not supported for TDLambda instance"); }
    }
}