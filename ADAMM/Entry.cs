using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAMM {
    public class Entry {
        public int EntryPosition { get; set; }
        public Athlete EntryAthlete { get; set; }
        public Event EntryEvent { get; set; }

        public Entry(int pos, Athlete a, Event e) {
            EntryPosition = pos;
            EntryAthlete = a;
            EntryEvent = e;
        }
    }
}
