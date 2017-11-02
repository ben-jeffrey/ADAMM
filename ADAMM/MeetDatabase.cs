﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;
using System.IO;

namespace ADAMM {
    class MeetDatabase {
        private static OdbcConnection DB;
        private String originalFilePath;

        public MeetDatabase(string dbFilePath) {
            File.Copy(dbFilePath, "db.mdb", true);
            originalFilePath = dbFilePath;
            DB = new OdbcConnection();
            string connectionString = "Driver={Microsoft Access Driver (*.mdb)};Dbq=db.mdb;Uid=Admin;Pwd=pX47P(sA_dfQ-r)-651V;ExtendedAnsiSQL=1;";
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
            //File.Copy("db.mdb", originalFilePath, true);
        }

        public String[] getMeetInfo() {
            OdbcCommand com = new OdbcCommand("SELECT Meet_name1, Meet_start FROM Meet");
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            r.Read();
            String[] info = new String[] { r.GetString(0), r.GetDate(1).ToString("M/d/yy")};
            return info;
        }
        public List<Event> createEvents() {
            List<Event> events = new List<Event>();
            OdbcCommand com = new OdbcCommand("SELECT Event_no, Event_ptr, Event_gender, Trk_Field, Num_prelanes FROM Event ORDER BY Event_no ASC");
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            while (r.Read()) {
                events.Add(new Event(r.GetInt32(0), r.GetInt32(1), r.GetChar(2), r.GetChar(3), r.GetInt32(4), this));
            }
            return events;
        }

        public List<Team> createTeams() {
            List<Team> teams = new List<Team>();
            OdbcCommand com = new OdbcCommand("SELECT Team_no, Team_name, Team_abbr, Team_short FROM Team ORDER BY Team_name ASC");
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            while (r.Read()) {
                teams.Add(new Team(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetString(3), this));
            }
            return teams;
        }

        public List<Athlete> createTeamRoster(int teamNum) {
            List<Athlete> athletes = new List<Athlete>();
            OdbcCommand com = new OdbcCommand("SELECT Ath_no, First_name, Last_name, Ath_Sex  FROM Athlete WHERE Team_no = " + teamNum);
            com.Connection = DB;
            OdbcDataReader r = com.ExecuteReader();
            while (r.Read()) {
                athletes.Add(new Athlete(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetChar(3), this));
            }
            return athletes;
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
                ath.firstName, ath.lastName, ath.AthleteGender, ath.AthleteTeam.TeamNumber, ath.number));
            com.Connection = DB;
            com.ExecuteNonQuery();
        }
        public void updateTeamRecord() {

        }
        public void updateEventRecord() {

        }
    }
}
