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
        public Hashtable VTable { get; set; }
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
        public TDLambda(Grid grid, List<Action> A, float gamma, float alpha, float lambda, Hashtable QSA) : this(grid, A, gamma, alpha, lambda) { if (QSA != null)this.QTable = QSA; }
        /// <summary>
        /// Construct a Q-learner instance
        /// </summary>
        /// <param name="grid">The grid instance which trying to learn</param>
        /// <param name="A">The list of valid actions</param>
        /// <param name="gamma">The discount factor</param>
        /// <param name="alpha">The learning rate</param>
        public TDLambda(Grid grid, List<Action> A, float gamma, float alpha, float lambda) : base(grid, A, gamma, lambda, alpha) { this.VTable = new Hashtable(); this.InitialState = grid.AgentPoint; }
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
        public TDLambda(RLearning r, float lambda) : this(r.Grid, r.Actions, r.Gamma, r.Alpha, lambda, r.QTable) {  this.VisitedStateActions = r.VisitedStateActions; }
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
        /// <summary>
        /// Learn V-values of a policy
        /// </summary>
        /// <param name="termination_validtor">The learning terminator validator; if it returns true the learning operation will halt.</param>
        /// <returns>The learned V-Values</returns>
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
            do
            {
                // choose action given this pi for state
                a = this.__choose_toothily_action(state, 0.1F);
                // change the destination `state` with respect to the choosen action 
                var s = this.GridHelper.Move(state, a);
                // get the new-state's reward
                var r = this.__get_reward(s.NewPoint);
                // update the Q-Value of current with [s, r, s'] values
                this.__update_v_value(s.OldPoint, r, s.NewPoint);
                // assign the next state
                state = s.NewPoint;
                // examine the learning loop
            } while (!termination_validtor(this.Grid, state, this.StepCounter) && ++this.StepCounter <= long.MaxValue);
            // stop the refresher's timer
            this.RefreshTimer.Stop();
            // return the learned policy
            return this.VTable;
        }
        /// <summary>
        /// Updates the V-Value
        /// </summary>
        /// <param name="st">The state at `t`</param>
        /// <param name="r">The awarded reward at `t+1`</param>
        /// <param name="stplus">The state at `t+1`</param>
        /// <returns>The updated V-Value</returns>
        protected VVal __update_v_value(State st, Reward r, State stplus)
        {
            var delta = r + this.Gamma * this.__get_v_value(stplus) - this.__get_v_value(st);                                               // δ ← r + γ * V(s') - V(s)
            this.__set_elig_value(st, this.__get_elig_value(st) + 1);                                                                       // e(s) ← e(s) + 1
            var keys = this.VisitedState.Keys.Cast<State>().ToArray();
            for (int i = 0; i < keys.Length; i++)                                                                                           // for each s,a
            {
                var s = keys[i];
                this.__set_v_value(s, (VVal)this.VTable[s] + this.Alpha * delta * this.__get_elig_value(s));                               // V(s) ← V(s, a) + αδe(s) 
                this.__set_elig_value(s, this.Gamma * this.Lambda * this.__get_elig_value(s));                                             // e(s, a) ← γλe(s, a)
            }
            return this.__get_v_value(st);
        }
        /// <summary>
        /// NOT SUPPORTED
        /// </summary>
        protected override VVal __update_q_value(State st, Action a, Reward r, State stplus, params object[] o) { throw new NotSupportedException("This method is not supported for TDLambda instance"); }
    }
}