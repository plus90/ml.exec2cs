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
    using EligVal = System.Single;
    public abstract class RLambdaLearning : RLearning
    {
        /// <summary>
        /// The lambda rate
        /// </summary>
        public readonly float Lambda;
        /// <summary>
        /// The eligibility trace container
        /// </summary>
        public Hashtable EligTable { get; protected set; }
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
        public RLambdaLearning(Grid grid, List<Action> A, float gamma, float alpha, float lambda, Hashtable QSA) : base(grid, A, gamma, alpha, QSA) { this.Lambda = lambda; this.EligTable = new Hashtable(); }
        /// <summary>
        /// Construct a Q-learner instance
        /// </summary>
        /// <param name="grid">The grid instance which trying to learn</param>
        /// <param name="A">The list of valid actions</param>
        /// <param name="gamma">The discount factor</param>
        /// <param name="alpha">The learning rate</param>
        /// <param name="lambda">The lambda rate</param>
        public RLambdaLearning(Grid grid, List<Action> A, float gamma, float alpha, float lambda) : base(grid, A, gamma, alpha) { this.Lambda = lambda; this.EligTable = new Hashtable(); }
        /// <summary>
        /// Sets eligibility trace value
        /// </summary>
        /// <param name="s">The state</param>
        /// <param name="a">The action</param>
        /// <param name="e">The eligibility value to be updated</param>
        protected virtual void __set_elig_value(State s, Action a, EligVal e)
        {
            var sig = new KeyValuePair<State, Action>(s, a);
            if (!this.EligTable.Contains(sig))
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
            if (!this.EligTable.Contains(sig))
                this.EligTable.Add(sig, (EligVal)0);
            return (EligVal)this.EligTable[sig];
        }
    }
}
