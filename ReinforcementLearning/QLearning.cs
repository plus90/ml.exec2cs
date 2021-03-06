﻿using System;
using System.Collections;
using System.Collections.Generic;
using Environment;
namespace ReinforcementLearning
{
    using Action = GridHelper.Directions;
    using QVal = System.Single;
    using Reward = Int16;
    using State = System.Drawing.Point;
    public class QLearning : RLearning
    {
        /// <summary>
        /// Construct a Q-learner instance
        /// </summary>
        /// <param name="grid">The grid instance which trying to learn</param>
        /// <param name="A">The list of valid actions</param>
        /// <param name="gamma">The discount factor</param>
        /// <param name="alpha">The learning rate</param>
        /// <param name="QTable">The initial Q-table(Can be also `null`)</param>
        public QLearning(Grid grid, List<Action> A, float gamma, float alpha, Hashtable QSA) : base(grid, A, gamma, alpha, QSA) { }
        /// <summary>
        /// Construct a Q-learner instance
        /// </summary>
        /// <param name="grid">The grid instance which trying to learn</param>
        /// <param name="A">The list of valid actions</param>
        /// <param name="gamma">The discount factor</param>
        /// <param name="alpha">The learning rate</param>
        public QLearning(Grid grid, List<Action> A, float gamma, float alpha) : base(grid, A, gamma, alpha) { }
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
                // mark current state-action as visited
                this.__visit(s.OldPoint, a);
                // get the new-state's reward
                var r = this.__get_reward(s.NewPoint);
                // update the Q-Value of current with [s, a, s'] values
                this.__update_q_value(s.OldPoint, a, r, s.NewPoint);
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
        protected override QVal __update_q_value(State st, Action a, Reward r, State stplus, params object[] o)
        {
            var qt = this.__get_q_value(st, a);
            QVal v = QVal.MinValue;
            // argmaxQ(s', b)                   
            foreach (var __a in this.Actions)
            {
                var __q = this.__get_q_value(stplus, __a);
                if (v < __q)
                    v = __q;
            }
            // Q(s, a) ← (1 - α)Q(s, a) + α[r + γ * argmaxQ(s', b)]
            qt = (1 - this.Alpha) * qt + this.Alpha * (r + this.Gamma * v);
            this.__set_q_value(st, a, qt);
            return qt;
        }
    }
}