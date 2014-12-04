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
    using State = Point;
    using Reward = Int16;
    using Action = GridHelper.Directions;
    public class QLearning
    {
        protected Grid Grid { get; set; }
        public static Hashtable QSA { get; protected set; }
        public static Hashtable VisitedSA { get; protected set; }
        protected Point PreviousState { get; set; }
        protected Random RandGen { get; set; }
        protected System.Timers.Timer RefreshTimer { get; set; }
        public List<Action> Actions { get; set; }
        protected GridHelper.Directions PreviousAction { get; set; }
        protected readonly float Alpha;
        protected readonly float Gamma;
        public long StepCounter { get; protected set; }
        static QLearning()
        {
            QLearning.QSA = new Hashtable();
            QLearning.VisitedSA = new Hashtable();
        }
        public QLearning(Grid grid, List<Action> A, float gamma, float alpha)
        {
            this.Grid = grid;
            this.Actions = A;
            this.Alpha = alpha;
            this.Gamma = gamma;
            this.StepCounter = 0;
            this.RandGen = new Random(System.Environment.TickCount);
            this.RefreshTimer = new System.Timers.Timer(400);
            this.RefreshTimer.Elapsed += new System.Timers.ElapsedEventHandler((sender, e) =>
            {
                lock (this.RandGen)
                    this.RandGen = new Random(System.Environment.TickCount + e.SignalTime.Millisecond);
                GC.Collect();
            });
            this.RefreshTimer.Start();
        }
        ~QLearning()
        {
            this.RefreshTimer.Stop();
            this.RefreshTimer.Dispose();
        }
        public void Train(Func<Grid, long, bool> termination_validtor = null)
        {
            if (termination_validtor == null)
                termination_validtor = new Func<Environment.Grid, long, bool>(this.__should_terminate);
            var gh = new GridHelper(this.Grid);
            Action a = Action.HOLD;
            do
            {
                // choose a random action
                lock (this.RandGen)
                    a = this.Actions[this.RandGen.Next(0, this.Actions.Count)];
                var s = gh.Move(this.Grid.AgentPoint, a);
                this.__visit(s.OldPoint, a);
                var r = this.__get_reward(s.NewPoint);
                this.__update_q_value(s.OldPoint, a, s.NewPoint, r);
                // go to the new point
                this.Grid.AgentPoint = s.NewPoint;
            } while (!termination_validtor(this.Grid, this.StepCounter) && ++this.StepCounter <= long.MaxValue);
        }
        /// <summary>
        /// Check if should terminate the training loop
        /// </summary>
        protected bool __should_terminate(Grid grid, long step_counter) { return grid.AgentPoint == grid.GoalPoint; }
        /// <summary>
        /// Get reward based on current state of grid
        /// </summary>
        /// <returns>The reward</returns>
        protected Reward __get_reward(State s) { if (s == this.Grid.GoalPoint) return 10; return 0; }
        /// <summary>
        /// Returns the Q-Value of a State-Action if any exists or initializes it by 0.
        /// </summary>
        /// <param name="s">The state of the agent</param>
        /// <param name="a">The action of the agent</param>
        /// <returns>The Q-Value of current State-Action</returns>
        protected QVal __get_q_value(State s, Action a)
        {
            var sig = new KeyValuePair<State, Action>(s, a);
            if (!QLearning.QSA.Contains(sig))
                QLearning.QSA.Add(sig, 0.0F);
            return (QVal)QLearning.QSA[sig];
        }
        /// <summary>
        /// Sets the Q-Value of a State-Action if any exists or initializes it.
        /// </summary>
        /// <param name="s">The state of the agent</param>
        /// <param name="a">The action of the agent</param>
        /// <returns>The Q-Value of current State-Action</returns>
        protected QVal __set_q_value(State s, Action a, QVal v)
        {
            var sig = new KeyValuePair<State, Action>(s, a);
            if (!QLearning.QSA.Contains(sig))
                QLearning.QSA.Add(sig, v);
            QLearning.QSA[sig] = v;
            return v;
        }
        /// <summary>
        /// Increase the visit rate of a State-Action if any exists or initializes it.
        /// </summary>
        /// <param name="s">The state of the agent</param>
        /// <param name="a">The action of the agent</param>
        /// <returns>The# of current State-Action is visited, counting this time.</returns>
        protected long __visit(State s, Action a)
        {
            var sig = new KeyValuePair<State, Action>(s, a);
            if (!QLearning.VisitedSA.Contains(sig))
                QLearning.VisitedSA.Add(sig, (long)0);
            QLearning.VisitedSA[sig] = (long)QLearning.VisitedSA[sig] + 1;
            return (long)QLearning.VisitedSA[sig];
        }
        /// <summary>
        /// Updates the Q-Value
        /// </summary>
        /// <param name="st">The state</param>
        /// <param name="a">The action</param>
        /// <param name="r">The awarded reward</param>
        /// <returns>The updated Q-Value</returns>
        protected QVal __update_q_value(State st, Action a, State stplus, Reward r)
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
            return this.__set_q_value(st, a, qt);
        }
    }
}
