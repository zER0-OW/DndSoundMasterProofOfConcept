using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DndSoundMasterProofOfConcept.LoopManagers
{
   

    public class Loop
    {
        public TimeSpan start { get; }
        public TimeSpan end { get; }

        public Loop(TimeSpan start, TimeSpan end)
        {
            this.start = start;
            this.end = end;
        }
    }
}
