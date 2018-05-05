using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAMM {
    public class Heat {
        // Used for getting random entries in seeding
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

        // Adds athlete to this heat
        public void addAthlete(Athlete a) {
            // Look for an empty land
            for (int l = 1; l <= HeatEvent.EventPositionCount; l++) {
                Boolean empty = true;
                foreach (Entry e in HeatEntries)
                    if (l == e.EntryPosition) {
                        empty = false;
                        break;
                    }
                // When an empty is found
                if (empty) {
                    // Create an entry and add it
                    HeatEntries.Add(new Entry(l, HeatNumber, a.AthletePointer, HeatEvent));
                    // Update the DB
                    MeetDB.insertNewEntry(a, HeatEvent, this, l);
                    break;
                }
            }
        }

        // Used exclusively for random seeding
        public Entry addRandomEntry(List<Entry> entries) {
            // Choose a random entry
            Entry choice = entries[rng.Next(entries.Count)];

            // Get the next land in seeding order
            int position = nextPosition();

            // Put the entry in that lane
            HeatEntries.Insert(position-1, choice);
            choice.EntryPosition = position;
            choice.EntryHeat = HeatNumber;

            // Return entry for convenience in seeding
            return choice;
        }

        // Adds given entry to the next open lane in seeding order
        public Entry addEntryToNext(Entry entry) {
            int position = nextPosition();
            HeatEntries.Insert(position-1, entry);
            entry.EntryPosition = position;
            entry.EntryHeat = HeatNumber;
            return entry;
        }

        // Removes athlete's entry from entry list
        public void removeAthlete(Athlete a) {
            HeatEntries.RemoveAll(e => e.EntryAthlete == a);
        }

        // Return entry at given position
        public Entry getEntryAt(int pos) {
            foreach (Entry e in HeatEntries)
                if (e.EntryPosition == pos)
                    return e;
            return null;
        }

        // Get a list of all entries, with empty posisitions containing a blank entry
        public List<Entry> EntriesWithEmpties() {
            List<Entry> entriesWithEmpties = new List<Entry>();
            entriesWithEmpties.AddRange(HeatEntries);

            // Iterate to the largest filled position
            // Fill empty positions as we go with blank entries
            for (int i = 1; i <= HeatEntries.Max(e => e.EntryPosition); i++)
                if (HeatEntries.Where(e => e.EntryPosition == i).Count() == 0)
                    entriesWithEmpties.Insert(i - 1, new Entry(i, HeatNumber, -1, HeatEvent));

            // If field event, add another entry at the end to make appending athletes easier for the user
            if (HeatEvent is FieldEvent)
                entriesWithEmpties.Add(new Entry(HeatEntries.Max(e => e.EntryPosition)+1, HeatNumber, -1, HeatEvent));

            return entriesWithEmpties;
        }

        // Returns true if heat contains given athlete
        public bool containsAthlete(Athlete a) {
            foreach (Entry e in HeatEntries) 
                if (e.EntryAthletePointer == a.AthletePointer)
                    return true;
            return false;
        }

        // Returns true if given position has an athlete in it
        public Boolean positionFilled(int pos) {
            foreach (Entry e in HeatEntries)
                if (e.EntryPosition == pos)
                    return true;
            return false;
        }

        // Returns the next empty position in 'seeded order'
        // Example:
        //  6 lanes = 3, 4, 2, 5, 1, 6
        public int nextPosition() {
            // If there a position counter, find the middle and count outwards, alternating sides
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
            // If there's no maximum, then use the position after the last one
            return HeatEntries.Count + 1;
        }

        // Returns true if heat cannot hold more athletes
        public bool full() {
            return HeatEntries.Count >= HeatEvent.EventPositionCount && HeatEvent.EventPositionCount > 0;
        }
    }
}
