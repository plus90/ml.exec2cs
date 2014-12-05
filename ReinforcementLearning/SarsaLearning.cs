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
    public class SarsaLearning : TDLearning
    {
        /// <summary>
        /// Construct a Q-learner instance
        /// </summary>
        /// <param name="grid">The grid instance which trying to learn</param>
        /// <param name="A">The list of valid actions</param>
        /// <param name="gamma">The discount factor</param>
        /// <param name="alpha">The learning rate</param>
        /// <param name="QTable">The initial Q-table(Can be also `null`)</param>
        public SarsaLearning(Grid grid, List<Action> A, float gamma, float alpha, Hashtable QSA) : base(grid, A, gamma, alpha, QSA) { }
        /// <summary>
        /// Construct a Q-learner instance
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
            Action a = this.__choose_action(state);
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
        protected override Action __choose_action(State s)
        {
            // 20% of time choose random move
            const float EXPLORATION_RATIO = 0.2F;
            if (this.RandGen.NextDouble() <= EXPLORATION_RATIO)
            {
                // return the base's choose action
                // which selects a random action
                return base.__choose_action(s);
            }
            else
            {
                // list of actions which have maximum Q-Value in current state
                List<Action> actions = new List<Action>();
                // latest larg Q value
                float max_q = QVal.MinValue;
                // choose actions which have maximum Q-Value
                foreach (var a in this.Actions)
                {
                    // get the Q-Value for current actions
                    var qv = this.__get_q_value(s, a);
                    // if it is a bigger than previously actions 
                    if (qv > max_q)
                    {
                        // update the maximum threshold
                        max_q = qv;
                        // ignore all previously choosen actions
                        actions.Clear();
                        // add current action to the list
                        actions.Add(a);
                    }
                    // it has the same value as previously choosen actions
                    else if (qv == max_q)
                    {
                        // add current action to the list also
                        actions.Add(a);
                    }
                    // ignore the actions who don't have the maximum Q-Value for current state
                }
                // it should never happen, but for fail safe
                if (actions.Count == 0)
                    return base.__choose_action(s);
                // randomly choose an action from the maximum actions' list
                return actions[this.RandGen.Next(0, actions.Count)];
            }
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
                throw new ArgumentException("Expecting an action as last comment", "o");
            var qt = this.__get_q_value(st, a);
            var v = this.__get_q_value(stplus, (Action)aplus[0]);
            qt = (1 - this.Alpha) * qt + this.Alpha * (r + this.Gamma * v);
            this.__set_q_value(st, a, qt);
            return qt;
        }
    }
}