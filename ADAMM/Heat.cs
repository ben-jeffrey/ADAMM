using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAMM {
    class Heat {
        public Event HeatEvent { get; set; }
        public int HeatNumber { get; set; }
        public static MeetDatabase MeetDB;
        public Dictionary<int, int> LaneAthletes { get; }

        public Heat(Event evt, int ht) {
            HeatEvent = evt;
            HeatNumber = ht;
            LaneAthletes = new Dictionary<int, int>();
        }

        public List<int> getOrderedEntries() {
            List<int> athNums = new List<int>();
            foreach (KeyValuePair<int, int> k in LaneAthletes) {
                athNums.Insert(k.Key-1, k.Value);
            }
            return athNums;
        }

        public void addCompetitor(int ath, int lane) {
            LaneAthletes.Add(lane, ath);
        }

        public bool containsAthlete(Athlete a) {
            foreach (KeyValuePair<int, int> l in LaneAthletes) 
                if (l.Value == a.AthletePointer)
                    return true;
            return false;
        }

        public bool full() {
            return LaneAthletes.Keys.Max() >= HeatEvent.EventPositionCount;
        }
    }
}
