using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAMM {
    public class Entry {
        public int EntryPosition { get; set; }
        public int EntryHeat { get; set; }
        public int EntryAthletePointer { get; set; }
        public Athlete EntryAthlete { get; set; }
        public Event EntryEvent { get; set; }

        public Entry(int pos, int h, int a, Event e) {
            EntryPosition = pos;
            EntryHeat = h;
            EntryAthletePointer = a;
            EntryAthlete = null;
            EntryEvent = e;
        }
    }
}
