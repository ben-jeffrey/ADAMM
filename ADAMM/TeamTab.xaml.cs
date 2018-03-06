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
    /// Interaction logic for TeamTab.xaml
    /// </summary>
    public partial class TeamTab : Page {

        Meet m;

        public TeamTab() {
            InitializeComponent();
        }

        public void SetUpMeet(Meet meet) {
            m = meet;
            foreach (Team t in m.MeetTeams)
                teamList.Items.Add(t);

            teamList.SelectedIndex = 0;
        }

        private void teamList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            Team selected = (Team)teamList.SelectedItem;
            TeamMenuFrame.Navigate(new TeamEditMenu(), selected);
        }

        private void TeamMenuFrame_LoadCompleted(object sender, NavigationEventArgs e) {
            if (e.Content.GetType() == typeof(TeamEditMenu)) {
                ((TeamEditMenu)e.Content).SetTeam((Team)e.ExtraData);
                ((TeamEditMenu)e.Content).SetMeet(m);
            }
        }
    }
}
