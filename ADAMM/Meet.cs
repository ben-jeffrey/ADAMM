﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;

namespace ADAMM
{
    class Meet {
        public MeetDatabase MeetDB { get; }
        private String MeetName;
        private String MeetDate;
        public List<Event> MeetEvents { get; set; }
        public List<Team> MeetTeams { get; set; }

        public Meet(string dbFilePath) {
            MeetDB = new MeetDatabase(dbFilePath);
            String[] meetInfo = MeetDB.getMeetInfo();
            MeetName = meetInfo[0];
            MeetDate = meetInfo[1];

            Athlete.MeetDB = MeetDB;
            Event.MeetDB = MeetDB;
            Team.MeetDB = MeetDB;
            Heat.MeetDB = MeetDB;

            MeetEvents = MeetDB.createEvents();
            MeetTeams = MeetDB.createTeams();
        }

        public List<String> getEntriesForEvent(int eventNum) {
            List<String> eventEntries = new List<string>();
            Event currentEvent = null;
            foreach (Event e in MeetEvents)
                if (e.EventNumber == eventNum)
                    currentEvent = e;

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
                if (e.isEligible(a) && e.containsAthlete(a))
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
            Athlete newAthlete = new Athlete(0, 0, "", "", 'M');

            newAthlete.AthleteTeam = MeetTeams[0];
            newAthlete.AthleteTeam.TeamRoster.Add(newAthlete);

            MeetDB.insertNewAthlete(newAthlete);

            return newAthlete;
        }

        public void close() {
            MeetDB.close();
        }
        
        public override string ToString() {
            return MeetName + " " + MeetDate;
        }
    }
}
