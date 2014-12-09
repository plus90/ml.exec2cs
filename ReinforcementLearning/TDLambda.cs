using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Environment;

namespace ReinforcementLearning
{
    using Action = Environment.GridHelper.Directions;
    using Reward = Int16;
    using State = System.Drawing.Point;
    using VVal = System.Single;
    public class TDLambda : RLambdaLearning, IUtility
    {
        /// <summary>
        /// The V(s) table's container
        /// </summary>
        public Hashtable UTable { get; set; }
        /// <summary>
        /// Set or get inital state
        /// </summary>
        public State InitialState { get; set; }
        /// <summary>
        /// Construct a TD(lambda)-learner instance
        /// </summary>
        /// <param name="grid">The grid instance which trying to learn</param>
        /// <param name="A">The list of valid actions</param>
        /// <param name="gamma">The discount factor</param>
        /// <param name="alpha">The learning rate</param>
        /// <param name="lambda">The lambda factor</param>
        /// <param name="QSA">The initial Q-table(Can be also `null`)</param>
        public TDLambda(Grid grid, List<Action> A, float gamma, float alpha, float lambda, Hashtable QSA) : this(grid, A, gamma, alpha, lambda) { if (QSA != null)this.QTable = QSA; }
        /// <summary>
        /// Construct a TD(lambda)-learner instance
        /// </summary>
        /// <param name="grid">The grid instance which trying to learn</param>
        /// <param name="A">The list of valid actions</param>
        /// <param name="gamma">The discount factor</param>
        /// <param name="alpha">The learning rate</param>
        /// <param name="lambda">The lambda factor</param>
        public TDLambda(Grid grid, List<Action> A, float gamma, float alpha, float lambda) : base(grid, A, gamma, lambda, alpha) { this.UTable = new Hashtable(); this.InitialState = grid.AgentPoint; }
        /// <summary>
        /// Construct a TD(lambda)-learner instance
        /// </summary>
        /// <param name="r">The Reinforcement learner instance</param>
        /// <param name="lambda">The lambda factor</param>
        public TDLambda(RLearning r, float lambda) : this(r.Grid, r.Actions, r.Gamma, r.Alpha, lambda, r.QTable) { this.VisitedStateActions = r.VisitedStateActions; this.VisitedState = r.VisitedState; }
        /// <summary>
        /// Sets eligibility trace value
        /// </summary>
        /// <param name="s">The state</param>
        /// <param name="a">The action</param>
        /// <param name="e">The eligibility value to be updated</param>
        protected virtual void __set_u_value(State s, VVal e) { var sig = s; if (!this.UTable.Contains(sig)) this.UTable.Add(sig, e); else this.UTable[sig] = e; }
        /// <summary>
        /// Gets UTable if any exists or initializes it by 0.
        /// </summary>
        /// <param name="s">The state</param>
        /// <param name="a">The action</param>
        /// <returns>The eligibility trace value</returns>
        protected virtual VVal __get_u_value(State s) { var sig = s; if (!this.UTable.Contains(sig)) this.UTable.Add(sig, (VVal)0); return (VVal)this.UTable[sig]; }
        /// <summary>
        /// Sets eligibility trace value
        /// </summary>
        /// <param name="s">The state</param>
        /// <param name="a">The action</param>
        /// <param name="e">The eligibility value to be updated</param>
        protected void __set_elig_value(State s, VVal e) { var sig = s; if (!this.EligTable.Contains(sig)) this.EligTable.Add(sig, e); else this.EligTable[sig] = e; }
        /// <summary>
        /// Gets eligibility trace value if any exists or initializes it by 0.
        /// </summary>
        /// <param name="s">The state</param>
        /// <param name="a">The action</param>
        /// <returns>The eligibility trace value</returns>
        protected VVal __get_elig_value(State s) { var sig = s; if (!this.EligTable.Contains(sig)) this.EligTable.Add(sig, (VVal)0); return (VVal)this.EligTable[sig]; }
        /// <summary>
        /// Learn U-values of a policy
        /// </summary>
        /// <param name="termination_validtor">The learning terminator validator; if it returns true the learning operation will halt.</param>
        /// <returns>The learned U-values</returns>
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
                this.__update_u_value(s.OldPoint, r, s.NewPoint);
                // assign the next state
                state = s.NewPoint;
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
        /// <param name="stplus">The state at `t+1`</param>
        /// <returns>The updated U-Value</returns>
        protected VVal __update_u_value(State st, Reward r, State stplus)
        {
            // δ ← r + γ * V(s') - V(s)
            var delta = r + this.Gamma * this.__get_u_value(stplus) - this.__get_u_value(st);
            // e(s) ← e(s) + 1
            this.__set_elig_value(st, this.__get_elig_value(st) + 1);                                                                       
            var keys = this.VisitedState.Keys.Cast<State>().ToArray();
            // for each s,a
            for (int i = 0; i < keys.Length; i++)                                                                                           
            {
                var s = keys[i];
                // V(s) ← V(s, a) + αδe(s)
                this.__set_u_value(s, this.__get_u_value(s) + this.Alpha * delta * this.__get_elig_value(s));
                // e(s, a) ← γλe(s, a)
                this.__set_elig_value(s, this.Gamma * this.Lambda * this.__get_elig_value(s));
            }
            return this.__get_u_value(st);
        }
        /// <summary>
        /// NOT SUPPORTED
        /// </summary>
        protected override VVal __update_q_value(State st, Action a, Reward r, State stplus, params object[] o) { throw new NotSupportedException("This method is not supported for TDLambda instance"); }
    }
}