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
    public abstract class RLearning : RLInstance
    {
        #region Members
        /// <summary>
        /// The defined valid actions
        /// </summary>
        public List<Action> Actions { get; protected set; }
        /// <summary>
        /// The Q-Table
        /// </summary>
        public Hashtable QTable { get; set; }
        /// <summary>
        /// The learning steps counter
        /// </summary>
        public long StepCounter { get; set; }
        /// <summary>
        /// The visited states' container
        /// </summary>
        public Hashtable VisitedStates { get; set; }
        /// <summary>
        /// The grid instance
        /// </summary>
        public Grid Grid { get; protected set; }
        /// <summary>
        /// The grid's helper instance
        /// </summary>
        public GridHelper GridHelper { get; protected set; }
        /// <summary>
        /// The learning rate
        /// </summary>
        public readonly float Alpha;
        /// <summary>
        /// The discount factor
        /// </summary>
        public readonly float Gamma;
        #endregion

        #region Constructors
        /// <summary>
        /// Construct a TD learner instance
        /// </summary>
        /// <param name="grid">The grid instance which trying to learn</param>
        /// <param name="A">The list of valid actions</param>
        /// <param name="gamma">The discount factor</param>
        /// <param name="alpha">The learning rate</param>
        /// <param name="QTable">The initial Q-table(Can be also `null`)</param>
        public RLearning(Grid grid, List<Action> A, float gamma, float alpha, Hashtable QTable)
            : this(grid, A, gamma, alpha)
        {
            if (QTable != null)
                this.QTable = QTable;
        }
        /// <summary>
        /// Construct a TD learner instance
        /// </summary>
        /// <param name="grid">The grid instance which trying to learn</param>
        /// <param name="A">The list of valid actions</param>
        /// <param name="gamma">The discount factor</param>
        /// <param name="alpha">The learning rate</param>
        public RLearning(Grid grid, List<Action> A, float gamma, float alpha)
            : base()
        {
            this.Grid = grid;
            this.Actions = A;
            this.Alpha = alpha;
            this.Gamma = gamma;
            this.StepCounter = 0;
            this.QTable = new Hashtable();
            this.VisitedStates = new Hashtable();
            this.GridHelper = new GridHelper(this.Grid);
        }
        #endregion
        
        #region Methods
        #region Protecteds
        /// <summary>
        /// Returns the Q-Value of a State-Action if any exists or initializes it by 0.
        /// </summary>
        /// <param name="s">The state of the agent</param>
        /// <param name="a">The action of the agent</param>
        /// <returns>The Q-Value of current State-Action</returns>
        protected QVal __get_q_value(State s, Action a)
        {
            // craete a signature of current State-Action
            var sig = new KeyValuePair<State, Action>(s, a);
            // if the signature has NOT been registered
            if (!this.QTable.Contains(sig))
                // initialize the signature with 0
                this.QTable.Add(sig, 0.0F);
            // return the Q-value of the signature
            return (QVal)this.QTable[sig];
        }
        /// <summary>
        /// Sets the Q-Value of a State-Action if any exists or initializes it.
        /// </summary>
        /// <param name="s">The state of the agent</param>
        /// <param name="a">The action of the agent</param>
        protected void __set_q_value(State s, Action a, QVal v)
        {
            // craete a signature of current State-Action
            var sig = new KeyValuePair<State, Action>(s, a);
            // if the signature has NOT been registered
            if (!this.QTable.Contains(sig))
                // initialize the signature with the value
                this.QTable.Add(sig, v);
            // initialize the signature with the value
            this.QTable[sig] = v;
        }
        /// <summary>
        /// Increase the visit rate of a State-Action if any exists or initializes it.
        /// </summary>
        /// <param name="s">The state of the agent</param>
        /// <param name="a">The action of the agent</param>
        /// <returns>The# of current State-Action is visited, counting this time.</returns>
        protected long __visit(State s, Action a)
        {
            // craete a signature of current State-Action
            var sig = new KeyValuePair<State, Action>(s, a);
            // if the signature has NOT been registered
            if (!this.VisitedStates.Contains(sig))
                // initialize the signature with 0
                this.VisitedStates.Add(sig, (long)0);
            // increase the visit# of the signature by one
            this.VisitedStates[sig] = (long)this.VisitedStates[sig] + 1;
            // return the updated visit#
            return (long)this.VisitedStates[sig];
        }
        #endregion
        #region Virtuals
        /// <summary>
        /// Randomly chooses an action
        /// </summary>
        /// <returns>The choosen action</returns>
        protected virtual Action __choose_random_action(State s) { lock (this.RandGen) /* choose a random action */ return this.Actions[this.RandGen.Next(0, this.Actions.Count)]; }
        /// <summary>
        /// Toothily chooses an action
        /// </summary>
        /// <param name="s">For current state</param>
        /// <param name="EXPLORATION_RATIO">The exploration ratio in range of [0..1]</param>
        /// <returns>The choosen action</returns>
        protected virtual Action __choose_toothily_action(State s, float EXPLORATION_RATIO = 0.2F)
        {
            if (this.RandGen.NextDouble() <= EXPLORATION_RATIO)
            {
                // return the base's choose action
                // which selects a random action
                return this.__choose_random_action(s);
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
                    return this.__choose_random_action(s);
                // randomly choose an action from the maximum actions' list
                return actions[this.RandGen.Next(0, actions.Count)];
            }
        }
        /// <summary>
        /// Get reward based on current state of grid
        /// </summary>
        /// <returns>The reward</returns>
        protected virtual Reward __get_reward(State s) { if (s == this.Grid.GoalPoint) return 10; return 0; }
        /// <summary>
        /// Check if should terminate the training loop
        /// </summary>
        protected virtual bool __should_terminate(Grid grid, State s, long step_counter) { return s == grid.GoalPoint; }
        #endregion
        #region Abstracts
        /// <summary>
        /// Updates the Q-Value
        /// </summary>
        /// <param name="st">The state at `t`</param>
        /// <param name="a">The action at `t`</param>
        /// <param name="stplus">The state at `t+1`</param>
        /// <param name="r">The awarded reward at `t+1`</param>
        /// <returns>The updated Q-Value</returns>
        protected abstract QVal __update_q_value(State st, Action a, Reward r, State stplus, params object[] o);
        /// <summary>
        /// Learn the grid
        /// </summary>
        /// <param name="termination_validtor">The learning terminator validator; if it returns true the learning operation will halt.</param>
        /// <returns>The learned policy</returns>
        public abstract Hashtable Learn(Func<Grid, State, long, bool> termination_validtor = null);
        #endregion
        #endregion
    }
}