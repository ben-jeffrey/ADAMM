using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace ADAMM {
    class ScheduleFile {
        private string ScheduleDirectory;

        public ScheduleFile(string path) {
            ScheduleDirectory = path;
        }

        // Creates csv of known format that contains the order of events
        // Used by Finishlynx software to get event numbers
        public void CreateSchFile (List<Event> events) {
            using (StreamWriter file =  new StreamWriter(ScheduleDirectory + "\\lynx.sch")) {
                foreach (Event e in events)
                    if (e is RunningEvent)
                        foreach (Heat h in e.EventHeats)
                            // Each line is of the format "EventNumber, RoundNumber, HeatNumber"
                            // RoundNumber is always 1 for now
                            file.WriteLine(String.Format("{0},1,{1}", e.EventNumber, h.HeatNumber));
            }
        }

        // Create csv of known format that contains the athletes in the meet
        public void CreatePplFile(Meet m) {
            using (StreamWriter file = new StreamWriter(ScheduleDirectory + "\\lynx.ppl")) {
                // Dict to keep track of athlete's event entries
                Dictionary<int, string> athleteMappings = new Dictionary<int, string>();

                // Create a string of entries for each athlete
                // Of the form: "EventNumber, EventNumber, . . ."
                foreach (Event e in m.MeetEvents)
                    foreach (Heat h in e.EventHeats)
                        foreach (Entry ent in h.HeatEntries) {
                            if (athleteMappings.ContainsKey(ent.EntryAthlete.AthleteNumber))
                                athleteMappings[ent.EntryAthlete.AthleteNumber] += String.Format(",{0}", e.EventNumber);
                            else
                                athleteMappings.Add(ent.EntryAthlete.AthleteNumber, String.Format("\"{0}", e.EventNumber));
                        }

                // Add each athlete's data to a line of ppl file
                foreach (Team t in m.MeetTeams)
                    foreach (Athlete a in t.TeamRoster)
                        try {
                            file.WriteLine(String.Format("{0},{1},{2},,{3},{4}\"", a.AthleteLastName, a.AthleteFirstName, t.TeamLongName, a.AthleteGender, athleteMappings[a.AthleteNumber]));
                        }  catch (KeyNotFoundException e) { }
            }
        }

        // Create csv of known format that contains all events and all entries in those events
        public void CreateEvtFile(List<Event> events) {
            using (StreamWriter file = new StreamWriter(ScheduleDirectory + "\\lynx.evt")) {
                foreach (Event e in events)
                    if (e is RunningEvent)
                        foreach (Heat h in e.EventHeats) {
                            // Format of EVT file:
                            // EventNumber, HeatNumber, EventName, , , , , , EventDistance
                            // , AthleteNumber, Position, LastName, FirstName, TeamName
                            file.WriteLine(String.Format("{0},1,{1},{2},,,,,,{3}", e.EventNumber, h.HeatNumber, e.EventName, e.EventDistance));
                            foreach (Entry ent in h.HeatEntries)
                                file.WriteLine(String.Format(",{0},{1},{2},{3},{4}", ent.EntryAthlete.AthleteNumber, ent.EntryPosition, ent.EntryAthlete.AthleteLastName, ent.EntryAthlete.AthleteFirstName, ent.EntryAthlete.AthleteTeam.TeamLongName));
                        }
            }
        }
    }   
}
