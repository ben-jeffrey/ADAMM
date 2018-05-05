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
    /// Submenu of EventTab for creating new events
    /// </summary>
    public partial class CreateEventMenu : Page {

        private Meet m;
        public Meet Meet { set { m = value; } }
        public EventTab ReturnTo;

        public CreateEventMenu() {
            InitializeComponent();
        }

        // Called by EventTab when the page is loaded
        public void Populate() {
            // Load in the division choices to the dropdown
            foreach (Division d in m.MeetDivisions)
                newEventDivision.Items.Add(d);

            // Set the type of event to 'running event'
            newEventType.SelectedIndex = 0;

            // Set the selected division to the first one
            newEventDivision.SelectedIndex = 0;

            // Set the event number to the next availible number
            newEventNumber.Text = m.getNextEventNumber().ToString();

            // Assume that we start in lanes
            newEventInLanes.IsChecked = true;
        }

        // Called when the type of the event is changed
        private void newEventType_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            int selected = newEventType.SelectedIndex;

            // Array of setting boxes for the event types
            Border[] displays = new Border[] { settingsRunning, settingsField, settingsRelay, settingsMulti };

            // Hide all settings
            foreach (Border d in displays)
                d.Visibility = Visibility.Collapsed;

            // Show the setting with the index of the selected dropdown option
            displays[selected].Visibility = Visibility.Visible;
        }

        // Called when the 'in lanes' checkbox is changed
        private void newEventInLanes_Click(object sender, RoutedEventArgs e) {
            // Disable input to the 'number of lanes' input if the checkbox is unchecked
            newEventLaneCount.IsEnabled = (bool)newEventInLanes.IsChecked;
        }

        // Called when the 'create' button is clicked to finalize the event
        private void newEventCreate_Click(object sender, RoutedEventArgs e) {
            // Get the number, gender, and division of the new event
            int number = int.Parse(newEventNumber.Text);

            char gender;
            if ((bool)newEventMale.IsChecked)
                gender = 'M';
            else if ((bool)newEventFemale.IsChecked)
                gender = 'F';
            else
                gender = 'X';

            Division div = (Division)newEventDivision.SelectedItem;

            // Empty new event object
            Event newEvent = null;

            // Switch on event types to control the parameters of each separately
            switch (((ComboBoxItem)newEventType.SelectedItem).Content) {
                case "Running Event":
                    // Get running distance, run type, and lane count (if needed)
                    int distance = int.Parse(newEventDistance.Text);
                    char category = Event.CategoryChars[(string)((ComboBoxItem)newRunningCategory.SelectedItem).Content];
                    int positions = newEventLaneCount.IsEnabled ? int.Parse(newEventLaneCount.Text) : 0;
                    // Create event
                    newEvent = new RunningEvent(number, -1, gender, positions, div, 'U', category, distance, 'M');
                    break;

                case "Field Event":
                    // Get type of field event and number of positions (if needed)
                    category = Event.CategoryChars[(string)newFieldCategory.SelectedItem];
                    positions = newEventInFlights.IsEnabled ? int.Parse(newEventFlightCount.Text) : 0;
                    // Create event
                    newEvent = new FieldEvent(number, -1, gender, div, 'U', category, 'E');
                    break;

                case "Relay Event":
                    //TODO: Implement relays
                    break;

                case "Multi Event":
                    //TODO: Implement multi-events
                    break;
            }

            // Add the event to the DB
            m.addNewEvent(newEvent);

            // Return to EventTab
            ReturnTo.PageFinished();
        }

        // Called when the creation is canceled
        private void newEventCancel_Click(object sender, RoutedEventArgs e) {
            // Return to EventTab
            ReturnTo.PageFinished();
        }
    }

}
