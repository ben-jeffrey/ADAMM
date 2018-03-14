using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAMM
{
    public class Event : IComparable {
        public static MeetDatabase MeetDB;
        public int EventNumber { get; }
        public int EventPointer { get; set; }
        public char EventGender { get; set; }
        public int EventPositionCount { get; }
        public Division EventDivision { get; set; }
        public string EventStatus { get { return StatusString(); } set { status = value[0]; } }
        private char status;
        public string EventStatusColor { get { return StatusColor(); } }
        public string EventCategory { get { return CategoryString(); } set { category = value[0]; } }
        private char category;
        public int EventDistance {get; set;}
        public char EventUnit { get; set; }
        public List<Heat> EventHeats { get; set; }
        public List<Entry> EventUnseededEntries { get; set; }
        private int level { get; set; }

        public Event(int number, int ptr, char gender, int posCount, Division div, char stat, char cat, int dist, char unit) {
            EventNumber = number;
            EventPointer = ptr;
            EventGender = gender;
            EventPositionCount = posCount;
            EventDivision = div;
            status = stat;
            category = cat;
            EventDistance = dist;
            EventUnit = unit;
            EventHeats = new List<Heat>();
            EventUnseededEntries = new List<Entry>();
            if (ptr >= 0 && isSeeded()) createHeats();
            else EventUnseededEntries = MeetDB.getUnseededEntries(this);
        }

        private void createHeats() {
            List<Entry> entries = MeetDB.getEntries(this);
            int totalHeats = 0;
            foreach (Entry e in entries)
                if (e.EntryHeat > totalHeats)
                    totalHeats = e.EntryHeat;

            for (int i = 0; i < totalHeats; i++)
                EventHeats.Add(new Heat(this, i+1));
            
            foreach (Entry e in entries)
                EventHeats[e.EntryHeat-1].HeatEntries.Add(e);
        }

        public void addAthlete(Athlete a) {
            Boolean added = false;
            foreach (Heat h in EventHeats)
                if (!h.full()) {
                    h.addAthlete(a);
                    added = true;
                    break;
                }
            if (!added) {
                Heat h = new Heat(this, EventHeats.Count);
                h.addAthlete(a);
                EventHeats.Add(h);
            }
        }

        public void removeAthlete(Athlete a) {
            foreach (Heat h in EventHeats)
                h.removeAthlete(a);
        }

        public List<List<Entry>> getHeatEntries() {
            List<List<Entry>> entries = new List<List<Entry>>();
            foreach (Heat h in EventHeats) {
                entries.Add(h.HeatEntries);
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

        public bool isSeeded() {
            return status != 'U';
        }

        public string StatusString() {
            switch (status) {
                case 'S': return "Scored";
                case 'U': return "Unseeded";
                case 'A': return "Done";
                case '1': return "Seeded";
                default: return "Unknown";
            }
        }

        public string StatusColor() {
            switch (status) {
                case 'S': return "Green";
                case 'U': return "White";
                case 'A': return "Gray";
                case '1': return "Blue";
                default: return "Red";
            }
        }

        public static Dictionary<string, char> CategoryChars = new Dictionary<string, char>
        {{"Dash", 'A'}, {"Run", 'B'}, {"Race Walk", 'D'}, {"Hurdles", 'E'}, {"High Jump", 'K'},
        { "Pole Vault", 'L'}, {"Long Jump", 'M'}, {"Triple Jump", 'N'}, {"Discus", 'O'},
        { "Javelin", 'Q'}, { "Shot Put", 'R'}, {"Relay", 'W'}};

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
            string gender = EventGender == 'M' ? "Mens" : "Womens";
            string distance = EventDistance.ToString() + " " + (EventUnit == 'M' ? "Meter" : "Yard");
            string type = CategoryString();
            string div = EventDivision.DivisionName;
            return String.Format("{0} {1} {2} {3}", gender, distance, type, div);
        }

        public int CompareTo(object obj) {
            return EventNumber.CompareTo(((Event)obj).EventNumber);
        }
    }
}
