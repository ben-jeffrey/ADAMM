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

namespace ADAMM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Meet m;
        public MainWindow() {
            InitializeComponent();
            m = new Meet("C:\\Users\\PinQiblo2\\Desktop\\db.mdb");
            Title = m.ToString();
            foreach (Event e in m.MeetEvents) 
                eventList.Items.Add(e);

            foreach (Team t in m.MeetTeams) {
                foreach (Athlete a in t.TeamRoster) {
                    athleteList.Items.Add(a);
                }
            }
        }

        private void eventList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems.Count - e.RemovedItems.Count <= 1) {
                int selectedEvent = ((Event)e.AddedItems[0]).EventNumber;
                List<String> selectedEntries = m.getEntriesForEvent(selectedEvent);

                heatTabs.Items.Clear();
                TabItem heat = new TabItem();
                ListView entryList = new ListView();
                heat.HorizontalAlignment = HorizontalAlignment.Stretch;
                heat.VerticalAlignment = VerticalAlignment.Stretch;
                heat.Height = double.NaN;
                heat.Width = double.NaN;
                heat.Header = 1;
                heat.Content = entryList;
                heatTabs.Items.Add(heat);
                heatTabs.SelectedIndex = 0;
                
                foreach (String entry in selectedEntries) {
                    if (entry == "") {
                        heat = new TabItem();
                        entryList = new ListView();
                        heat.HorizontalAlignment = HorizontalAlignment.Stretch;
                        heat.VerticalAlignment = VerticalAlignment.Stretch;
                        heat.Height = double.NaN;
                        heat.Width = double.NaN;
                        heat.Header = heatTabs.Items.Count+1;
                        heat.Content = entryList;
                        heatTabs.Items.Add(heat);
                    } else {
                        entryList.Items.Add(entry);
                    }
                }
            }
        }

        private void athleteList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            Athlete currentAthlete = (Athlete)e.AddedItems[0];
            athleteFName.Text = currentAthlete.firstName;
            athleteLName.Text = currentAthlete.lastName;

            if (currentAthlete.AthleteGender == 'M')
                athleteMale.IsChecked = true;
            else
                athleteFemale.IsChecked = true;

            athleteTeam.ItemsSource = m.MeetTeams;
            athleteTeam.SelectedItem = currentAthlete.AthleteTeam;
        }

        private void athleteUpdate_Click(object sender, RoutedEventArgs e) {
            Athlete currentAthlete = (Athlete)athleteList.SelectedItem;
            currentAthlete.firstName = athleteFName.Text;
            currentAthlete.lastName = athleteLName.Text;
            if ((bool)athleteMale.IsChecked)
                currentAthlete.AthleteGender = 'M';
            else
                currentAthlete.AthleteGender = 'F';
            currentAthlete.AthleteTeam = (Team)athleteTeam.SelectedItem;

            currentAthlete.updateRecord();
        }

        private void athleteRevert_Click(object sender, RoutedEventArgs e) {
            Athlete currentAthlete = (Athlete)athleteList.SelectedItem;
            athleteFName.Text = currentAthlete.firstName;
            athleteLName.Text = currentAthlete.lastName;

            if (currentAthlete.AthleteGender == 'M')
                athleteMale.IsChecked = true;
            else
                athleteFemale.IsChecked = true;

            athleteTeam.ItemsSource = m.MeetTeams;
            athleteTeam.SelectedItem = currentAthlete.AthleteTeam;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            m.close();
        }
    }
}
