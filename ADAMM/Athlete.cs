﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ADAMM
{
    public class Athlete : INotifyPropertyChanged {
        public static MeetDatabase MeetDB;
        public event PropertyChangedEventHandler PropertyChanged;

        public int AthleteNumber { get { return number; } set { number = value; OnPropertyChanged(); } }
        private int number;
        public int AthletePointer { get { return pointer;} set { pointer = value; OnPropertyChanged(); } }
        private int pointer;
        public Team AthleteTeam { get { return team; } set { team = value; OnPropertyChanged(); } }
        private Team team;
        public string AthleteFirstName { get { return firstName; } set { firstName = value; OnPropertyChanged(); } }
        private string firstName;
        public string AthleteLastName { get { return lastName; } set { lastName = value; OnPropertyChanged(); } }
        private string lastName;
        public string AthleteFullName { get { return lastName + ", " + firstName; } }
        public char AthleteGender { get { return gender; } set { gender = value; OnPropertyChanged(); } }
        private char gender;
        public Division AthleteDivision { get { return division; } set { division = value; OnPropertyChanged(); } }
        private Division division;

        public Athlete(int comp, int ath, String fname, String lname, Char sex, Division div) {
            number = comp;
            pointer = ath;
            firstName = fname.Trim();
            lastName = lname.Trim();
            gender = sex;
            division = div;
            team = null;
        }

        public void updateRecord() {
            MeetDB.updateAthleteRecord(this);
        }

        public bool filter(String filter) {
            return firstName.ToLower().Contains(filter.ToLower()) || lastName.ToLower().Contains(filter.ToLower());
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            if (this.PropertyChanged != null)
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString() {
            return number + " " + AthleteTeam.TeamNumber + " " + firstName + " " + lastName;
        }
    }
}
