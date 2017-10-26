using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAMM
{
    class Team {
        private static MeetDatabase MeetDB;
        public List<Athlete> TeamRoster { get; }
        public int TeamNumber { get; }
        public string longName { get; }
        private string shortName { get; set; }
        private string abbreviation { get; set; }

        public Team(int num, String lName, String sName, String abbr, MeetDatabase db) {
            TeamNumber = num;
            longName = lName;
            shortName = sName;
            abbreviation = abbr;
            MeetDB = db;
            TeamRoster = MeetDB.createTeamRoster(TeamNumber);
            foreach (Athlete a in TeamRoster)
                a.AthleteTeam = this;
        }

        public Athlete findAthlete(int athNum) {
            Athlete found = null;
            foreach (Athlete a in TeamRoster) {
                if (athNum == a.number) {
                    found = a;
                    break;
                }
            }
            return found;
        }

    }
}
