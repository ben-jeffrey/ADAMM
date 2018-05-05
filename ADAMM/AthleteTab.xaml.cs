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
    /// Submenu of the top-level window meant for reading and altering athlete data
    /// </summary>

    public partial class AthleteTab : Page {

        public Meet m;

        public AthleteTab() {
            InitializeComponent();
        }

        // Called when the page loads by the main window
        public void SetUpMeet(Meet meet) {
            m = meet;

            // Populate displayed list with athletes
            foreach (Team t in m.MeetTeams)
                foreach (Athlete a in t.TeamRoster)
                    athleteList.Items.Add(a);
        }

        //Called when an athlete is selected
        private void athleteList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            // Check that there are items selected
            int difference = e.AddedItems.Count - e.RemovedItems.Count;
            // If a selection was added or if the selection is unchanged
            if (difference == 1 || difference == 0) {

                //Populate the athlete editor fields with the selected athlete's demographics
                Athlete currentAthlete = (Athlete)e.AddedItems[0];
                athleteFName.Text = currentAthlete.AthleteFirstName;
                athleteLName.Text = currentAthlete.AthleteLastName;

                if (currentAthlete.AthleteGender == 'M')
                    athleteMale.IsChecked = true;
                else
                    athleteFemale.IsChecked = true;

                // Populated dropdown for teams and select this athlete's team
                athleteTeam.ItemsSource = m.MeetTeams;
                athleteTeam.SelectedItem = currentAthlete.AthleteTeam;

                // Populate events that the athlete can be in and is in
                //   Clear the previous athlete's data
                athleteEnteredEvents.Items.Clear();
                athleteEligibleEvents.Items.Clear();
                foreach (Event ev in m.getEntriesForAthlete(currentAthlete))
                    athleteEnteredEvents.Items.Add(ev);
                foreach (Event ev in m.getEligibleEventsForAthlete(currentAthlete))
                    athleteEligibleEvents.Items.Add(ev);
            }

        }

        // Called when the athlete's data needs to be written to the DB
        private void athleteUpdate_Click(object sender, RoutedEventArgs e) {
            // Update athlete object's demographics
            Athlete currentAthlete = (Athlete)athleteList.SelectedItem;
            currentAthlete.AthleteFirstName = athleteFName.Text;
            currentAthlete.AthleteLastName = athleteLName.Text;

            if ((bool)athleteMale.IsChecked)
                currentAthlete.AthleteGender = 'M';
            else
                currentAthlete.AthleteGender = 'F';

            // Change the athlete's team to whatever is selected
            // Also change the roster of the new/old team
            if (currentAthlete.AthleteTeam != null)
                currentAthlete.AthleteTeam.TeamRoster.Remove(currentAthlete);
            currentAthlete.AthleteTeam = (Team)athleteTeam.SelectedItem;
            currentAthlete.AthleteTeam.TeamRoster.Add(currentAthlete);

            // Write changes out
            currentAthlete.updateRecord();

            // Send new list of entries to the top level of the object tree so the changes can be propogated
            List<Event> entered = new List<Event>();
            foreach (Event ev in athleteEnteredEvents.Items)
                entered.Add(ev);
            m.updateEntriesForAthlete(currentAthlete, entered);
        }

        // Called to cancel any unsaved changes made to an athlete
        private void athleteRevert_Click(object sender, RoutedEventArgs e) {
            // Overwrite all of the controls with the data from the athlete
            // Works just like the list selectionchanged event above

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

        // Called when a character is added to or removed from the search bar
        // Filters out all athletes that do no match the search query
        private void athleteSearch_TextChanged(object sender, TextChangedEventArgs e) {
            athleteList.Items.Clear();
            foreach (Team t in m.MeetTeams)
                foreach (Athlete a in t.TeamRoster)
                    if (a.filter(athleteSearch.Text))
                        athleteList.Items.Add(a);
        }

        // Called to add a new athlete to the meet
        private void athleteAdd_Click(object sender, RoutedEventArgs e) {
            // Create an empty athlete and select it for editing
            Athlete newAthlete = m.addNewAthlete();
            athleteList.Items.Add(newAthlete);
            athleteList.SelectedItem = newAthlete;
            athleteList.ScrollIntoView(newAthlete);
        }

        // Called when an event is dropped onto the 'entered events' list of an athlete
        private void athleteEnteredEvents_Drop(object sender, DragEventArgs e) {
            Event currentEvent = e.Data.GetData(typeof(Event)) as Event;

            // If the athlete is already in the event, do nothing
            if (athleteEnteredEvents.Items.Contains(currentEvent)) return;
            
            // Move the event from one list to the other
            athleteEligibleEvents.Items.Remove(currentEvent);
            athleteEnteredEvents.Items.Add(currentEvent);

            // Re-sort the list of entered events
            athleteEnteredEvents.Items.SortDescriptions.Clear();
            athleteEnteredEvents.Items.SortDescriptions.Add(
                new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
        }

        // Called when an entry is dropped onto the 'eligible' list of an athlete
        private void athleteEligibleEvents_Drop(object sender, DragEventArgs e) {
            Event currentEvent = e.Data.GetData(typeof(Event)) as Event;
            
            // If the athlete is already not in the event, do nothing
            if (athleteEligibleEvents.Items.Contains(currentEvent)) return;

            // Move the event from one list to the other
            athleteEnteredEvents.Items.Remove(currentEvent);
            athleteEligibleEvents.Items.Add(currentEvent);

            // Re-sort the list of eligible events
            athleteEligibleEvents.Items.SortDescriptions.Clear();
            athleteEligibleEvents.Items.SortDescriptions.Add(
                new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
        }

        // Called when an event in either list is clicked
        private void athleteEvents_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            // Figure out which list was clicked
            ListBox parent = (ListBox)sender;

            // Find which child of the list was clicked
            object data = GetDataFromListBox(parent, e.GetPosition(parent));

            // If a child was clicked
            if (data != null) {
                // Select the child
                parent.SelectedItem = data;
                // Begin dragdrop
                DragDrop.DoDragDrop(parent, data, DragDropEffects.Move);
            }
        }

        // Called to hit-test the listboxes to find what item to drag
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
