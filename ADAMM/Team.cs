using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ADAMM
{
    public class Team : INotifyPropertyChanged {
        public static MeetDatabase MeetDB;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<Athlete> TeamRoster { get; set; }
        public int TeamNumber { get; }
        public string TeamLongName { get; set; }
        public string TeamShortName { get; set; }
        public string TeamAbbrev { get; set; }
        public string TeamCity { get; set; }
        public string TeamState { get; set; }
        public string TeamZip { get; set; }
        public string TeamCountry { get; set; }
        // Dict keyed by each division
        //    that's value is another dictionary keyed by events
        //      that's value is the team's score in that event
        public Dictionary<Division, Dictionary<Event, int>>  TeamScoreByDivision { get; }

        public Team(int num, String lName, String sName, String abbr, String city, String state, String zip, String country) {
            TeamNumber = num;
            TeamLongName = lName;
            TeamShortName = sName;
            TeamAbbrev = abbr;
            TeamCity = city;
            TeamState = state;
            TeamZip = zip;
            TeamCountry = country;
            TeamScoreByDivision = new Dictionary<Division, Dictionary<Event, int>>();
        }

        // Populate team with athletes and also grab the divisions for scoring
        public void populate(List<Division> divisions) {
            TeamRoster = MeetDB.createTeamRoster(divisions, TeamNumber);
            foreach (Athlete a in TeamRoster)
                a.AthleteTeam = this;

            foreach (Division d in divisions)
                TeamScoreByDivision.Add(d, new Dictionary<Event, int>());
        }

        // Return athlete reference with given number
        // null if none found
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

        // Calculates total score in the given division
        public int CalculateScoreByDivision(Division div) {
            int totalScore = 0;
            
            // Sum score of the values for the dict of divsion 'div'
            foreach (KeyValuePair<Event, int> eventScore in TeamScoreByDivision[div])
                totalScore += TeamScoreByDivision[div][eventScore.Key];

            return totalScore;
        }
    }
}
