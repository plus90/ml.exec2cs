using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Environment;
using System.Collections;

namespace ReinforcementLearning
{
    using QVal = System.Single;
    using State = System.Drawing.Point;
    using Reward = Int16;
    using Action = Environment.GridHelper.Directions;
    using NVal = System.Single;
    using UVal = System.Single;
    using StateAction = KeyValuePair<System.Drawing.Point, Environment.GridHelper.Directions>;
    using StateActionState = KeyValuePair<System.Drawing.Point, KeyValuePair<Environment.GridHelper.Directions, System.Drawing.Point>>;
    public class ADP : RLearning, IUtility
    {
        /// <summary>
        /// The U(s) table's container
        /// </summary>
        public Hashtable UTable { get; set; }
        /// <summary>
        /// The T(s,pi(s),s') table's container
        /// </summary>
        public Hashtable TTable { get; set; }
        /// <summary>
        /// The N(s,a) table's container
        /// </summary>
        public Hashtable NSA { get; set; }
        /// <summary>
        /// The N(s,a,s') table's container
        /// </summary>
        public Hashtable NSAS { get; set; }
        /// <summary>
        /// Set or get inital state
        /// </summary>
        public State InitialState { get; set; }
        /// <summary>
        /// Construct a Q-learner instance
        /// </summary>
        /// <param name="grid">The grid instance which trying to learn</param>
        /// <param name="A">The list of valid actions</param>
        /// <param name="gamma">The discount factor</param>
        /// <param name="alpha">The learning rate</param>
        /// <param name="QTable">The initial Q-table(Can be also `null`)</param>
        public ADP(Grid grid, List<Action> A, float gamma, float alpha, Hashtable QSA) : this(grid, A, gamma, alpha) { if (QSA != null)this.QTable = QSA; }
        /// <summary>
        /// Construct a Q-learner instance
        /// </summary>
        /// <param name="grid">The grid instance which trying to learn</param>
        /// <param name="A">The list of valid actions</param>
        /// <param name="gamma">The discount factor</param>
        /// <param name="alpha">The learning rate</param>
        public ADP(Grid grid, List<Action> A, float gamma, float alpha)
            : base(grid, A, gamma, alpha)
        {
            this.NSA = new Hashtable();
            this.NSAS = new Hashtable();
            this.UTable = new Hashtable(); 
            this.TTable = new Hashtable();
            this.InitialState = grid.AgentPoint;

        }
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
        public ADP(RLearning r) : this(r.Grid, r.Actions, r.Gamma, r.Alpha, r.QTable) { this.VisitedStateActions = r.VisitedStateActions; }
        /// <summary>
        /// Sets eligibility trace value
        /// </summary>
        /// <param name="s">The state</param>
        /// <param name="a">The action</param>
        /// <param name="e">The eligibility value to be updated</param>
        protected virtual void __set_u_value(State s, UVal e)
        {
            var sig = s;
            if (!this.UTable.Contains(sig))
                this.UTable.Add(sig, e);
            else this.UTable[sig] = e;
        }
        /// <summary>
        /// Gets UTable if any exists or initializes it by 0.
        /// </summary>
        /// <param name="s">The state</param>
        /// <param name="a">The action</param>
        /// <returns>The eligibility trace value</returns>
        protected virtual UVal __get_u_value(State s)
        {
            var sig = s;
            if (!this.UTable.Contains(sig))
                this.UTable.Add(sig, (UVal)0);
            return (UVal)this.UTable[sig];
        }
        ///// <summary>
        /// Sets eligibility trace value
        /// </summary>
        /// <param name="s">The state</param>
        /// <param name="a">The action</param>
        /// <param name="e">The eligibility value to be updated</param>
        protected virtual void __set_nsa_value(State s, Action a, NVal e)
        {
            var sig = new StateAction(s, a);
            if (!this.NSA.Contains(sig))
                this.NSA.Add(sig, e);
            else this.NSA[sig] = e;
        }
        /// <summary>
        /// Gets UTable if any exists or initializes it by 0.
        /// </summary>
        /// <param name="s">The state</param>
        /// <param name="a">The action</param>
        /// <returns>The eligibility trace value</returns>
        protected virtual NVal __get_nsa_value(State s, Action a)
        {
            var sig = new StateAction(s, a);
            if (!this.NSA.Contains(sig))
                this.NSA.Add(sig, (NVal)0);
            return (NVal)this.NSA[sig];
        }
        ///// <summary>
        /// Sets eligibility trace value
        /// </summary>
        /// <param name="s">The state</param>
        /// <param name="a">The action</param>
        /// <param name="e">The eligibility value to be updated</param>
        protected virtual void __set_nsas_value(State s, Action a, State sprim, NVal e)
        {
            var sig = new StateActionState(s, new KeyValuePair<Action, State>(a, sprim));
            if (!this.NSAS.Contains(sig))
                this.NSAS.Add(sig, e);
            else this.NSAS[sig] = e;
        }
        /// <summary>
        /// Gets UTable if any exists or initializes it by 0.
        /// </summary>
        /// <param name="s">The state</param>
        /// <param name="a">The action</param>
        /// <returns>The eligibility trace value</returns>
        protected virtual NVal __get_t_value(State s, Action a, State sprim)
        {
            var sig = new StateActionState(s, new KeyValuePair<Action, State>(a, sprim));
            if (!this.TTable.Contains(sig))
                this.TTable.Add(sig, (NVal)0);
            return (NVal)this.TTable[sig];
        }
        /// <summary>
        /// Sets eligibility trace value
        /// </summary>
        /// <param name="s">The state</param>
        /// <param name="a">The action</param>
        /// <param name="e">The eligibility value to be updated</param>
        protected virtual void __set_t_value(State s, Action a, State sprim, NVal e)
        {
            var sig = new StateActionState(s, new KeyValuePair<Action, State>(a, sprim));
            if (!this.TTable.Contains(sig))
                this.TTable.Add(sig, e);
            else this.TTable[sig] = e;
        }
        /// <summary>
        /// Gets UTable if any exists or initializes it by 0.
        /// </summary>
        /// <param name="s">The state</param>
        /// <param name="a">The action</param>
        /// <returns>The eligibility trace value</returns>
        protected virtual NVal __get_nsas_value(State s, Action a, State sprim)
        {
            var sig = new StateActionState(s, new KeyValuePair<Action, State>(a, sprim));
            if (!this.NSAS.Contains(sig))
                this.NSAS.Add(sig, (NVal)0);
            return (NVal)this.NSAS[sig];
        }
        protected void __visit(State s, Action a, State sprim)
        {
            var sig = new StateAction(s, a);
            if (!this.VisitedStateActions.Contains(sig))
                this.VisitedStateActions.Add(sig, new List<State>() { sprim });
            else (this.VisitedStateActions[sig] as List<State>).Add(sprim);
        }
        /// <summary>
        /// Learn the grid
        /// </summary>
        /// <param name="termination_validtor">The learning terminator validator; if it returns true the learning operation will halt.</param>
        /// <returns>The learned policy</returns>
        public override Hashtable Learn(Func<Grid, State, long, bool> termination_validtor = null)
        {
            // start the refresher's timer
            this.RefreshTimer.Start();
            // if no termination validator passed
            if (termination_validtor == null)
                // assign the built-in default validator
                termination_validtor = new Func<Environment.Grid, State, long, bool>(this.__should_terminate);
            // deine the initial state
            State state = this.InitialState;
            // define an initial action
            Action a = Action.HOLD;
            this.VisitedStateActions = new Hashtable();
            do
            {
                // choose action given this pi for state
                a = this.__choose_toothily_action(state, 0.1F);
                // change the destination `state` with respect to the choosen action 
                var s = this.GridHelper.Move(state, a);
                // get the new-state's reward
                var r = this.__get_reward(s.NewPoint);
                // update the Q-Value of current with [s, r, a, s'] values
                this.__update_t_value(s.OldPoint, r, a, s.NewPoint);
                this.__update_u_value(s.OldPoint, r, a, s.NewPoint); 
                // assign the next state
                state = s.NewPoint;
                this.__visit(s.OldPoint, a, s.NewPoint);
                // examine the learning loop
            } while (!termination_validtor(this.Grid, state, this.StepCounter) && ++this.StepCounter <= long.MaxValue);
            // stop the refresher's timer
            this.RefreshTimer.Stop();
            // return the learned policy
            return this.UTable;
        }
        /// <summary>
        /// Updates the U-Value
        /// </summary>
        /// <param name="st">The state at `t`</param>
        /// <param name="r">The awarded reward at `t+1`</param>
        /// <param name="a">The taken action at `t`</param>
        /// <param name="stplus">The state at `t+1`</param>
        /// <returns>The updated V-Value</returns>
        private void __update_u_value(State st, Reward r, Action a, State stplus)
        {
            UVal k = 0.0F;
            foreach (var action in this.Actions)
            {
                k += this.__get_t_value(st, action, stplus) * this.__get_u_value(stplus);
            }
            this.__set_u_value(st, r + this.Gamma * k);
        }
        /// <summary>
        /// Updates the T-Value
        /// </summary>
        /// <param name="st">The state at `t`</param>
        /// <param name="r">The awarded reward at `t+1`</param>
        /// <param name="a">The taken action at `t`</param>
        /// <param name="stplus">The state at `t+1`</param>
        /// <returns>The updated V-Value</returns>
        protected void __update_t_value(State st, Reward r, Action a, State stplus)
        {
            if (!this.UTable.Contains(st))
            {
                this.__set_u_value(st, r);
            }
            else
            {
                this.__set_nsa_value(st, a, this.__get_nsa_value(st, a) + 1);
                this.__set_nsas_value(st, a, stplus, this.__get_nsas_value(st, a, stplus) + 1);
                if (!this.VisitedStateActions.Contains(new StateAction(st, a))) return;
                foreach (State t in this.VisitedStateActions[new StateAction(st, a)] as List<State>)
                {
                    if (this.__get_nsas_value(st, a, t) != 0)
                        this.TTable[new StateActionState(st, new KeyValuePair<Action, State>(a, t))] =
                            this.__get_nsas_value(st, a, t) / this.__get_nsa_value(st, a);
                }
            }
        }
        /// <summary>
        /// NOT SUPPORTED
        /// </summary>
        protected override float __update_q_value(System.Drawing.Point st, Environment.GridHelper.Directions a, short r, System.Drawing.Point stplus, params object[] o) { throw new NotSupportedException("THIS METHOD NOT SUPPORTED!"); }
    }
}
