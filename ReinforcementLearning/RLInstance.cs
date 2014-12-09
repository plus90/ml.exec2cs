using System;

namespace ReinforcementLearning
{
    public abstract class RLInstance
    {
        /// <summary>
        /// The random# generator
        /// </summary>
        protected Random RandGen { get; set; }
        /// <summary>
        /// The refresher timer
        /// </summary>
        protected System.Timers.Timer RefreshTimer { get; set; }
        /// <summary>
        /// Construct a Reinforcement Learning instance
        /// </summary>
        public RLInstance()
        {
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
        /// Destruct the instace
        /// </summary>
        ~RLInstance()
        {
            this.RefreshTimer.Stop();
            this.RefreshTimer.Dispose();
        }
    }
}
