using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Submenu of the top-level window meant for creating and editing teams
    /// </summary>
    public partial class TeamTab : Page {

        Meet m;

        public TeamTab() {
            InitializeComponent();
        }

        // Called when the page loads by the main window
        public void SetUpMeet(Meet meet) {
            m = meet;

            // Populate team list
            foreach (Team t in m.MeetTeams)
                teamList.Items.Add(t);

            // Select the first team
            teamList.SelectedIndex = 0;

            // Create a frame for the team scores of each division
            foreach (Division d in m.MeetDivisions) {
                Frame f = new Frame();
                f.LoadCompleted += ScorePane_LoadCompleted;
                f.Navigate(new TeamScorePane(), d);

                TabItem tab = new TabItem() { Header=d.DivisionName, Content=f };

                teamScores.Items.Add(tab);
            }
        }

        // Called by amin window whenever the this tab is selected
        public void Refresh() {
            // Force refresh the scores of each division
            foreach (TabItem t in teamScores.Items) {
                ((TeamScorePane)((Frame)t.Content).Content).Refresh();
            }
        }

        // Pass the necessary data to each score pane when it loads
        private void ScorePane_LoadCompleted(object sender, NavigationEventArgs e) {
            ((TeamScorePane)e.Content).Setup((Division)e.ExtraData, m.MeetTeams);
        }

        // Called when a team is selected
        private void teamList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            // Load the setting of the selected team
            Team selected = (Team)teamList.SelectedItem;
            TeamMenuFrame.Navigate(new TeamEditMenu(), selected);
        }

        // Pass necessary data to team edit menu when it loads
        private void TeamMenuFrame_LoadCompleted(object sender, NavigationEventArgs e) {
            if (e.Content.GetType() == typeof(TeamEditMenu)) {
                ((TeamEditMenu)e.Content).SetTeam((Team)e.ExtraData);
                ((TeamEditMenu)e.Content).SetMeet(m);
            }
        }
    }
}
