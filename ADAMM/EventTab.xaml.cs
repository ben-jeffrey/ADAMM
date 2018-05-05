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
    /// Submenu of the top-level window meant for entering results and altering event data
    /// </summary>
    public partial class EventTab : Page {

        private Meet m;

        public EventTab() {
            InitializeComponent();
        }

        // Called when the page loads by the main window
        public void SetUpMeet(Meet meet) {
            m = meet;

            // Populate event list and select event #1
            foreach (Event e in m.MeetEvents)
                eventList.Items.Add(e);
            eventList.SelectedIndex = 0;
        }

        // Called when an event list selection is changed
        private void eventList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            // If an item was added to the selection or the selection was unchanged in size
            int difference = e.AddedItems.Count - e.RemovedItems.Count;
            if (difference == 1 || difference == 0) {
                // Display the heats of the selected event
                Event selectedEvent = (Event)e.AddedItems[0];
                List<Heat> selectedHeat = selectedEvent.EventHeats;
                eventInteractionPane.Navigate(new HeatTabsContainer(), selectedHeat);
                
            }
        }

        // Called when a character is added to or removed from the search bar
        private void eventSearch_TextChanged(object sender, TextChangedEventArgs e) {
            // Repopulate event list with unfiltered events
            eventList.Items.Clear();
            foreach (Event ev in m.MeetEvents)
                if (ev.filter(eventSearch.Text))
                    eventList.Items.Add(ev);
        }


        /* These 8 functions all send the necessary data to a submenu when the relevant button is pressed */
        private void addEvent_Click(object sender, RoutedEventArgs e) {
            eventInteractionPane.Navigate(new CreateEventMenu());
        }

        private void adjustEvent_Click(object sender, RoutedEventArgs e) {
            eventInteractionPane.Navigate(new EventAdjustMenu(), eventList.SelectedItem);
        }

        private void seedEvent_Click(object sender, RoutedEventArgs e) {
            eventInteractionPane.Navigate(new EventSeedMenu(), eventList.SelectedItem);
        }

        private void reportEvent_Click(object sender, RoutedEventArgs e) {
            List<Event> tempEventList = new List<Event>();
            foreach (Event evt in eventList.SelectedItems)
                tempEventList.Add(evt);
            eventInteractionPane.Navigate(new EventReportMenu(), tempEventList);
        }

        private void EventPane_LoadCompleted(object sender, NavigationEventArgs e) {
            if (e.Content.GetType() == typeof(HeatTabsContainer)) {
                List<Heat> h = (List<Heat>)e.ExtraData;
                ((HeatTabsContainer)e.Content).SetUp(m, h);
            } else if (e.Content.GetType() == typeof(CreateEventMenu)) {
                ((CreateEventMenu)e.Content).Meet = m;
                ((CreateEventMenu)e.Content).Populate();
                ((CreateEventMenu)e.Content).ReturnTo = this;
            } else if (e.Content.GetType() == typeof(EventAdjustMenu)) {
                Event selectedEvent = (Event)e.ExtraData;
                ((EventAdjustMenu)e.Content).Setup(m, selectedEvent);
                ((EventAdjustMenu)e.Content).ReturnTo = this;
            } else if (e.Content.GetType() == typeof(EventSeedMenu)) {
                Event selectedEvent = (Event)e.ExtraData;
                ((EventSeedMenu)e.Content).Setup(m, selectedEvent);
                ((EventSeedMenu)e.Content).ReturnTo = this;
            } else if (e.Content.GetType() == typeof(EventReportMenu)) {
                List<Event> selectedEvents = (List<Event>)e.ExtraData;
                ((EventReportMenu)e.Content).Setup(m, selectedEvents);
                ((EventReportMenu)e.Content).ReturnTo = this;
            }
        }

        // Called by a submenu when it's done
        public void PageFinished() {
            eventInteractionPane.Navigate(new HeatTabsContainer(), ((Event)eventList.SelectedItem).EventHeats);
        }

        // Called when the score button is clicked
        private void scoreEvent_Click(object sender, RoutedEventArgs e) {
            // Call the event's scoring routine
            ((Event)eventList.SelectedItem).Score();
        }
    }
}
