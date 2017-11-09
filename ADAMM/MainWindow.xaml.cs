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
            m = new Meet("C:\\Users\\PinQiblo2\\Desktop\\db.zip");
            Title = m.ToString();
            foreach (Event e in m.MeetEvents)
                eventList.Items.Add(e);

            foreach (Team t in m.MeetTeams)
                foreach (Athlete a in t.TeamRoster)
                    athleteList.Items.Add(a);
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
            athleteFName.Text = currentAthlete.AthleteFirstName;
            athleteLName.Text = currentAthlete.AthleteLastName;

            if (currentAthlete.AthleteGender == 'M')
                athleteMale.IsChecked = true;
            else
                athleteFemale.IsChecked = true;

            athleteTeam.ItemsSource = m.MeetTeams;
            athleteTeam.SelectedItem = currentAthlete.AthleteTeam;

            athleteEnteredEvents.Items.Clear();
            athleteEligibleEvents.Items.Clear();
            foreach (Event ev in m.getEntriesForAthlete(currentAthlete))
                athleteEnteredEvents.Items.Add(ev);
            foreach (Event ev in m.getEligibleEventsForAthlete(currentAthlete)) 
                athleteEligibleEvents.Items.Add(ev);
            
        }

        private void athleteUpdate_Click(object sender, RoutedEventArgs e) {
            Athlete currentAthlete = (Athlete)athleteList.SelectedItem;
            currentAthlete.AthleteFirstName = athleteFName.Text;
            currentAthlete.AthleteLastName = athleteLName.Text;

            if ((bool)athleteMale.IsChecked)
                currentAthlete.AthleteGender = 'M';
            else
                currentAthlete.AthleteGender = 'F';

            if (currentAthlete.AthleteTeam != null)
                currentAthlete.AthleteTeam.TeamRoster.Remove(currentAthlete);
            currentAthlete.AthleteTeam = (Team)athleteTeam.SelectedItem;
            currentAthlete.AthleteTeam.TeamRoster.Add(currentAthlete);

            currentAthlete.updateRecord();
        }

        private void athleteRevert_Click(object sender, RoutedEventArgs e) {
            Athlete currentAthlete = (Athlete)athleteList.SelectedItem;
            athleteFName.Text = currentAthlete.AthleteFirstName;
            athleteLName.Text = currentAthlete.AthleteLastName;

            if (currentAthlete.AthleteGender == 'M')
                athleteMale.IsChecked = true;
            else if (currentAthlete.AthleteGender == 'F')
                athleteFemale.IsChecked = true;

            athleteTeam.ItemsSource = m.MeetTeams;
            athleteTeam.SelectedItem = currentAthlete.AthleteTeam;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            m.close();
        }

        private void eventSearch_TextChanged(object sender, TextChangedEventArgs e) {
            eventList.Items.Clear();
            foreach (Event ev in m.MeetEvents)
                if (ev.filter(eventSearch.Text))
                    eventList.Items.Add(ev);
        }

        private void athleteSearch_TextChanged(object sender, TextChangedEventArgs e) {
            athleteList.Items.Clear();
            foreach (Team t in m.MeetTeams)
                foreach (Athlete a in t.TeamRoster)
                    if (a.filter(athleteSearch.Text))
                        athleteList.Items.Add(a);
        }

        private void athleteAdd_Click(object sender, RoutedEventArgs e) {
            Athlete newAthlete = m.addNewAthlete();
            athleteList.Items.Add(newAthlete);
            athleteList.SelectedItem = newAthlete;
            athleteList.ScrollIntoView(newAthlete);
        }
    }
}
