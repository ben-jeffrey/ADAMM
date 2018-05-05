using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;
using System.IO;

namespace ADAMM
{
    public class Meet {
        public MeetDatabase MeetDB { get; }
        public String MeetName { get; set; }
        public String MeetDate { get; set; }
        public List<Event> MeetEvents { get; set; }
        public List<Team> MeetTeams { get; set; }
        public List<Division> MeetDivisions { get; set; }
        public string MeetSchedulePath { get; set; }

        // Schedule file used to communicate with Finishlynx timing software
        private ScheduleFile MeetSchedule;
        // Directory to look in for timing data from Finishlynx software
        private string LIFDirectory = @"C:\Users\PinQiblo2\Desktop\Competitions\";

        #region Creation of Meet

        public Meet(string dbFilePath) {
            // Open database at path
            MeetDB = new MeetDatabase(dbFilePath);

            // Get the name and date of the meet
            String[] meetInfo = MeetDB.getMeetInfo();
            MeetName = meetInfo[0];
            MeetDate = meetInfo[1];

            // Propogate the database to other core classes
            Athlete.MeetDB = MeetDB;
            Event.MeetDB = MeetDB;
            Team.MeetDB = MeetDB;
            Heat.MeetDB = MeetDB;

            // Generate divisions, teams, and events
            MeetDivisions = MeetDB.createDivisions();
            MeetTeams = MeetDB.createTeams(MeetDivisions);
            MeetEvents = MeetDB.createEvents(MeetDivisions);
            // Go through all entries and give them athlete references
            PropagateAthletes();

            // Generate schedule files at path
            MeetSchedulePath = @"C:\Users\PinQiblo2\Desktop\Competitions\";
            MeetSchedule = new ScheduleFile(MeetSchedulePath);
            MeetSchedule.CreateSchFile(MeetEvents);
            MeetSchedule.CreatePplFile(this);
            MeetSchedule.CreateEvtFile(MeetEvents);
        }

        // Iterate over every entry in the meet and match an athlete reference to it
        private void PropagateAthletes() {
            foreach (Event e in MeetEvents) {
                if (e.isSeeded())
                    foreach (Heat h in e.EventHeats)
                        foreach (Entry ent in h.HeatEntries)
                            foreach (Team t in MeetTeams) {
                                Athlete found = t.findAthlete(ent.EntryAthletePointer);
                                if (found != null) ent.EntryAthlete = found;
                            }
                foreach (Entry ent in e.EventUnseededEntries)
                    foreach (Team t in MeetTeams) {
                        Athlete found = t.findAthlete(ent.EntryAthletePointer);
                        if (found != null) ent.EntryAthlete = found;
                    }
            }
        }

        #endregion
        
        #region Athletes and Entries

        // Gets all events containing the given athlete
        public List<Event> getEntriesForAthlete(Athlete a) {
            List<Event> entries = new List<Event>();
            foreach (Event e in MeetEvents)
                if (e.containsAthlete(a))
                    entries.Add(e);
            return entries;
        }

        // Gets all events the given athlete could compete in
        public List<Event> getEligibleEventsForAthlete(Athlete a) {
            List<Event> entries = new List<Event>();
            foreach (Event e in MeetEvents)
                if (e.isEligible(a) && !e.containsAthlete(a))
                    entries.Add(e);
            return entries;
        }

        // Get athlete reference given an athlete pointer
        private Athlete findAthlete(int athNum) {
            Athlete found = null;
            foreach (Team t in MeetTeams) {
                found = t.findAthlete(athNum);
                if (found != null)
                    break;
            }
            return found;
        }

        // Add empty athlete to the meet
        public Athlete addNewAthlete() {
            Athlete newAthlete = new Athlete(0, 0, "", "", 'M', null);

            // Defaults to first team
            newAthlete.AthleteTeam = MeetTeams[0];
            newAthlete.AthleteTeam.TeamRoster.Add(newAthlete);

            // Insert athlete into DB
            MeetDB.insertNewAthlete(newAthlete);

            return newAthlete;
        }

        // Update which events an athlete is in
        // Add events the athlete is now in
        // Remove events the athlete is no longer in
        public void updateEntriesForAthlete(Athlete a, List<Event> events) {
            foreach (Event e in events)
                if (!e.containsAthlete(a))
                    addAthleteToEvent(a, e);
            foreach (Event e in getEntriesForAthlete(a)) {
                if (!events.Contains(e))
                    removeAthleteFromEvent(a, e);
            }
        }

        // Shortcut to event's adding method
        public void addAthleteToEvent(Athlete a, Event e) {
            e.addAthlete(a);
        }


        // Shortcut to event's removal method
        public void removeAthleteFromEvent(Athlete a, Event e) {
            e.removeAthlete(a);
        }

        #endregion

        #region Events

        // Add given event to meet object and DB
        public void addNewEvent(Event e) {
            MeetEvents.Add(e);
            MeetDB.insertNewEvent(e);
        }

        // Get list of athletes that are eligble for given event
        // Specifically, athletes that are not already in the event
        public List<Athlete> getEligibleAthletesForEvent(Event e) {
            List<Athlete> eligible = new List<Athlete>();
            foreach (Team t in MeetTeams)
                foreach (Athlete a in t.TeamRoster)
                    if (e.isEligible(a) && !e.containsAthlete(a))
                        eligible.Add(a);
            return eligible;
        }

        // Remove entries from the event in the DB
        public void removeEntries(List<Entry> toRemove) {
            foreach (Entry ent in toRemove)
                MeetDB.removeEntry(ent);
        }

        // Update entries from the event in the DB
        public void updateEntries(List<Entry> toUpdate) {
            foreach (Entry ent in toUpdate)
                MeetDB.updateEntryRecord(ent);
        }

        // Add entries in the event to the DB
        public void addEntries(List<Entry> toAdd) {
            foreach (Entry ent in toAdd)
                MeetDB.insertNewEntry(ent);
        }

        // Update entry results in the DB
        public void resultEntry(Entry e) {
            MeetDB.updateEntryResult(e);
        }

        // Add all entries in an event to the DB
        public void addEventEntries(Event e) {
            List<Entry> flatEntries = new List<Entry>();
            foreach (Heat h in e.EventHeats)
                foreach (Entry ent in h.HeatEntries)
                    flatEntries.Add(ent);
            addEntries(flatEntries);
        }

        // Read a given heat's results from a Finishlynx time file (.LIF)
        public Dictionary<int, string> getFinishlynxTimes(Heat h) {
            Dictionary<int, string> output = new Dictionary<int, string>();
            try {
                // Try to open the file with the name "EventNumber-RoundNumber-HeatNumer.lif"
                //   Round number is always 1 for now
                using (StreamReader file = new StreamReader(LIFDirectory + String.Format("{0:000}-1-{1:00}.lif", h.HeatEvent.EventNumber, h.HeatNumber))) {
                    // Get entire file as a string
                    string LIFcontents = file.ReadToEnd();
                    // Split into lines
                    string[] LIFlines = LIFcontents.Split('\n');
                    // Boolean to help skip the one-line header
                    bool header = false;

                    // For each line, skip the header if necessary, and split the line on commas
                    foreach (string line in LIFlines) {
                        if (!header) {
                            header = true;
                            continue;
                        }
                        string[] LIFcells = line.Split(',');
                        // cell 2 is the lane, cell 6 is the time
                        if (line.Length > 0)
                            output.Add(int.Parse(LIFcells[2]), LIFcells[6]);
                    }
                }
            } catch (FileNotFoundException e) { Console.WriteLine(String.Format("{0:000}-1-{1:00}.lif", h.HeatEvent.EventNumber, h.HeatNumber)); }

            return output;
        }

        #endregion

        #region Teams

        // Update team data to DB
        public void updateTeam(Team t) {
            MeetDB.updateTeamRecord(t);
        }

        #endregion

        #region Miscellanious

        // Find the largest event number and return it + 1
        public int getNextEventNumber() {
            return MeetEvents.Max(e => e.EventNumber) + 1;
        }

        #endregion

        // External close for DB
        public void close() {
            MeetDB.close();
        }
        
        public override string ToString() {
            return MeetName + " " + MeetDate;
        }
    }
}
