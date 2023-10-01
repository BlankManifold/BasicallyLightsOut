namespace Managers
{
    public partial class StatsManager
    {
        public enum Dim {FOURxFOUR, FIVExFIVE ,SIXxSIX, SEVENxSEVEN};
        public enum Type { SINGLE, AVG5, AVG12};
        private Godot.Collections.Dictionary _statsDict = new Godot.Collections.Dictionary()
        {
            // new Godot.Collections.Dictionary()
            // { 
            //     {
            //         Dim.FOURxFOUR, new Godot.Collections.Dictionary()
            //         {
            //             {Type.SINGLE,0},
            //             {Type.AVG5,0},
            //             {Type.AVG12,0}
            //         }
            //     },
            //     {
            //         Dim.FIVExFIVE, new Godot.Collections.Dictionary()
            //         {
            //             {Type.SINGLE,0},
            //             {Type.AVG5,0},
            //             {Type.AVG12,0}
            //         }
            //     },
            //     {
            //         Dim.SIXxSIX, new Godot.Collections.Dictionary()
            //         {
            //             {Type.SINGLE,0},
            //             {Type.AVG5,0},
            //             {Type.AVG12,0}
            //         }
            //     },
            //     {
            //         Dim.SEVENxSEVEN, new Godot.Collections.Dictionary()
            //         {
            //             {Type.SINGLE,0},
            //             {Type.AVG5,0},
            //             {Type.AVG12,0}
            //         }
            //     }
            // }
        };
    }
}