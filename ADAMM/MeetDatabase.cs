﻿using System;
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
            // Delete local DB if one exists
            File.Delete("db/db.mdb");
            // Extract DB from path to local space
            ZipFile.ExtractToDirectory(dbFilePath, "db");
            // Save original path
            originalFilePath = dbFilePath;

            // Create a connection to local DB
            DB = new OdbcConnection();
            string connectionString = "Driver={Microsoft Access Driver (*.mdb)};Dbq=db/db.mdb;Uid=Admin;Pwd=pX47P(sA_dfQ-r)-651V;";
            DB.ConnectionString = connectionString;
        }

        public void close() {
            DB.Close();
        }

        private void open() {
            if (DB.State != System.Data.ConnectionState.Open)
                DB.Open();
        }

        // ALL QUERY FUNCTIONS ARE OF THE FORM:
        /*
         open();
         OdbcCommand com = new OdbcCommand("SQL");
         com.Connection = DB;
         OdbcDataReader r = com.ExecuteReader();
         [read data]
         r.Close();
         DB.Close();
         [return data]
         */

        // ALL NON-QUERIES ARE OF THE FORM:
        /*
         open();
         OdbcCommand com = new OdbcCommand("SQL");
         com.Connection = DB;
         com.ExecuteNonQuery();
         DB.Close();
         finishUpdate();
         */

        public String[] getMeetInfo() {
            open();
            OdbcCommand com = new OdbcCommand("SELECT Meet_name1, Meet_start FROM Meet");
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            r.Read();
            String[] info = new String[] { r.GetString(0), r.GetDate(1).ToString("M/d/yy")};
            r.Close();
            DB.Close();
            return info;
        }

        public List<Event> createEvents(List<Division> divisions) {
            open();
            List<Event> events = new List<Event>();
            OdbcCommand com = new OdbcCommand("SELECT Event_no, Event_ptr, Event_gender, Num_prelanes, Div_no, Event_stat, Event_stroke, Event_dist, Res_meas, Trk_Field FROM Event ORDER BY Event_no ASC");
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            while (r.Read()) {
                // Add division to event as we read it
                Division eventDivision = null;
                foreach (Division d in divisions)
                    if (d.DivisionNumber == r.GetInt32(4))
                        eventDivision = d;
                if (r.GetChar(9) == 'T') {
                    events.Add(new RunningEvent(r.GetInt16(0), r.GetInt32(1), r.GetString(2)[0], r.GetInt16(3), eventDivision, r.GetString(5)[0], r.GetString(6)[0], (int)r.GetFloat(7), r.GetString(8)[0]));
                } else
                    events.Add(new FieldEvent(r.GetInt16(0), r.GetInt32(1), r.GetString(2)[0], eventDivision, r.GetString(5)[0], r.GetString(6)[0], r.GetString(8)[0]));
            }
            r.Close();
            // Populate events with entries
            foreach (Event e in events) {
                e.populate();
            }
            DB.Close();
            return events;
        }

        public List<Team> createTeams(List<Division> divisions) {
            open();
            List<Team> teams = new List<Team>();
            OdbcCommand com = new OdbcCommand("SELECT Team_no, Team_name, team_short, Team_abbr, Team_city, Team_state, Team_zip, Team_cntry FROM Team ORDER BY Team_name ASC");
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            while (r.Read()) {
                // Use empty strings instead of nulls for some attributes that may not exist
                teams.Add(new Team(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetString(3),
                    r.IsDBNull(4) ? "" : r.GetString(4),
                    r.IsDBNull(5) ? "" : r.GetString(5),
                    r.IsDBNull(6) ? "" : r.GetString(6),
                    r.IsDBNull(7) ? "" : r.GetString(7)));
            }
            // Fill each team with athletes and score templates
            foreach (Team t in teams)
                t.populate(divisions);
            DB.Close();
            return teams;
        }

        public List<Athlete> createTeamRoster(List<Division> divisions, int teamNum) {
            open();
            List<Athlete> athletes = new List<Athlete>();
            OdbcCommand com = new OdbcCommand("SELECT Comp_no, Ath_no, First_name, Last_name, Ath_Sex, Ath_age  FROM Athlete WHERE Team_no = " + teamNum);
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            while (r.Read()) {
                // Set athlete's division based on age
                Division athleteDivision = null;
                foreach (Division d in divisions)
                    if (d.DivisionAgeLow <= r.GetInt16(5) && r.GetInt16(5) <= d.DivisionAgeHigh)
                        athleteDivision = d;
                athletes.Add(new Athlete(r.GetInt32(0), r.GetInt32(1), r.GetString(2), r.GetString(3), r.GetString(4)[0], athleteDivision));
            }
            r.Close();
            //DB.Close();
            return athletes;
        }

        public List<Division> createDivisions() {
            open();
            List<Division> divisions = new List<Division>();
            divisions.Add(new Division(0, "Default", 0, 0));
            OdbcCommand com = new OdbcCommand("SELECT Div_no, Div_name, low_age, high_age FROM Divisions;");
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            while (r.Read())
                // Division may or may not have age constraints
                if (!r.GetValue(2).GetType().Equals(typeof(DBNull)))
                    divisions.Add(new Division(r.GetInt32(0), r.GetString(1), r.GetInt16(2), r.GetInt16(3)));
                // Don't add nameless divisions
                else if (r.GetString(1) != " ")
                    divisions.Add(new Division(r.GetInt32(0), r.GetString(1), 0, 0));
            r.Close();

            // Get the score template for each division once all have been made
            foreach (Division d in divisions)
                getScoreTemplate(d);

            DB.Close();
            return divisions;
        }

        private void getScoreTemplate(Division d) {
            open();
            List<Division> divisions = new List<Division>();
            OdbcCommand com = new OdbcCommand("SELECT score_place, ind_score FROM Scoring WHERE score_sex='M' AND (score_divno=0 OR score_divno=" + d.DivisionNumber + ");");
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            while (r.Read()) {
                d.DivisionScoreTemplate.Add(r.GetInt32(0), r.GetInt32(1));
            }
            r.Close();
        }

        public List<Entry> getEntries(Event e) {
            open();
            OdbcCommand com = new OdbcCommand("SELECT Fin_lane, Fin_heat, Ath_no, ActualSeed_time, Fin_Time, Fin_stat FROM Entry WHERE Event_ptr = " + e.EventPointer + " ORDER BY Fin_heat ASC, Fin_lane ASC;");
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            List<Entry> entries = new List<Entry>();
            while (r.Read()) {
                Entry newEntry = new Entry(r.GetInt32(0), r.GetInt32(1), r.GetInt32(2), e);
                // Times and statuses may or may not exist, check before each
                if (!r.GetValue(3).GetType().Equals(typeof(DBNull)))
                    newEntry.EntrySeedMark = r.GetFloat(3);
                else newEntry.EntrySeedMark = 0;
                if (!r.GetValue(4).GetType().Equals(typeof(DBNull)))
                    newEntry.EntryMark = r.GetFloat(4);
                else newEntry.EntryMark = 0;
                if (!r.GetValue(5).GetType().Equals(typeof(DBNull)))
                    newEntry.EntryStatus = r.GetString(5)[0];
                else newEntry.EntryStatus = ' ';
                entries.Add(newEntry);
            }
            r.Close();
            // Closing here slow the program down quite a bit, interestingly enough
            //DB.Close();
            return entries;
        }

        public List<Entry> getUnseededEntries(Event e) {
            List<Entry> entries = new List<Entry>();
            // If the event is seeded, theres no need to read the database
            if (e.isSeeded()) {
                foreach (Heat h in e.EventHeats)
                    foreach (Entry ent in h.HeatEntries)
                        entries.Add(ent);
            }
            else {
                open();
                OdbcCommand com = new OdbcCommand("SELECT Ath_no, ActualSeed_time FROM Entry WHERE Event_ptr = " + e.EventPointer + ";");
                com.Connection = DB;
                OdbcDataReader r = com.ExecuteReader();
                while (r.Read()) {
                    Entry newEntry = new Entry(0, 0, r.GetInt32(0), e);
                    // Seed times may not exist
                    if (!r.GetValue(1).GetType().Equals(typeof(DBNull)))
                        newEntry.EntrySeedMark = r.GetFloat(1);
                    entries.Add(newEntry);
                }
                r.Close();
                // Closing here ALSO slow the program down quite a bit
                //DB.Close();
            }
            return entries;
        }

        public void updateAthleteRecord(Athlete ath) {
            open();
            OdbcCommand com = new OdbcCommand(
                String.Format("UPDATE Athlete SET First_name='{0}', Last_name='{1}', Ath_sex='{2}', Team_no={3} WHERE Ath_no = {4};",
                ath.AthleteFirstName, ath.AthleteLastName, ath.AthleteGender, ath.AthleteTeam.TeamNumber, ath.AthletePointer));
            com.Connection = DB;
            com.ExecuteNonQuery();
            DB.Close();
            finishUpdate();
        }

        public void updateTeamRecord(Team t) {
            open();
            OdbcCommand com = new OdbcCommand(
                String.Format("UPDATE Team SET Team_name='{0}', team_short='{1}', Team_abbr='{2}', Team_city='{3}', Team_state='{4}', Team_zip='{5}', Team_cntry='{6}' WHERE Team_no = {7};",
                t.TeamLongName, t.TeamShortName, t.TeamAbbrev, t.TeamCity, t.TeamState, t.TeamZip, t.TeamCountry, t.TeamNumber));
            com.Connection = DB;
            com.ExecuteNonQuery();
            DB.Close();
            finishUpdate();
        }

        public void updateEntryRecord(Entry e) {
            open();
            OdbcCommand com = new OdbcCommand(
                String.Format("UPDATE Entry SET ActualSeed_time={0}, Fin_heat={1}, Fin_lane={2} WHERE Event_ptr={3} AND Ath_no={4};",
                e.EntrySeedMark, e.EntryHeat, e.EntryPosition, e.EntryEvent.EventPointer, e.EntryAthletePointer));
            com.Connection = DB;
            com.ExecuteNonQuery();
            DB.Close();
            finishUpdate();
        }

        public void updateEntryResult(Entry e) {
            open();
            OdbcCommand com = new OdbcCommand(
                String.Format("UPDATE Entry SET Fin_Time={0}, Fin_stat='{1}' WHERE Event_ptr={2} AND Ath_no={3};",
                e.EntryMark, e.EntryStatus, e.EntryEvent.EventPointer, e.EntryAthletePointer));
            com.Connection = DB;
            com.ExecuteNonQuery();
            DB.Close();
            finishUpdate();
        }

        public void insertNewAthlete(Athlete ath) {
            open();
            // Get next highest number
            OdbcCommand com = new OdbcCommand("SELECT MAX(Comp_no) FROM Athlete");
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            r.Read();
            // Make new highest athlete number
            ath.AthleteNumber = r.GetInt32(0) + 1;
            r.Close();

            com.CommandText = String.Format("INSERT INTO Athlete (First_name, Last_name, Ath_sex, Comp_no, Team_no) VALUES ('{0}', '{1}', '{2}', {3}, {4});",
                ath.AthleteFirstName, ath.AthleteLastName, ath.AthleteGender, ath.AthleteNumber, ath.AthleteTeam.TeamNumber);
            com.ExecuteNonQuery();

            // Go back and get the primary key
            com.CommandText = "SELECT Ath_no FROM Athlete WHERE Comp_no = " + ath.AthleteNumber + ";";
            r = com.ExecuteReader();
            r.Read();
            ath.AthletePointer = r.GetInt32(0);

            DB.Close();
            finishUpdate();
        }

        public void insertNewEntry(Athlete ath, Event ev, Heat he, int lane) {
            open();
            OdbcCommand com = new OdbcCommand(String.Format("INSERT INTO Entry (Event_ptr, Ath_no, ActSeed_course, ConvSeed_course, Hand_time, Fin_heat, Fin_lane)" +
                "VALUES ({0}, {1}, '{2}', '{2}', FALSE, {3}, {4})",
                ev.EventPointer, ath.AthletePointer, ev.EventUnit, he.HeatNumber, lane));
            com.Connection = DB;
            com.ExecuteNonQuery();
            DB.Close();
            finishUpdate();
        }

        public void insertNewEntry(Entry e) {
            open();
            OdbcCommand com = new OdbcCommand(String.Format("INSERT INTO Entry (Event_ptr, Ath_no, ActSeed_course, ConvSeed_course, Hand_time, Fin_heat, Fin_lane)" +
                "VALUES ({0}, {1}, '{2}', '{2}', FALSE, {3}, {4})",
                e.EntryEvent.EventPointer, e.EntryAthletePointer, e.EntryEvent.EventUnit, e.EntryHeat, e.EntryPosition));
            com.Connection = DB;
            com.ExecuteNonQuery();
            DB.Close();
            finishUpdate();
        }

        public void insertNewEvent(Event e) {
            open();
            // Set character type field for DB
            char Trk_Field = e.GetType() == typeof(FieldEvent) ? 'F' : 'T';
            OdbcCommand com = new OdbcCommand(String.Format("INSERT INTO Event (Event_no, Event_gender, Event_dist, Event_stroke, Event_stat, Trk_Field, Res_Meas, Num_prelanes, Div_no)" +
                "VALUES ({0}, '{1}', {2}, '{3}', '{4}', '{5}', '{6}', {7}, {8});",
                e.EventNumber, e.EventGender, e.EventDistance, Event.CategoryChars[e.EventCategory], e.EventStatus, Trk_Field, e.EventUnit, e.EventPositionCount, e.EventDivision.DivisionNumber));
            com.Connection = DB;
            com.ExecuteNonQuery();

            // Go back and get the primary key
            com = new OdbcCommand("SELECT Event_ptr FROM Event WHERE Event_no = " + e.EventNumber + ";");
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            r.Read();
            e.EventPointer = r.GetInt32(0);
            DB.Close();
            finishUpdate();
        }

        public void removeEntry(Athlete ath, Event ev) {
            open();
            OdbcCommand com = new OdbcCommand(String.Format("DELETE FROM Entry WHERE Event_ptr={0} AND Ath_no={1}", ev.EventPointer, ath.AthletePointer));
            com.Connection = DB;
            com.ExecuteNonQuery();
            DB.Close();
            finishUpdate();
        }

        public void removeEntry(Entry e) {
            open();
            OdbcCommand com = new OdbcCommand(String.Format("DELETE FROM Entry WHERE Event_ptr={0} AND Ath_no={1}", e.EntryEvent.EventPointer, e.EntryAthletePointer));
            com.Connection = DB;
            com.ExecuteNonQuery();
            DB.Close();
            finishUpdate();
        }

        // Writes the local database copy out to the original
        // Done this way to circumnavigate Windows permission errors
        private void finishUpdate() {
            File.Copy("db/db.mdb", "db/tmp/db.mdb");
            File.Delete(originalFilePath);
            ZipFile.CreateFromDirectory("db/tmp", originalFilePath);
            File.Delete("db/tmp/db.mdb");
        }
    }
}
