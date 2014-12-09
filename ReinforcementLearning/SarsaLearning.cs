using System;
using System.Collections;
using System.Collections.Generic;
using Environment;
namespace ReinforcementLearning
{
    using Action = GridHelper.Directions;
    using QVal = System.Single;
    using Reward = Int16;
    using State = System.Drawing.Point;
    public class SarsaLearning : RLearning
    {
        /// <summary>
        /// Construct a SARSA-learner instance
        /// </summary>
        /// <param name="grid">The grid instance which trying to learn</param>
        /// <param name="A">The list of valid actions</param>
        /// <param name="gamma">The discount factor</param>
        /// <param name="alpha">The learning rate</param>
        /// <param name="QTable">The initial Q-table(Can be also `null`)</param>
        public SarsaLearning(Grid grid, List<Action> A, float gamma, float alpha, Hashtable QSA) : base(grid, A, gamma, alpha, QSA) { }
        /// <summary>
        /// Construct a SARSA-learner instance
        /// </summary>
        /// <param name="grid">The grid instance which trying to learn</param>
        /// <param name="A">The list of valid actions</param>
        /// <param name="gamma">The discount factor</param>
        /// <param name="alpha">The learning rate</param>
        public SarsaLearning(Grid grid, List<Action> A, float gamma, float alpha) : base(grid, A, gamma, alpha) { }
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
            Action a = this.__choose_toothily_action(state);
            do
            {
                // change the destination `state` with respect to the choosen action 
                var s = this.GridHelper.Move(state, a);
                // get the new-state's reward
                var r = this.__get_reward(s.NewPoint);
                // choose a' from s'
                Action aprim = this.__choose_toothily_action(s.NewPoint);
                // update the Q-Value of current with [s, a, s', r] values
                this.__update_q_value(s.OldPoint, a, r, s.NewPoint, aprim);
                // mark current state-action as visited
                this.__visit(s.OldPoint, a);
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
        protected override QVal __update_q_value(State st, Action a, Reward r, State stplus, params object[] aplus)
        {
            if (aplus.Length == 0 || !(aplus[0] is Action))
                throw new ArgumentException("Expecting an action as last comment", "aplus");
            var qt = this.__get_q_value(st, a);
            var v = this.__get_q_value(stplus, (Action)aplus[0]);
            // Q(s, a) ← (1 - α)Q(s, a) + α[r + γ * Q(s', a')]
            qt = (1 - this.Alpha) * qt + this.Alpha * (r + this.Gamma * v);
            this.__set_q_value(st, a, qt);
            return qt;
        }
    }
}