using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAMM
{
    public class Team {
        public static MeetDatabase MeetDB;
        public List<Athlete> TeamRoster { get; }
        public int TeamNumber { get; }
        public string longName { get; }
        public string shortName { get; set; }
        private string abbreviation { get; set; }

        public Team(int num, String lName, String sName, String abbr, List<Division> divisions) {
            TeamNumber = num;
            longName = lName;
            shortName = sName;
            abbreviation = abbr;
            TeamRoster = MeetDB.createTeamRoster(divisions, TeamNumber);
            foreach (Athlete a in TeamRoster)
                a.AthleteTeam = this;
        }

        public Athlete findAthlete(int athNum) {
            Athlete found = null;
            foreach (Athlete a in TeamRoster) {
                if (athNum == a.AthletePointer) {
                    found = a;
                    break;
                }
            }
            return found;
        }

    }
}
