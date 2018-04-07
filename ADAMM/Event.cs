using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ADAMM
{
    public class Event : IComparable, INotifyPropertyChanged {
        public static MeetDatabase MeetDB;
        public event PropertyChangedEventHandler PropertyChanged;

        public string EventName { get { return ToString(); } }
        public int EventNumber { get; }
        public int EventPointer { get { return pointer; } set { pointer = value; OnPropertyChanged(); } }
        int pointer;
        public char EventGender { get { return gender; } set { gender = value; OnPropertyChanged(); } }
        char gender;
        public int EventPositionCount { get { return positionCount; } set { positionCount = value; OnPropertyChanged(); } }
        int positionCount;
        public Division EventDivision { get { return division; } set { division = value; OnPropertyChanged(); } }
        Division division;
        public string EventStatus { get { return StatusString(); } set { status = value[0]; OnPropertyChanged(); } }
        private char status;
        public string EventStatusColor { get { return StatusColor(); } }
        public string EventCategory { get { return CategoryString(); } set { category = value[0]; OnPropertyChanged(); } }
        private char category;
        public int EventDistance { get { return distance; } set { distance = value; OnPropertyChanged(); } }
        int distance;
        public char EventUnit { get { return unit; } set { unit = value; OnPropertyChanged(); } }
        char unit;
        public List<Heat> EventHeats { get { return heats; } set { heats = value; OnPropertyChanged(); } }
        List<Heat> heats;
        public List<Entry> EventUnseededEntries { get { return unseededEntries; } set { unseededEntries = value; OnPropertyChanged(); } }
        List<Entry> unseededEntries;

        public Event(int number, int ptr, char gen, int posCount, Division div, char stat, char cat, int dist, char un) {
            EventNumber = number;
            pointer = ptr;
            gender = gen;
            positionCount = posCount;
            division = div;
            status = stat;
            category = cat;
            distance = dist;
            unit = un;
            heats = new List<Heat>();
            unseededEntries = new List<Entry>();
        }

        public void populate() {
            if (EventPointer >= 0 && isSeeded()) createHeats();
            EventUnseededEntries = MeetDB.getUnseededEntries(this);
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            if (this.PropertyChanged != null)
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
