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
    public abstract class TDLearning
    {
        /// <summary>
        /// The defined valid actions
        /// </summary>
        public List<Action> Actions { get; protected set; }
        /// <summary>
        /// The Q-Table
        /// </summary>
        public Hashtable QTable { get; protected set; }
        /// <summary>
        /// The learning steps counter
        /// </summary>
        public long StepCounter { get; protected set; }
        /// <summary>
        /// The visited states' container
        /// </summary>
        public Hashtable VisitedStates { get; protected set; }
        /// <summary>
        /// The grid instance
        /// </summary>
        protected Grid Grid { get; set; }
        /// <summary>
        /// The grid's helper instance
        /// </summary>
        protected GridHelper GridHelper { get; set; }
        /// <summary>
        /// The random# generator
        /// </summary>
        protected Random RandGen { get; set; }
        /// <summary>
        /// The refresher timer
        /// </summary>
        protected System.Timers.Timer RefreshTimer { get; set; }
        /// <summary>
        /// The learning rate
        /// </summary>
        protected readonly float Alpha;
        /// <summary>
        /// The discount factor
        /// </summary>
        protected readonly float Gamma;
        /// <summary>
        /// Construct a TD learner instance
        /// </summary>
        /// <param name="grid">The grid instance which trying to learn</param>
        /// <param name="A">The list of valid actions</param>
        /// <param name="gamma">The discount factor</param>
        /// <param name="alpha">The learning rate</param>
        /// <param name="QTable">The initial Q-table(Can be also `null`)</param>
        public TDLearning(Grid grid, List<Action> A, float gamma, float alpha, Hashtable QTable)
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
        public TDLearning(Grid grid, List<Action> A, float gamma, float alpha)
        {
            this.Grid = grid;
            this.Actions = A;
            this.Alpha = alpha;
            this.Gamma = gamma;
            this.StepCounter = 0;
            this.QTable = new Hashtable();
            this.VisitedStates = new Hashtable();
            this.GridHelper = new GridHelper(this.Grid);
            this.RandGen = new Random(System.Environment.TickCount);
            this.RefreshTimer = new System.Timers.Timer(400);
            this.RefreshTimer.Elapsed += new System.Timers.ElapsedEventHandler((sender, e) =>
            {
                lock (this.RandGen)
                    this.RandGen = new Random(System.Environment.TickCount + e.SignalTime.Millisecond);
                GC.Collect();
            });
        }
        /// <summary>
        /// The destructor
        /// </summary>
        ~TDLearning()
        {
            this.RefreshTimer.Stop();
            this.RefreshTimer.Dispose();
        }
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
        /// <summary>
        /// Randomly chooses an action
        /// </summary>
        /// <returns>The choosen action</returns>
        protected virtual Action __choose_action()
        {
            // The init choose procedure
            lock (this.RandGen)
                // choose a random action
                return this.Actions[this.RandGen.Next(0, this.Actions.Count)];
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
    }
}
