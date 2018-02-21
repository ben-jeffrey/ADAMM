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
using System.ComponentModel;

namespace ADAMM {
    /// <summary>
    /// Interaction logic for EventAdjustMenu.xaml
    /// </summary>
    public partial class EventAdjustMenu : Page {

        public EventTab ReturnTo;
        Meet meet;

        public EventAdjustMenu() {
            InitializeComponent();
        }

        public void Setup(Meet m, Event e) {
            meet = m;
            List<Entry> flatEntries = new List<Entry>();
            foreach (Heat h in e.EventHeats)
                flatEntries.AddRange(h.HeatEntries);
            EventEntriesList.ItemsSource = flatEntries;

            /* Alternating Colors
            foreach (Entry entry in EventEntriesList.Items) {
                if (entry.EntryHeat % 2 == 0) {
                    ListViewItem container = (ListViewItem)EventEntriesList.ItemContainerGenerator.ContainerFromItem(entry);
                    container.Background = Brushes.LightGray;
                }
            } */

            List<Athlete> eligibleAthletes = m.getEligibleAthletesForEvent(e);
            EligibleAthletesList.ItemsSource = eligibleAthletes;
        }

        private void AthleteSearch_TextChanged(object sender, TextChangedEventArgs e) {
            List<Athlete> filteredAthletes = new List<Athlete>();
            foreach (Team t in meet.MeetTeams)
                foreach (Athlete a in t.TeamRoster)
                    if (a.filter(AthleteSearch.Text))
                        filteredAthletes.Add(a);
            EligibleAthletesList.ItemsSource = filteredAthletes;
        }

        private void EntryList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (sender is ListViewItem) {
                ListViewItem data = sender as ListViewItem;
                data.IsSelected = true;
                DragDrop.DoDragDrop(data, data.DataContext, DragDropEffects.Move);
            }
        }

        private void EntryList_Drop(object sender, DragEventArgs e) {
            Entry draggedEntry = e.Data.GetData(typeof(Entry)) as Entry;
            Entry swapEntry = ((ListViewItem)sender).DataContext as Entry;

            int draggedIndex = draggedEntry.EntryPosition - 1;
            int swapIndex = swapEntry.EntryPosition - 1;

            draggedEntry.EntryPosition = swapIndex + 1;
            swapEntry.EntryPosition = draggedIndex + 1;

            CollectionViewSource.GetDefaultView(EventEntriesList.ItemsSource).SortDescriptions.Add(new SortDescription("EntryPosition", ListSortDirection.Ascending));

            //EventEntriesList.ItemsSource. [swapIndex] = draggedEntry;
            //EventEntriesList.ItemsSource[draggedIndex] = swapEntry;
        }
    }
}
