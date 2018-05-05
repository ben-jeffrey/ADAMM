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
        
        // Offical event number
        public int EventNumber { get; }

        // Internal primary key
        public int EventPointer { get { return pointer; } set { pointer = value; OnPropertyChanged(); } }
        int pointer;

        public char EventGender { get { return gender; } set { gender = value; OnPropertyChanged(); } }
        char gender;

        public int EventPositionCount { get { return positionCount; } set { positionCount = value; OnPropertyChanged(); } }
        int positionCount;

        public Division EventDivision { get { return division; } set { division = value; OnPropertyChanged(); } }
        Division division;

        public string EventStatus { get { return StatusString(); } set { status = value[0]; OnPropertyChanged(); OnPropertyChanged("EventStatusColor"); } }
        private char status;

        // Dispaly color matched with display status
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

        // Called after events are generated to populate them with entries
        public void populate() {
            if (EventPointer >= 0 && isSeeded()) createHeats();
            // Unseeded entries are necessary for all events, in case it needs to be seeded again
            EventUnseededEntries = MeetDB.getUnseededEntries(this);
        }

        // Populates event with entries from the DB
        private void createHeats() {
            // Get entries from DB
            List<Entry> entries = MeetDB.getEntries(this);
            
            // Iterate over entries to find the number of heats
            int totalHeats = 0;
            foreach (Entry e in entries)
                if (e.EntryHeat > totalHeats)
                    totalHeats = e.EntryHeat;

            // Create the correct number of empty heats
            for (int i = 0; i < totalHeats; i++)
                EventHeats.Add(new Heat(this, i+1));
            
            // Fill heats with the relevant entries
            foreach (Entry e in entries)
                if (e.EntryHeat > 0)
                    EventHeats[e.EntryHeat-1].HeatEntries.Add(e);
        }

        // Adds athlete to a heat in the event
        public void addAthlete(Athlete a) {
            // Add athlete to the first empty heat
            Boolean added = false;
            foreach (Heat h in EventHeats)
                if (!h.full()) {
                    h.addAthlete(a);
                    added = true;
                    break;
                }

            // If there was no empty heat, make one and add the athlete to it
            if (!added) {
                Heat h = new Heat(this, EventHeats.Count);
                h.addAthlete(a);
                EventHeats.Add(h);
            }
        }

        // Remove athlete from event
        public void removeAthlete(Athlete a) {
            foreach (Heat h in EventHeats)
                h.removeAthlete(a);
        }

        // Gets list of heats' entry lists
        public List<List<Entry>> getHeatEntries() {
            List<List<Entry>> entries = new List<List<Entry>>();
            foreach (Heat h in EventHeats) {
                entries.Add(h.HeatEntries);
            }
            return entries;
        }

        // Each type of event scores differnetly
        // Any event type must implement a score method
        public virtual void Score() { }

        // Return true if the event is not filtered out by the query string
        // Currently only event numbers are queried on
        public bool filter(String filter) {
            return EventNumber.ToString().Contains(filter);
        }

        // Returns true if athlete is eligible for this event
        public bool isEligible(Athlete a) {
            return a.AthleteGender == EventGender && a.AthleteDivision == EventDivision;
        }

        // Returns true if athlete is in this event
        public bool containsAthlete(Athlete a) {
            foreach (Heat h in EventHeats)
                if (h.containsAthlete(a))
                    return true;
            return false;
        }

        // Shortcut for checking seeding status
        public bool isSeeded() {
            return status != 'U';
        }

        /* Converting status characters into strings and colors */
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
                case '1': return "LightBlue";
                default: return "Red";
            }
        }

        // Public dict for converting event names to type charaters
        public static Dictionary<string, char> CategoryChars = new Dictionary<string, char>
        {{"Dash", 'A'}, {"Run", 'B'}, {"Race Walk", 'D'}, {"Hurdles", 'E'}, {"High Jump", 'K'},
        { "Pole Vault", 'L'}, {"Long Jump", 'M'}, {"Triple Jump", 'N'}, {"Discus", 'O'},
        { "Javelin", 'Q'}, { "Shot Put", 'R'}, {"Relay", 'W'}};

        // Switch function to give the type name for the type character
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
            string div = EventDivision == null? "" : EventDivision.DivisionName;
            return String.Format("{0} {1} {2} {3}", gender, distance, type, div);
        }

        // Comparator to allow sorting by event numbers
        public int CompareTo(object obj) {
            return EventNumber.CompareTo(((Event)obj).EventNumber);
        }

        // Raise property changed event to properly update UI
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            if (this.PropertyChanged != null)
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
