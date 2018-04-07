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
    /// Interaction logic for CreateEventMenu.xaml
    /// </summary>
    public partial class CreateEventMenu : Page {

        private Meet m;
        public Meet Meet { set { m = value; } }
        public EventTab ReturnTo;

        public CreateEventMenu() {
            InitializeComponent();
        }

        public void Populate() {
            foreach (Division d in m.MeetDivisions)
                newEventDivision.Items.Add(d);
            newEventType.SelectedIndex = 0;
            newEventDivision.SelectedIndex = 0;
            newEventNumber.Text = m.getNextEventNumber().ToString();
            newEventInLanes.IsChecked = true;
        }

        private void newEventType_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            int selected = newEventType.SelectedIndex;
            Border[] displays = new Border[] { settingsRunning, settingsField, settingsRelay, settingsMulti };
            foreach (Border d in displays)
                d.Visibility = Visibility.Collapsed;
            displays[selected].Visibility = Visibility.Visible;
        }

        private void newEventInLanes_Click(object sender, RoutedEventArgs e) {
            newEventLaneCount.IsEnabled = (bool)newEventInLanes.IsChecked;
        }

        private void newEventCreate_Click(object sender, RoutedEventArgs e) {
            int number = int.Parse(newEventNumber.Text);

            char gender;
            if ((bool)newEventMale.IsChecked)
                gender = 'M';
            else if ((bool)newEventFemale.IsChecked)
                gender = 'F';
            else
                gender = 'X';

            Division div = (Division)newEventDivision.SelectedItem;

            Event newEvent = null;

            switch (((ComboBoxItem)newEventType.SelectedItem).Content) {
                case "Running Event":
                    int distance = int.Parse(newEventDistance.Text);
                    char category = Event.CategoryChars[(string)((ComboBoxItem)newRunningCategory.SelectedItem).Content];
                    int positions = newEventLaneCount.IsEnabled ? int.Parse(newEventLaneCount.Text) : 0;
                    newEvent = new RunningEvent(number, -1, gender, positions, div, 'U', category, distance, 'M');
                    break;

                case "Field Event":
                    category = Event.CategoryChars[(string)newFieldCategory.SelectedItem];
                    positions = newEventInFlights.IsEnabled ? int.Parse(newEventFlightCount.Text) : 0;
                    newEvent = new FieldEvent(number, -1, gender, div, 'U', category, 'E');
                    break;

                case "Relay Event":

                    break;

                case "Multi Event":

                    break;
            }
            m.addNewEvent(newEvent);

            ReturnTo.PageFinished();
        }

        private void newEventCancel_Click(object sender, RoutedEventArgs e) {
            ReturnTo.PageFinished();
        }
    }

}
