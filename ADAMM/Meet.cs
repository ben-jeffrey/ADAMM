using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;

namespace ADAMM
{
    public class Meet {
        public MeetDatabase MeetDB { get; }
        private String MeetName;
        private String MeetDate;
        public List<Event> MeetEvents { get; set; }
        public List<Team> MeetTeams { get; set; }
        public List<Division> MeetDivisions { get; set; }

        public static Athlete EmptyAthlete = new Athlete(-1, -1, "", "", ' ', null);
        public static Event EmtpyEvent = new Event(-1, -1, ' ', 0, null, ' ', ' ', 0, ' ');

        #region Creation of Meet

        public Meet(string dbFilePath) {
            MeetDB = new MeetDatabase(dbFilePath);
            String[] meetInfo = MeetDB.getMeetInfo();
            MeetName = meetInfo[0];
            MeetDate = meetInfo[1];

            Athlete.MeetDB = MeetDB;
            Event.MeetDB = MeetDB;
            Team.MeetDB = MeetDB;
            Heat.MeetDB = MeetDB;

            MeetDivisions = MeetDB.createDivisions();
            MeetTeams = MeetDB.createTeams(MeetDivisions);
            MeetEvents = MeetDB.createEvents(MeetDivisions);
            PropagateAthletes();
        }

        private void PropagateAthletes() {
            foreach (Event e in MeetEvents)
                foreach (Heat h in e.EventHeats)
                    foreach (Entry ent in h.HeatEntries)
                        foreach (Team t in MeetTeams) {
                            Athlete found  = t.findAthlete(ent.EntryAthletePointer);
                            if (found != null) ent.EntryAthlete = found;
                        }
        }

        #endregion
        
        #region Athletes and Entries

        public List<Event> getEntriesForAthlete(Athlete a) {
            List<Event> entries = new List<Event>();
            foreach (Event e in MeetEvents)
                if (e.containsAthlete(a))
                    entries.Add(e);
            return entries;
        }

        public List<Event> getEligibleEventsForAthlete(Athlete a) {
            List<Event> entries = new List<Event>();
            foreach (Event e in MeetEvents)
                if (e.isEligible(a) && !e.containsAthlete(a))
                    entries.Add(e);
            return entries;
        }

        private Athlete findAthlete(int athNum) {
            Athlete found = null;
            foreach (Team t in MeetTeams) {
                found = t.findAthlete(athNum);
                if (found != null)
                    break;
            }
            return found;
        }

        public Athlete addNewAthlete() {
            Athlete newAthlete = new Athlete(0, 0, "", "", 'M', null);

            newAthlete.AthleteTeam = MeetTeams[0];
            newAthlete.AthleteTeam.TeamRoster.Add(newAthlete);

            MeetDB.insertNewAthlete(newAthlete);

            return newAthlete;
        }

        public void updateEntries(Athlete a, List<Event> events) {
            foreach (Event e in events)
                if (!e.containsAthlete(a))
                    addAthleteToEvent(a, e);
            foreach (Event e in getEntriesForAthlete(a)) {
                if (!events.Contains(e))
                    removeAthleteFromEvent(a, e);
            }
        }

        public void addAthleteToEvent(Athlete a, Event e) {
            e.addAthlete(a);
        }

        public void removeAthleteFromEvent(Athlete a, Event e) {
            e.removeAthlete(a);
        }

        #endregion

        #region Events

        public void addNewEvent(Event e) {
            MeetEvents.Add(e);
            MeetDB.insertNewEvent(e);
        }

        public List<Athlete> getEligibleAthletesForEvent(Event e) {
            List<Athlete> eligible = new List<Athlete>();
            foreach (Team t in MeetTeams)
                foreach (Athlete a in t.TeamRoster)
                    if (e.isEligible(a))
                        eligible.Add(a);
            return eligible;
        }

        #endregion

        #region Miscellanious

        public int getNextEventNumber() {
            return MeetEvents.Max(e => e.EventNumber) + 1;
        }

        #endregion

        public void close() {
            MeetDB.close();
        }
        
        public override string ToString() {
            return MeetName + " " + MeetDate;
        }
    }
}
