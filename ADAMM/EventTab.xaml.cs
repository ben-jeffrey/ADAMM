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
                entryList.AlternationCount = 2;
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
                        heat.Header = heatTabs.Items.Count + 1;
                        heat.Content = entryList;
                        heatTabs.Items.Add(heat);
                    }
                    else {
                        entryList.Items.Add(entry);
                    }
                }
            }
        }

        private void eventSearch_TextChanged(object sender, TextChangedEventArgs e) {
            eventList.Items.Clear();
            foreach (Event ev in m.MeetEvents)
                if (ev.filter(eventSearch.Text))
                    eventList.Items.Add(ev);
        }

    }
}
