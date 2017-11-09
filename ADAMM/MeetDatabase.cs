using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;
using System.IO;
using System.IO.Compression;

namespace ADAMM {
    class MeetDatabase {
        private static OdbcConnection DB;
        private String originalFilePath;

        public MeetDatabase(string dbFilePath) {
            //File.Copy(dbFilePath, "db.mdb", true);
            ZipFile.ExtractToDirectory(dbFilePath, "/db");
            originalFilePath = dbFilePath;
            DB = new OdbcConnection();
            string connectionString = "Driver={Microsoft Access Driver (*.mdb)};Dbq=/db/db.mdb;Uid=Admin;Pwd=pX47P(sA_dfQ-r)-651V;";
            DB.ConnectionString = connectionString;
            DB.Open();
        }

        private void open() {
            /*try {
                DB.Open();
            } catch (Exception e) {
                Console.WriteLine(e.StackTrace);
            }*/
        }

        public void close() {
            DB.Close();
            File.Delete(originalFilePath);
            ZipFile.CreateFromDirectory("db", originalFilePath);
        }

        public String[] getMeetInfo() {
            OdbcCommand com = new OdbcCommand("SELECT Meet_name1, Meet_start FROM Meet");
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            r.Read();
            String[] info = new String[] { r.GetString(0), r.GetDate(1).ToString("M/d/yy")};
            return info;
        }
        public List<Event> createEvents(List<Division> divisions) {
            List<Event> events = new List<Event>();
            OdbcCommand com = new OdbcCommand("SELECT Event_no, Event_ptr, Event_gender, Trk_Field, Num_prelanes, Div_no FROM Event ORDER BY Event_no ASC");
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            while (r.Read()) {
                Division eventDivision = null;
                foreach (Division d in divisions)
                    if (d.DivisionNumber == r.GetInt32(5))
                        eventDivision = d;
                events.Add(new Event(r.GetInt16(0), r.GetInt32(1), r.GetString(2)[0], r.GetString(3)[0], r.GetInt16(4), eventDivision));
            }
            return events;
        }

        public List<Team> createTeams(List<Division> divisions) {
            List<Team> teams = new List<Team>();
            OdbcCommand com = new OdbcCommand("SELECT Team_no, Team_name, Team_abbr, Team_short FROM Team ORDER BY Team_name ASC");
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            while (r.Read()) {
                teams.Add(new Team(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetString(3), divisions));
            }
            return teams;
        }

        public List<Athlete> createTeamRoster(List<Division> divisions, int teamNum) {
            List<Athlete> athletes = new List<Athlete>();
            OdbcCommand com = new OdbcCommand("SELECT Comp_no, Ath_no, First_name, Last_name, Ath_Sex, Ath_age  FROM Athlete WHERE Team_no = " + teamNum);
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            while (r.Read()) {
                Division athleteDivision = null;
                Console.WriteLine(r.GetValue(5).GetType());
                foreach (Division d in divisions)
                    if (d.DivisionAgeLow <= r.GetInt16(5) && r.GetInt16(5) <= d.DivisionAgeHigh)
                        athleteDivision = d;
                athletes.Add(new Athlete(r.GetInt32(0), r.GetInt32(1), r.GetString(2), r.GetString(3), r.GetString(4)[0], athleteDivision));
            }
            return athletes;
        }

        public List<Division> createDivisions() {
            List<Division> divisions = new List<Division>();
            OdbcCommand com = new OdbcCommand("SELECT Div_no, Div_name, low_age, high_age FROM Divisions;");
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            while (r.Read()) {
                if (!r.GetValue(2).GetType().Equals(typeof(DBNull)))
                    divisions.Add(new Division(r.GetInt32(0), r.GetString(1), r.GetInt16(2), r.GetInt16(3)));
            }
            return divisions;
        }

        public List<int[]> getEntries(int eventPtr) {
            OdbcCommand com = new OdbcCommand("SELECT Ath_no, Fin_heat, Fin_lane FROM Entry WHERE Event_ptr = " + eventPtr + " ORDER BY Fin_heat ASC, Fin_lane ASC;");
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            List<int[]> entries = new List<int[]>();
            while (r.Read()) {
                entries.Add(new int[] { r.GetInt32(0), r.GetInt32(1), r.GetInt32(2) });
            }
            return entries;
        }

        public void updateAthleteRecord(Athlete ath) {
            OdbcCommand com = new OdbcCommand(
                String.Format("UPDATE Athlete SET First_name='{0}', Last_name='{1}', Ath_sex='{2}', Team_no={3} WHERE Ath_no = {4};",
                ath.AthleteFirstName, ath.AthleteLastName, ath.AthleteGender, ath.AthleteTeam.TeamNumber, ath.AthletePointer));
            com.Connection = DB;
            com.ExecuteNonQuery();
            finishUpdate();
        }
        public void updateTeamRecord() {

        }
        public void updateEventRecord() {

        }

        public void insertNewAthlete(Athlete ath) {
            OdbcCommand com = new OdbcCommand("SELECT MAX(Comp_no) FROM Athlete");
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            r.Read();
            ath.AthleteNumber = r.GetInt32(0) + 1;
            r.Close();

            com.CommandText = String.Format("INSERT INTO Athlete (First_name, Last_name, Ath_sex, Comp_no, Team_no) VALUES ('{0}', '{1}', '{2}', {3}, {4});",
                ath.AthleteFirstName, ath.AthleteLastName, ath.AthleteGender, ath.AthleteNumber, ath.AthleteTeam.TeamNumber);
            com.ExecuteNonQuery();

            com.CommandText = "SELECT Ath_no FROM Athlete WHERE Comp_no = " + ath.AthleteNumber + ";";
            r = com.ExecuteReader();
            r.Read();
            ath.AthletePointer = r.GetInt32(0);

            finishUpdate();
        }

        private void finishUpdate() {
            File.Delete(originalFilePath);
            ZipFile.CreateFromDirectory("db", originalFilePath);
        }
    }
}
