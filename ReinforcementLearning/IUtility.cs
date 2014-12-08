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
    using VVal = System.Single;
    public interface IUtility
    {
        /// <summary>
        /// The V(s) table's container
        /// </summary>
        Hashtable UTable { get; set; }
        /// <summary>
        /// Set or get inital state
        /// </summary>
        State InitialState { get; set; }
        /// <summary>
        /// Learn the grid
        /// </summary>
        /// <param name="termination_validtor">The learning terminator validator; if it returns true the learning operation will halt.</param>
        /// <returns>The learned policy</returns>
        Hashtable Learn(Func<Grid, State, long, bool> termination_validtor = null);
    }
}
