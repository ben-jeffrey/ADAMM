using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAMM {
    class RunningEvent : Event {

        // No changes to the base ctor
        public RunningEvent(int number, int ptr, char gender, int posCount, Division div, char stat, char cat, int dist, char unit) : base(number, ptr, gender, posCount, div, stat, cat, dist, unit) {

        }

        // Provides scores to teams for this event
        public override void Score() {
            // Get the score template for this division
            Dictionary<int, int> pointsByPlace = EventDivision.DivisionScoreTemplate;
            // Prepare lists for teams and ordered entries
            List<Team> teamsInEvent = new List<Team>();
            List<Entry> finalEntries = new List<Entry>();

            // Populate lists with entries and teams that participated in this event
            foreach (Heat h in EventHeats)
                foreach (Entry e in h.HeatEntries) {
                    finalEntries.Add(e);
                    if (!teamsInEvent.Contains(e.EntryAthlete.AthleteTeam))
                        teamsInEvent.Add(e.EntryAthlete.AthleteTeam);
                }

            // For each team
            foreach (Team t in teamsInEvent)
                // If a record of this event's score exists on that team, set it to 0
                if (t.TeamScoreByDivision[EventDivision].ContainsKey(this))
                    t.TeamScoreByDivision[EventDivision][this] = 0;
                // If no record exists, add one set to 0
                else
                    t.TeamScoreByDivision[EventDivision].Add(this, 0);

            // Sort ascending because faster is better in running events
            finalEntries.OrderBy(e => e.EntryMark);

            // For each entry, get the place from the index, find the score, and update the record in the relevant team
            for (int i = 0; i < finalEntries.Count; i++)
                finalEntries[i].EntryAthlete.AthleteTeam.TeamScoreByDivision[EventDivision][this] += pointsByPlace[i + 1];

            // Set event to 'scored'
            EventStatus = "S";
        }
    }
}
