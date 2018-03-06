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
    /// Interaction logic for AthleteTab.xaml
    /// </summary>

    public partial class AthleteTab : Page {

        public Meet m;

        public AthleteTab() {
            InitializeComponent();
        }

        public void SetUpMeet(Meet meet) {
            m = meet;
            foreach (Team t in m.MeetTeams)
                foreach (Athlete a in t.TeamRoster)
                    athleteList.Items.Add(a);
        }

        private void athleteList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            int difference = e.AddedItems.Count - e.RemovedItems.Count;
            if (difference == 1 || difference == 0) {
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

            List<Event> entered = new List<Event>();
            foreach (Event ev in athleteEnteredEvents.Items)
                entered.Add(ev);
            m.updateEntriesForAthlete(currentAthlete, entered);
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

            athleteEnteredEvents.Items.Clear();
            athleteEligibleEvents.Items.Clear();
            foreach (Event ev in m.getEntriesForAthlete(currentAthlete))
                athleteEnteredEvents.Items.Add(ev);
            foreach (Event ev in m.getEligibleEventsForAthlete(currentAthlete))
                athleteEligibleEvents.Items.Add(ev);
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

        private void athleteEnteredEvents_Drop(object sender, DragEventArgs e) {
            Event currentEvent = e.Data.GetData(typeof(Event)) as Event;
            if (athleteEnteredEvents.Items.Contains(currentEvent)) return;
            athleteEligibleEvents.Items.Remove(currentEvent);
            athleteEnteredEvents.Items.Add(currentEvent);
            athleteEnteredEvents.Items.SortDescriptions.Clear();
            athleteEnteredEvents.Items.SortDescriptions.Add(
                new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void athleteEligibleEvents_Drop(object sender, DragEventArgs e) {
            Event currentEvent = e.Data.GetData(typeof(Event)) as Event;
            if (athleteEligibleEvents.Items.Contains(currentEvent)) return;
            athleteEnteredEvents.Items.Remove(currentEvent);
            athleteEligibleEvents.Items.Add(currentEvent);
            athleteEligibleEvents.Items.SortDescriptions.Clear();
            athleteEligibleEvents.Items.SortDescriptions.Add(
                new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void athleteEvents_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            ListBox parent = (ListBox)sender;
            object data = GetDataFromListBox(parent, e.GetPosition(parent));
            if (data != null) {
                parent.SelectedItem = data;
                DragDrop.DoDragDrop(parent, data, DragDropEffects.Move);
            }
        }

        private object GetDataFromListBox(ListBox src, Point pt) {
            UIElement element = src.InputHitTest(pt) as UIElement;
            if (element != null) {
                object data = DependencyProperty.UnsetValue;
                while (data == DependencyProperty.UnsetValue) {
                    data = src.ItemContainerGenerator.ItemFromContainer(element);
                    if (data == DependencyProperty.UnsetValue)
                        element = VisualTreeHelper.GetParent(element) as UIElement;
                    if (element == src)
                        return null;
                }
                if (data != DependencyProperty.UnsetValue)
                    return data;
            }
            return null;
        }
        
    }
}
