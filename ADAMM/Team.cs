using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAMM
{
    public class Team {
        public static MeetDatabase MeetDB;
        public List<Athlete> TeamRoster { get; set; }
        public int TeamNumber { get; }
        public string TeamLongName { get; set; }
        public string TeamShortName { get; set; }
        public string TeamAbbrev { get; set; }
        public string TeamCity { get; set; }
        public string TeamState { get; set; }
        public string TeamZip { get; set; }
        public string TeamCountry { get; set; }

        public Team(int num, String lName, String sName, String abbr, String city, String state, String zip, String country) {
            TeamNumber = num;
            TeamLongName = lName;
            TeamShortName = sName;
            TeamAbbrev = abbr;
            TeamCity = city;
            TeamState = state;
            TeamZip = zip;
            TeamCountry = country;
        }

        public void populate(List<Division> divisions) {
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
