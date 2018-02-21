using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAMM {
    public class Heat {
        public Event HeatEvent { get; set; }
        public int HeatNumber { get; set; }
        public static MeetDatabase MeetDB;
        public List<Entry> HeatEntries { get; }

        public Heat(Event evt, int ht) {
            HeatEvent = evt;
            HeatNumber = ht;
            HeatEntries = new List<Entry>();
        }

        public void addAthlete(Athlete a) {
            for (int l = 1; l <= HeatEvent.EventPositionCount; l++) {
                Boolean empty = true;
                foreach (Entry e in HeatEntries)
                    if (l == e.EntryPosition) {
                        empty = false;
                        break;
                    }
                if (empty) {
                    HeatEntries.Add(new Entry(l, HeatNumber, a.AthletePointer, HeatEvent));
                    MeetDB.insertNewEntry(a, HeatEvent, this, l);
                    break;
                }
            }
        }

        public void removeAthlete(Athlete a) {
            HeatEntries.RemoveAll(e => e.EntryAthlete == a);
        }

        public List<int> getOrderedEntries() {
            List<int> athNums = new List<int>();
            foreach (Entry e in HeatEntries) {
                athNums.Insert(e.EntryPosition, e.EntryAthletePointer);
            }
            return athNums;
        }

        public bool containsAthlete(Athlete a) {
            foreach (Entry e in HeatEntries) 
                if (e.EntryAthletePointer == a.AthletePointer)
                    return true;
            return false;
        }

        public Boolean positionFilled(int pos) {
            foreach (Entry e in HeatEntries)
                if (e.EntryPosition == pos)
                    return true;
            return false;
        }

        public bool full() {
            return HeatEntries.Count >= HeatEvent.EventPositionCount;
        }
    }
}
