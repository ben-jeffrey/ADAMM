using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAMM {
    public class Heat {
        static Random rng = new Random();
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

        public Entry addRandomEntry(List<Entry> entries) {
            Entry choice = entries[rng.Next(entries.Count)];
            int position = nextPosition();
            HeatEntries.Insert(position-1, choice);
            choice.EntryPosition = position;
            choice.EntryHeat = HeatNumber;
            return choice;
        }

        public Entry addEntryToNext(Entry entry) {
            int position = nextPosition();
            HeatEntries.Insert(position-1, entry);
            entry.EntryPosition = position;
            entry.EntryHeat = HeatNumber;
            return entry;
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

        public List<Entry> EntriesWithEmpties() {
            List<Entry> entriesWithEmpties = new List<Entry>();
            entriesWithEmpties.AddRange(HeatEntries);

            if (HeatEvent.EventPositionCount > 0) {
                for (int i = 1; i <= HeatEvent.EventPositionCount; i++)
                    if (HeatEntries.Where(e => e.EntryPosition == i).Count() == 0)
                        entriesWithEmpties.Insert(i - 1, new Entry(i, HeatNumber, -1, HeatEvent));
            } else {
                entriesWithEmpties.Add(new Entry(HeatEntries.Count()+1, HeatNumber, -1, HeatEvent));
            }

            return entriesWithEmpties;
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

        public int nextPosition() {
            if (HeatEvent.EventPositionCount > 0) {
                int position = HeatEvent.EventPositionCount / 2;
                int alternator = 1;
                for (int i = 1; i < HeatEvent.EventPositionCount; i++) {
                    if (HeatEntries.ElementAtOrDefault(position) != null)
                        return position + 1;
                    position = position + (i * alternator);
                    alternator *= -1;
                }
            }
            return HeatEntries.Count + 1;
        }

        public bool full() {
            return HeatEntries.Count >= HeatEvent.EventPositionCount && HeatEvent.EventPositionCount > 0;
        }
    }
}
