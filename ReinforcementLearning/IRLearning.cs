using System;
namespace ReinforcementLearning
{
    interface IRLearning
    {
        System.Collections.Generic.List<global::Environment.GridHelper.Directions> Actions { get; }
        global::Environment.Grid Grid { get; }
        global::Environment.GridHelper GridHelper { get; }
        System.Collections.Hashtable Learn(Func<global::Environment.Grid, System.Drawing.Point, long, bool> termination_validtor = null);
        System.Collections.Hashtable QTable { get; set; }
        long StepCounter { get; set; }
        System.Collections.Hashtable VisitedState { get; set; }
        System.Collections.Hashtable VisitedStateActions { get; set; }
    }
}
