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
        }

        private void eventList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems.Count - e.RemovedItems.Count <= 1) {
                Event selectedEvent = (Event)e.AddedItems[0];
                List<List<Entry>> selectedEntries = m.getEntriesForEvent(selectedEvent);

                heatTabs.Items.Clear();

                foreach (List<Entry> h in selectedEntries) {
                    TabItem heat = new TabItem();
                    Frame EntryFrame = new Frame();
                    EntryFrame.LoadCompleted += HeatTab_LoadCompleted;
                    EntryFrame.Navigate(new HeatTab(), h);
                    heat.HorizontalAlignment = HorizontalAlignment.Stretch;
                    heat.VerticalAlignment = VerticalAlignment.Stretch;
                    heat.Height = double.NaN;
                    heat.Width = double.NaN;
                    heat.Header = heatTabs.Items.Count + 1; ;
                    heat.Content = EntryFrame;
                    heatTabs.Items.Add(heat);
                }

                heatTabs.SelectedIndex = 0;
            }
        }

        private void eventSearch_TextChanged(object sender, TextChangedEventArgs e) {
            eventList.Items.Clear();
            foreach (Event ev in m.MeetEvents)
                if (ev.filter(eventSearch.Text))
                    eventList.Items.Add(ev);
        }

        void HeatTab_LoadCompleted(object sender, NavigationEventArgs e) {
            List<Entry> h = (List<Entry>)e.ExtraData;
            ((HeatTab)e.Content).SetUpEntries(h);
        }
    }
}
