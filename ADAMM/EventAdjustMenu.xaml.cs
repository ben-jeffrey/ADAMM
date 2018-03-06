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
        Event evt;
        bool seeded;
        List<Entry> removedEntries = new List<Entry>();
        List<Entry> updatedEntries = new List<Entry>();
        List<Entry> addedEntries = new List<Entry>();

        public EventAdjustMenu() {
            InitializeComponent();
        }

        public void Setup(Meet m, Event e) {
            meet = m;
            evt = e;
            seeded = e.isSeeded();
            List<Entry> flatEntries = new List<Entry>();
            if (seeded) {
                foreach (Heat h in e.EventHeats)
                    flatEntries.AddRange(h.EntriesWithEmpties());

                CollectionViewSource.GetDefaultView(EventEntriesList.ItemsSource).SortDescriptions.Add(new SortDescription("EntryHeat", ListSortDirection.Ascending));
                CollectionViewSource.GetDefaultView(EventEntriesList.ItemsSource).SortDescriptions.Add(new SortDescription("EntryPosition", ListSortDirection.Ascending));
            } else
                flatEntries = e.EventUnseededEntries;

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
            foreach (Athlete a in meet.getEligibleAthletesForEvent(evt))
                if (a.filter(AthleteSearch.Text))
                    filteredAthletes.Add(a);
            EligibleAthletesList.ItemsSource = filteredAthletes;
        }

        private void DataList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (sender is ListViewItem) {
                ListViewItem data = sender as ListViewItem;
                data.IsSelected = true;
                DragDrop.DoDragDrop(data, data.DataContext, DragDropEffects.Move);
            }
        }

        private void EntryList_Drop(object sender, DragEventArgs e) {
            Athlete draggedAthlete = e.Data.GetData(typeof(Athlete)) as Athlete;
            Entry draggedEntry = e.Data.GetData(typeof(Entry)) as Entry;
            Entry swapEntry = ((ListViewItem)sender).DataContext as Entry;

            if (draggedEntry != null) {
                int draggedPos = draggedEntry.EntryPosition;
                draggedEntry.EntryPosition = swapEntry.EntryPosition;
                swapEntry.EntryPosition = draggedPos;

                int draggedHeat = draggedEntry.EntryHeat;
                draggedEntry.EntryHeat = swapEntry.EntryHeat;
                swapEntry.EntryHeat = draggedHeat;

                updatedEntries.Add(draggedEntry);
                updatedEntries.Add(swapEntry);

            } else if (draggedAthlete != null) {
                Entry newEntry = new Entry(swapEntry.EntryPosition, swapEntry.EntryHeat, draggedAthlete.AthletePointer, swapEntry.EntryEvent);
                List<Entry> newEntries = new List<Entry>();

                newEntry.EntryAthlete = draggedAthlete;
                newEntries.Add(newEntry);
                addedEntries.Add(newEntry);
                removedEntries.Add(swapEntry);

                foreach (Entry ent in EventEntriesList.ItemsSource)
                    if (ent != swapEntry)
                        newEntries.Add(ent);

                EventEntriesList.ItemsSource = newEntries;
            }

            if (seeded) {
                CollectionViewSource.GetDefaultView(EventEntriesList.ItemsSource).SortDescriptions.Add(new SortDescription("EntryHeat", ListSortDirection.Ascending));
                CollectionViewSource.GetDefaultView(EventEntriesList.ItemsSource).SortDescriptions.Add(new SortDescription("EntryPosition", ListSortDirection.Ascending));
            }

            
            //EventEntriesList.Items[draggedIndex].IsSelected = false;
        }

        private void Save_Click(object sender, RoutedEventArgs e) {
            meet.removeEntries(removedEntries);
            meet.updateEntries(updatedEntries);
            meet.addEntries(addedEntries);
            ReturnTo.PageFinished();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
            ReturnTo.PageFinished();
        }
    }
}
