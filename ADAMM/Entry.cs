using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ADAMM {
    public class Entry : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        public int EntryPosition { get; set; }
        public int EntryHeat { get; set; }
        public int EntryAthletePointer { get; set; }
        public Athlete EntryAthlete { get; set; }
        public Event EntryEvent { get; set; }
        // Marks are stored in seconds, inches, or centimeters
        public double EntrySeedMark { get; set; }
        public string EntryDisplaySeedMark { get { return displayify(EntrySeedMark); } }
        public double EntryMark { get; set; }
        // ' ' status means there is no status
        public string EntryDisplayMark { get { if (EntryStatus == ' ') return displayify(EntryMark); else return statusString(EntryStatus); } }
        public char EntryStatus { get; set; }


        public Entry(int pos, int h, int a, Event e) {
            EntryPosition = pos;
            EntryHeat = h;
            EntryAthletePointer = a;
            // Athlete reference starts as null and is propogated ins a later step of initialization
            EntryAthlete = null;
            EntryEvent = e;
        }

        // Convert the integer mark into a form that is human-readable
        private string displayify(double mark) {
            // Mark is in seconds for running events
            if (EntryEvent is RunningEvent) {
                TimeSpan time = new TimeSpan(0, 0, 0, (int)mark, (int)(mark * 1000 - (int)mark * 1000));
                if (time.TotalMinutes >= 1)
                    return time.ToString(@"m\:ss\.ff");
                else
                    return time.ToString(@"ss\.ff");
            // Only handles inches so far
            } else {
                if (EntryEvent.EventUnit == 'E')
                    // If no fractional inches
                    if (mark == (int)mark)
                        // Format ft-in
                        return String.Format("{0}-{1}", (int)mark / 12, (int)mark % 12);
                    // If fractional inches
                    else
                        // Format ft-in.frac
                        return String.Format("{0}-{1:0.00}", (int)mark / 12, (int)mark % 12 + (mark - (int)mark));
                else
                    return mark.ToString();
            }
        }

        // Switch function for making status chars readable
        private string statusString(char status) {
            switch (status) {
                case 'S': return "DNS";
                case 'Q': return "DQ";
                case 'R': return "SCR";
                case 'N': return "NH";
                case 'F': return "FOUL";
            }
            return "";
        }

        // Switch function to convert back to status chars
        private char statusChar(string status) {
            switch (status) {
                case "DNS": return 'S';
                case "DQ": return 'Q';
                case "SCR": return 'R';
                case "NH": return 'N';
                case "FOUL": return 'F';
            }
            return ' ';
        }

        // Parse an input string into result data
        public void ParseResultInput(string input) {
            // If nothing, do nothing
            if (input == "") return;

            // If length 1, check for status char
            if (input.Length == 1)
                if (statusString(input[0]) != "") {
                    EntryStatus = input[0];
                    return;
                }

            // Check for status word
            if (statusChar(input.ToUpper()) != ' ') {
                EntryStatus = statusChar(input.ToUpper());
                return;
            }

            // If we're here, assume no status
            EntryStatus = ' ';

            // If string should be in time
            if (EntryEvent is RunningEvent) {
                // Manually split string into time components
                string[] splitCentiseconds = input.Split('.');
                int centiseconds = int.Parse(splitCentiseconds.Last());
                for (int i = splitCentiseconds.Last().Length; i < 3; i++) centiseconds *= 10;

                string[] splitSeconds = splitCentiseconds[0].Split(':');
                int seconds = int.Parse(splitSeconds.Last());

                int minutes = 0;
                if (splitSeconds.Length > 1)
                    minutes = int.Parse(splitSeconds[1]);

                // Construct timespan with components
                TimeSpan time = new TimeSpan(0, 0, minutes, seconds, centiseconds);
                // Write seconds value to mark
                EntryMark = time.TotalSeconds;
            } else {
                // Manually parse inches and feet
                string[] splitInches = input.Split('-');
                double inches = double.Parse(splitInches.Last());
                int feet = splitInches.Length > 1 ? int.Parse(splitInches[0]) : 0;

                // Write total inches to mark
                EntryMark = 12 * feet + inches;
            }
        }

        // Raise property changed event to properly update UI
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            if (this.PropertyChanged != null)
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
