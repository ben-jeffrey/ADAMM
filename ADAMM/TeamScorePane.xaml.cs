using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ADAMM {
    /// <summary>
    /// Pane in TeamTab that passively displays team scores
    /// </summary>
    public partial class TeamScorePane : Page {
        public TeamScorePane() {
            InitializeComponent();
        }

        // Called by TeamTab on page load
        public void Setup(Division d, List<Team> teams) {
            // Populate listview with a special object that pairs teams with their scores given a division
            List<TeamScorePair> scoreList = new List<TeamScorePair>();
            foreach (Team t in teams)
                scoreList.Add(new TeamScorePair(t, d));

            teamScores.ItemsSource = scoreList;
        }

        // Called by TeamTab whenever it's loaded to refresh the team's scores
        public void Refresh() {
            // Refresh all of the scores
            foreach (TeamScorePair t in teamScores.Items)
                t.RefreshScore();
        }

        // Class to pair a team with a division to provide scores for that division
        // Created to work around WPF not wanting to update the team's score data
        private class TeamScorePair : INotifyPropertyChanged {
            // Propery changed notifyer to update score data on refresh
            public event PropertyChangedEventHandler PropertyChanged;
            public Team Team { get; set; }
            public Division Division { get; set; }
            public string Name { get { return Team.TeamLongName; } }
            public int Score { get { return Team.CalculateScoreByDivision(Division); } }

            public TeamScorePair(Team t, Division d) {
                Team = t;
                Division = d;
            }

            // Raises property changed event so the listview displays the newest scores
            public virtual void RefreshScore() {
                if (PropertyChanged != null)
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Score"));
            }
        }
    }
}
