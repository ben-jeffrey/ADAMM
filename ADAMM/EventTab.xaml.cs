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
    /// Interaction logic for EventTab.xaml
    /// </summary>
    public partial class EventTab : Page {

        private Meet m;

        public EventTab() {
            InitializeComponent();
        }

        public void SetUpMeet(Meet meet) {
            m = meet;
            foreach (Event ev in m.MeetEvents)
                eventList.Items.Add(ev);

            eventList.SelectedIndex = 0;
        }

        private void eventList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            int difference = e.AddedItems.Count - e.RemovedItems.Count;
            if (difference == 1 || difference == 0) {
                Event selectedEvent = (Event)e.AddedItems[0];
                List<Heat> selectedHeat = selectedEvent.EventHeats;

                eventInteractionPane.Navigate(new HeatTabsContainer(), selectedHeat);
                
            }
        }

        private void eventSearch_TextChanged(object sender, TextChangedEventArgs e) {
            eventList.Items.Clear();
            foreach (Event ev in m.MeetEvents)
                if (ev.filter(eventSearch.Text))
                    eventList.Items.Add(ev);
        }

        private void addEvent_Click(object sender, RoutedEventArgs e) {
            eventInteractionPane.Navigate(new CreateEventMenu());
        }

        private void adjustEvent_Click(object sender, RoutedEventArgs e) {
            eventInteractionPane.Navigate(new EventAdjustMenu(), eventList.SelectedItem);
        }

        private void EventPane_LoadCompleted(object sender, NavigationEventArgs e) {
            if (e.Content.GetType() == typeof(HeatTabsContainer)) {
                List<Heat> h = (List<Heat>)e.ExtraData;
                ((HeatTabsContainer)e.Content).SetUpHeats(h);
            } else if (e.Content.GetType() == typeof(CreateEventMenu)) {
                ((CreateEventMenu)e.Content).Meet = m;
                ((CreateEventMenu)e.Content).Populate();
                ((CreateEventMenu)e.Content).ReturnTo = this;
            } else if (e.Content.GetType() == typeof(EventAdjustMenu)) {
                Event selectedEvent = (Event)e.ExtraData;
                ((EventAdjustMenu)e.Content).Setup(m, selectedEvent);
                ((EventAdjustMenu)e.Content).ReturnTo = this;
            }
        }

        public void PageFinished() {
            eventInteractionPane.Navigate(new HeatTabsContainer(), ((Event)eventList.SelectedItem).EventHeats);
        }
    }
}
