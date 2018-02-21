using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;
using System.IO;
using System.IO.Compression;

namespace ADAMM {
    public class MeetDatabase {
        private static OdbcConnection DB;
        private String originalFilePath;

        public MeetDatabase(string dbFilePath) {
            //pX47P(sA_dfQ-r)-651V
            ZipFile.ExtractToDirectory(dbFilePath, "db");
            originalFilePath = dbFilePath;
            DB = new OdbcConnection();
            string connectionString = "Driver={Microsoft Access Driver (*.mdb)};Dbq=db/db.mdb;Uid=Admin;Pwd=pX47P(sA_dfQ-r)-651V;";
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
            finishUpdate();
            File.Delete("db/db.mdb");
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
            OdbcCommand com = new OdbcCommand("SELECT Event_no, Event_ptr, Event_gender, Num_prelanes, Div_no, Event_stat, Event_stroke, Event_dist, Res_meas, Trk_Field FROM Event ORDER BY Event_no ASC");
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            while (r.Read()) {
                Division eventDivision = null;
                foreach (Division d in divisions)
                    if (d.DivisionNumber == r.GetInt32(4))
                        eventDivision = d;
                if (r.GetChar(9) == 'T') {
                    //Console.WriteLine(r.GetInt16(3));
                    events.Add(new RunningEvent(r.GetInt16(0), r.GetInt32(1), r.GetString(2)[0], r.GetInt16(3), eventDivision, r.GetString(5)[0], r.GetString(6)[0], (int)r.GetFloat(7), r.GetString(8)[0]));
                } else
                    events.Add(new FieldEvent(r.GetInt16(0), r.GetInt32(1), r.GetString(2)[0], eventDivision, r.GetString(5)[0], r.GetString(6)[0], r.GetString(8)[0]));
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

        public List<Entry> getEntries(Event e) {
            OdbcCommand com = new OdbcCommand("SELECT Fin_lane, Fin_heat, Ath_no FROM Entry WHERE Event_ptr = " + e.EventPointer + " ORDER BY Fin_heat ASC, Fin_lane ASC;");
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            List<Entry> entries = new List<Entry>();
            while (r.Read()) {
                entries.Add(new Entry(r.GetInt32(0), r.GetInt32(1), r.GetInt32(2), e));
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

        public void insertNewEntry(Athlete ath, Event ev, Heat he, int lane) {
            OdbcCommand com = new OdbcCommand(String.Format("INSERT INTO Entry (Event_ptr, Ath_no, ActSeed_course, ConvSeed_course, Hand_time, Fin_heat, Fin_lane)" +
                "VALUES ({0}, {1}, '{2}', '{2}', FALSE, {3}, {4})",
                ev.EventPointer, ath.AthletePointer, ev.EventUnit, he.HeatNumber, lane));
            com.Connection = DB;
            com.ExecuteNonQuery();
            finishUpdate();
        }

        public void insertNewEvent(Event e) {
            char Trk_Field = e.GetType() == typeof(FieldEvent) ? 'F' : 'T';
            OdbcCommand com = new OdbcCommand(String.Format("INSERT INTO Event (Event_no, Event_gender, Event_dist, Event_stroke, Event_stat, Trk_Field, Res_Meas, Num_prelanes, Div_no)" +
                "VALUES ({0}, '{1}', {2}, '{3}', '{4}', '{5}', '{6}', {7}, {8});",
                e.EventNumber, e.EventGender, e.EventDistance, Event.CategoryChars[e.EventCategory], e.EventStatus, Trk_Field, e.EventUnit, e.EventPositionCount, e.EventDivision.DivisionNumber));
            com.Connection = DB;
            com.ExecuteNonQuery();

            com = new OdbcCommand("SELECT Event_ptr FROM Event WHERE Event_no = " + e.EventNumber + ";");
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            r.Read();
            e.EventPointer = r.GetInt32(0);
            finishUpdate();
        }

        public void removeEntry(Athlete ath, Event ev) {
            OdbcCommand com = new OdbcCommand(String.Format("DELETE FROM Entry WHERE Event_ptr={0} AND Ath_no={1}", ev.EventPointer, ath.AthletePointer));
            com.Connection = DB;
            com.ExecuteNonQuery();
            finishUpdate();
        }

        private void finishUpdate() {
            File.Copy("db/db.mdb", "db/tmp/db.mdb");
            File.Delete(originalFilePath);
            ZipFile.CreateFromDirectory("db/tmp", originalFilePath);
            File.Delete("db/tmp/db.mdb");
        }
    }
}
