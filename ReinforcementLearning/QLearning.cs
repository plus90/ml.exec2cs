using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Environment;
using System.Collections;
using System.Drawing;
namespace ReinforcementLearning
{
    using QVal = System.Single;
    using State = System.Drawing.Point;
    using Reward = Int16;
    using Action = GridHelper.Directions;
    public class QLearning : TDLearning
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
        /// Updates the Q-Value
        /// </summary>
        /// <param name="st">The state at `t`</param>
        /// <param name="a">The action at `t`</param>
        /// <param name="stplus">The state at `t+1`</param>
        /// <param name="r">The awarded reward at `t+1`</param>
        /// <returns>The updated Q-Value</returns>
        protected override QVal __update_q_value(State st, Action a, State stplus, Reward r)
        {
            var qt = this.__get_q_value(st, a);
            QVal v = QVal.MinValue;
            var gh = new GridHelper(this.Grid);
            foreach (var __a in this.Actions)
            {
                var __q = this.__get_q_value(stplus, __a);
                if (v < __q)
                    v = __q;
            }
            qt = (1 - this.Alpha) * qt + this.Alpha * (r + this.Gamma * v);
            this.__set_q_value(st, a, qt);
            return qt;
        }
    }
}