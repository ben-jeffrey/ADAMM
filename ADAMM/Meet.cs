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
            MeetEvents = MeetDB.createEvents(MeetDivisions);
            MeetTeams = MeetDB.createTeams(MeetDivisions);
        }

        public List<String> getEntriesForEvent(Event currentEvent) {
            List<String> eventEntries = new List<string>();
            List<Dictionary<int, int>> heatEntries = currentEvent.getHeatEntries();
            foreach (Dictionary<int, int> h in heatEntries) {
                for (int i = 1; i <= currentEvent.EventPositionCount; i++) {
                    if (h.ContainsKey(i)) {
                        Athlete a = findAthlete(h[i]);
                        eventEntries.Add(i + " " + a.AthleteFirstName + " " + a.AthleteLastName);
                    } else {
                        eventEntries.Add(i.ToString());
                    }
                }
                eventEntries.Add("");
            }
            if (eventEntries.Count > 0)
                eventEntries.RemoveAt(eventEntries.Count - 1);

            return eventEntries;
        }

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
            if (e.EventStatus != "Unseeded")
                foreach (Heat h in e.EventHeats)
                    if (!h.full()) {
                        for (int l = 1; l <= h.HeatEvent.EventPositionCount; l++)
                            if (!h.LaneAthletes.Keys.Contains(l)) {
                                h.LaneAthletes.Add(l, a.AthletePointer);
                                MeetDB.insertNewEntry(a, e, h, l);
                                break;
                            }
                        break;
                    }
        }

        public void removeAthleteFromEvent(Athlete a, Event e) {
            if (e.EventStatus != "Unseeded") {
                foreach (Heat h in e.EventHeats)
                    if (h.containsAthlete(a)) {
                        foreach (KeyValuePair<int, int> l in h.LaneAthletes)
                            if (l.Value == a.AthletePointer) {
                                h.LaneAthletes.Remove(l.Key);
                                MeetDB.removeEntry(a, e);
                                break;
                            }
                        break;
                    }
            }

        }

        public void close() {
            MeetDB.close();
        }
        
        public override string ToString() {
            return MeetName + " " + MeetDate;
        }
    }
}
