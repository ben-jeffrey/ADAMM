using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAMM
{
    class Event : IComparable {
        public static MeetDatabase MeetDB;
        public int EventNumber { get; }
        public int EventPointer { get; }
        public String EventName { get; }
        private char EventGender { get; set; }
        public char EventType { get; set; }
        public int EventPositionCount { get; }
        public Division EventDivision { get; set; }
        public char EventStatus { get; set; }
        public List<Heat> EventHeats { get; set; }
        private int level { get; set; }

        public Event(int number, int ptr, char gender, char trkfld, int posCount, Division div, char stat) {
            EventNumber = number;
            EventPointer = ptr;
            EventGender = gender;
            EventType = trkfld;
            EventPositionCount = posCount;
            EventDivision = div;
            EventStatus = stat;
            EventHeats = new List<Heat>();
            EventName = ToString();
            createHeats();
        }

        private void createHeats() {
            List<int[]> entries = MeetDB.getEntries(EventPointer);
            int currentHeat = 0;
            foreach (int[] entry in entries) {
                if (entry[1] != currentHeat) {
                    currentHeat = entry[1];
                    EventHeats.Add(new Heat(this, currentHeat));
                }
                EventHeats.Last().addCompetitor(entry[0], entry[2]);
            }
        }

        public List<Dictionary<int, int>> getHeatEntries() {
            List<Dictionary<int, int>> entries = new List<Dictionary<int, int>>();
            foreach (Heat h in EventHeats) {
                entries.Add(h.LaneAthletes);
            }
            return entries;
        }

        public bool filter(String filter) {
            return EventNumber.ToString().Contains(filter);
        }

        public bool isEligible(Athlete a) {
            return a.AthleteGender == EventGender && a.AthleteDivision == EventDivision;
        }

        public bool containsAthlete(Athlete a) {
            foreach (Heat h in EventHeats)
                if (h.containsAthlete(a))
                    return true;
            return false;
        }

        public string StatusString() {
            switch (EventStatus) {
                case 'S':
                    return "Scored";
                case 'U':
                    return "Unseeded";
                case 'D':
                    return "Done";
                default:
                    return "Unknown";
            }
        }

        public override string ToString() {
            string gender = EventGender == 'M' ? "Men's" : "Women's";
            string type = EventType == 'T' ? " Run" : " Jump";
            return "Event #" + EventNumber + ", " + gender + type;
        }

        public int CompareTo(object obj) {
            return EventNumber.CompareTo(((Event)obj).EventNumber);
        }
    }
}
