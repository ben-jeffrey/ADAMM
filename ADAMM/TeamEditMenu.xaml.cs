using System;
using System.Collections.Generic;
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
    /// Submenu of TeamTab for editing team data
    /// </summary>
    public partial class TeamEditMenu : Page {

        Meet m;
        Team t;

        public TeamEditMenu() {
            InitializeComponent();
        }

        // Called by TeamTab on page load
        public void SetMeet(Meet m) {
            this.m = m;
        }

        // Called by TeamTab on page load
        public void SetTeam(Team t) {
            // Populate all of the text field with the team's data
            this.t = t;
            TeamLongName.Text = t.TeamLongName;
            TeamShortName.Text = t.TeamShortName;
            TeamAbbreviation.Text = t.TeamAbbrev;
            TeamCity.Text = t.TeamCity;
            TeamState.Text = t.TeamState;
            TeamZip.Text = t.TeamZip;
            TeamCountry.Text = t.TeamCountry;

        }

        // Called on 'save' button click
        private void Save_Click(object sender, RoutedEventArgs e) {
            // Write fields out to object
            t.TeamLongName = TeamLongName.Text;
            t.TeamShortName = TeamShortName.Text;
            t.TeamAbbrev = TeamAbbreviation.Text;
            t.TeamCity = TeamCity.Text;
            t.TeamState = TeamState.Text;
            t.TeamZip = TeamZip.Text;
            t.TeamCountry = TeamCountry.Text;

            // Update DB
            m.updateTeam(t);
        }

        // Called on 'cancel' button click
        private void Cancel_Click(object sender, RoutedEventArgs e) {
            // Rewrite the team data to the text fields
            TeamLongName.Text = t.TeamLongName;
            TeamShortName.Text = t.TeamShortName;
            TeamAbbreviation.Text = t.TeamAbbrev;
            TeamCity.Text = t.TeamCity;
            TeamState.Text = t.TeamState;
            TeamZip.Text = t.TeamZip;
            TeamCountry.Text = t.TeamCountry;
        }
    }
}
