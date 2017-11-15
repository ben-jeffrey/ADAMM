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
        private char EventGender { get; set; }
        public char EventType { get; set; }
        public int EventPositionCount { get; }
        public Division EventDivision { get; set; }
        public string EventStatus { get { return StatusString(); } set { status = value[0]; } }
        private char status;
        public string EventCategory { get { return CategoryString(); } set { category = value[0]; } }
        private char category;
        public int EventDistance {get; set;}
        public char EventMeasure { get; set; }
        public List<Heat> EventHeats { get; set; }
        private int level { get; set; }

        public Event(int number, int ptr, char gender, char trkfld, int posCount, Division div, char stat, char cat, int dist, char measure) {
            EventNumber = number;
            EventPointer = ptr;
            EventGender = gender;
            EventType = trkfld;
            EventPositionCount = posCount;
            EventDivision = div;
            status = stat;
            category = cat;
            EventDistance = dist;
            EventMeasure = measure;
            EventHeats = new List<Heat>();
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
            switch (status) {
                case 'S': return "Scored";
                case 'U': return "Unseeded";
                case 'A': return "Done";
                default: return "Unknown";
            }
        }

        public string CategoryString() {
            switch(category) {
                case 'A': return "Dash";
                case 'B': return "Run";
                case 'D': return "Race Walk";
                case 'E': return "Hurdles";
                case 'K': return "High Jump";
                case 'L': return "Pole Vault";
                case 'M': return "Long Jump";
                case 'N': return "Triple Jump";
                case 'O': return "Discus";
                case 'Q': return "Javelin";
                case 'R': return "Shot Put";
                case 'W': return "Relay";
                default: return "Unknown";
            }
        }

        public override string ToString() {
            string gender = EventGender == 'M' ? "Mens " : "Womens ";
            string distance = "";
            if (EventType == 'T')
                distance = EventDistance.ToString() + " " + (EventMeasure == 'M' ? "Meter " : "Yard ");
            string type = CategoryString();
            string div = EventDivision.DivisionName;
            return String.Format("{0}{1}{2} {3}", gender, distance, type, div);
        }

        public int CompareTo(object obj) {
            return EventNumber.CompareTo(((Event)obj).EventNumber);
        }
    }
}
