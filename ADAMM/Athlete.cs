using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAMM
{
    class Athlete {
        private static MeetDatabase MeetDB;
        public int number { get; set; }
        public Team AthleteTeam { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public char AthleteGender { get; set; }

        public Athlete(int ath, String fname, String lname, Char gender, MeetDatabase db) {
            number = ath;
            firstName = fname.Trim();
            lastName = lname.Trim();
            AthleteGender = gender;
            MeetDB = db;
        }

        public void updateRecord() {
            MeetDB.updateAthleteRecord(this);
        }

        public override string ToString() {
            return number + " " + AthleteTeam.TeamNumber + " " + firstName + " " + lastName;
        }
    }
}
