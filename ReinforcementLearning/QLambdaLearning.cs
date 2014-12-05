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
    public class QLambdaLearning : TDLambdaLearning
{
        /// <summary>
        /// Construct a Q-learner instance
        /// </summary>
        /// <param name="grid">The grid instance which trying to learn</param>
        /// <param name="A">The list of valid actions</param>
        /// <param name="gamma">The discount factor</param>
        /// <param name="alpha">The learning rate</param>
        /// <param name="QTable">The initial Q-table(Can be also `null`)</param>
        public QLambdaLearning(Grid grid, List<Action> A, float gamma, float alpha, float lambda, Hashtable QSA) : base(grid, A, gamma, alpha, lambda, QSA) { }
        /// <summary>
        /// Construct a Q-learner instance
        /// </summary>
        /// <param name="grid">The grid instance which trying to learn</param>
        /// <param name="A">The list of valid actions</param>
        /// <param name="gamma">The discount factor</param>
        /// <param name="alpha">The learning rate</param>
        public QLambdaLearning(Grid grid, List<Action> A, float gamma, float alpha, float lambda) : base(grid, A, gamma, lambda, alpha) { }
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
            // define an initial action
            Action a = Action.HOLD;
            // deine the initial state
            State state = this.Grid.AgentPoint;
            do
            {
                // choose a random action
                a = this.__choose_random_action(state);
                // change the destination `state` with respect to the choosen action 
                var s = this.GridHelper.Move(state, a);
                // get the new-state's reward
                var r = this.__get_reward(s.NewPoint);
                // choose a' from s'
                Action aprim = this.__choose_toothily_action(s.NewPoint);
                // update the Q-Value of current with [s, a, s', a'] values
                this.__update_q_value(s.OldPoint, a, r, s.NewPoint, aprim);
                // mark current state-action as visited
                this.__visit(s.OldPoint, a);
                // assign the next state
                state = s.NewPoint;
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
        /// <returns>The updated Q-Value</returns>
        protected override EligVal __update_q_value(State st, Action a, Reward r, State stplus, params object[] aplus)
        {
            if (aplus.Length == 0 || !(aplus[0] is Action))
                throw new ArgumentException("Expecting an action as last comment", "aplus");
            var qt = this.__get_q_value(st, a);
            Action astar = (Action)aplus[0];
            QVal v = this.__get_q_value(stplus, astar);
            foreach (var __a in this.Actions) { var __q = this.__get_q_value(stplus, __a); if (v < __q) { v = __q; astar = __a; } }         // argmaxQ(s', b)
            var delta = (r + this.Gamma * this.__get_q_value(stplus, astar) - this.__get_q_value(st, a));                                   // δ ← r + γ * Q(s', a*) - Q(s, a)
            this.__set_elig_value(st, a, this.__get_elig_value(st, a) + 1);                                                                 // e(s, a) ← e(s, a) + 1
            var keys = this.QTable.Keys.Cast<KeyValuePair<State, Action>>().ToArray();
            for (int i = 0; i < keys.Length; i++)                                                                                           // for each s,a
            {
                var sa = (KeyValuePair<State, Action>)keys[i];
                this.__set_q_value(sa.Key, sa.Value, (QVal)this.QTable[sa] + this.Alpha * delta * this.__get_elig_value(sa.Key, sa.Value)); // Q(s, a) ← Q(s, a) + αδe(s, a) 
                if ((Action)aplus[0] == astar)
                    this.__set_elig_value(sa.Key, sa.Value, this.Gamma * this.Lambda * this.__get_elig_value(sa.Key, sa.Value));            // e(s, a) ← γλe(s, a)
                else
                    this.__set_elig_value(sa.Key, sa.Value, 0);
            }
            return qt;
        }
    }
}
