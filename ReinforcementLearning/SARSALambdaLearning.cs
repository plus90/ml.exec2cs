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
    using Action = GridHelper.Directions;
    using EligVal = System.Single;
    public class SARSALambdaLearning : SarsaLearning
    {
        /// <summary>
        /// The lambda rate
        /// </summary>
        protected readonly float Lambda;
        public Hashtable EligTable { get; protected set; }
        /// <summary>
        /// Construct a Q-learner instance
        /// </summary>
        /// <param name="grid">The grid instance which trying to learn</param>
        /// <param name="A">The list of valid actions</param>
        /// <param name="gamma">The discount factor</param>
        /// <param name="alpha">The learning rate</param>
        /// <param name="QTable">The initial Q-table(Can be also `null`)</param>
        public SARSALambdaLearning(Grid grid, List<Action> A, float gamma, float alpha, float lambda, Hashtable QSA) : base(grid, A, gamma, alpha, QSA) { this.Lambda = lambda; this.EligTable = new Hashtable(); }
        /// <summary>
        /// Construct a Q-learner instance
        /// </summary>
        /// <param name="grid">The grid instance which trying to learn</param>
        /// <param name="A">The list of valid actions</param>
        /// <param name="gamma">The discount factor</param>
        /// <param name="alpha">The learning rate</param>
        public SARSALambdaLearning(Grid grid, List<Action> A, float gamma, float alpha, float lambda) : base(grid, A, gamma, alpha) { this.Lambda = lambda; this.EligTable = new Hashtable(); }
        /// <summary>
        /// Sets eligibility trace value
        /// </summary>
        /// <param name="s">The state</param>
        /// <param name="a">The action</param>
        /// <param name="e">The eligibility value to be updated</param>
        protected virtual void __set_elig_value(State s, Action a, EligVal e)
        {
            var sig = new KeyValuePair<State, Action>(s, a);
            if (this.EligTable.Contains(sig))
                this.EligTable.Add(sig, e);
            else this.EligTable[sig] = e;
        }
        /// <summary>
        /// Gets eligibility trace value if any exists or initializes it by 0.
        /// </summary>
        /// <param name="s">The state</param>
        /// <param name="a">The action</param>
        /// <returns>The eligibility trace value</returns>
        protected virtual EligVal __get_elig_value(State s, Action a)
        {
            var sig = new KeyValuePair<State, Action>(s, a);
            if (this.EligTable.Contains(sig))
                this.EligTable.Add(sig, 0);
            return (EligVal)this.EligTable[sig];
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
            State state = this.Grid.AgentPoint;
            // define an initial action
            Action a = Action.HOLD;
            do
            {
                // change the destination `state` with respect to the choosen action 
                var s = this.GridHelper.Move(state, a);
                // mark current state-action as visited
                this.__visit(s.OldPoint, a);
                // get the new-state's reward
                var r = this.__get_reward(s.NewPoint);
                // choose a' from s'
                Action aprim = this.__choose_action(s.NewPoint);
                // update the Q-Value of current with [s, a, s', r] values
                this.__update_q_value(s.OldPoint, a, r, s.NewPoint, aprim);
                // assign the next state
                state = s.NewPoint;
                // assign the next action
                a = aprim;
                // examine the learning loop
            } while (!termination_validtor(this.Grid, state, this.StepCounter) && ++this.StepCounter <= long.MaxValue);
            // stop the refresher's timer
            this.RefreshTimer.Stop();
            // return the learned policy
            return this.QTable;
        }
        /// <summary>
        /// Updates the Q-Value
        /// </summary>
        /// <param name="st">The state at `t`</param>
        /// <param name="a">The action at `t`</param>
        /// <param name="r">The awarded reward at `t+1`</param>
        /// <param name="stplus">The state at `t+1`</param>
        /// <param name="aplus">The action at `t+1`</param>
        /// <returns>The updated Q-Value</returns>
        protected override EligVal __update_q_value(State st, Action a, Reward r, State stplus, params object[] aplus)
        {
            if (aplus.Length == 0 || !(aplus[0] is Action))
                throw new ArgumentException("Expecting an action as last comment", "o");
            var delta = (r + this.Gamma * this.__get_q_value(stplus, (Action)aplus[0]) - this.__get_q_value(st, a));                        // δ <- r + γ * Q(s', a') - Q(s, a)
            this.__set_elig_value(st, a, this.__get_elig_value(st, a) + 1);                                                                 // e(s, a) <- e(s, a) + 1
            foreach (KeyValuePair<State, Action> sa in this.QTable.Keys)                                                                    // for all s, a
            {
                this.__set_q_value(sa.Key, sa.Value, (QVal)this.QTable[sa] + this.Alpha * delta * this.__get_elig_value(sa.Key, sa.Value)); // Q(s, a) ← Q(s, a) + αδe(s, a) 
                this.__set_elig_value(sa.Key, sa.Value, this.Gamma * this.Lambda * this.__get_elig_value(sa.Key, sa.Value));                // e(s, a) ← γλe(s, a)
            }
            return 0.0F;
        }
    }
}
