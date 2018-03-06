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
    /// Interaction logic for TeamEditMenu.xaml
    /// </summary>
    public partial class TeamEditMenu : Page {

        Meet m;
        Team t;

        public TeamEditMenu() {
            InitializeComponent();
        }

        public void SetMeet(Meet m) {
            this.m = m;
        }

        public void SetTeam(Team t) {
            this.t = t;
            TeamLongName.Text = t.TeamLongName;
            TeamShortName.Text = t.TeamShortName;
            TeamAbbreviation.Text = t.TeamAbbrev;
            TeamCity.Text = t.TeamCity;
            TeamState.Text = t.TeamState;
            TeamZip.Text = t.TeamZip;
            TeamCountry.Text = t.TeamCountry;

        }

        private void Save_Click(object sender, RoutedEventArgs e) {
            t.TeamLongName = TeamLongName.Text;
            t.TeamShortName = TeamShortName.Text;
            t.TeamAbbrev = TeamAbbreviation.Text;
            t.TeamCity = TeamCity.Text;
            t.TeamState = TeamState.Text;
            t.TeamZip = TeamZip.Text;
            t.TeamCountry = TeamCountry.Text;
            m.updateTeam(t);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
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
