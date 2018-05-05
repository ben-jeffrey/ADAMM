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
    /// Submenu of EventTab for moving and adding entries in an event (displays all heats at once)
    /// </summary>
    public partial class EventAdjustMenu : Page {

        public EventTab ReturnTo;
        Meet meet;
        Event evt;
        // Lists to keep track of what entries were changed and how
        List<Entry> removedEntries = new List<Entry>();
        List<Entry> updatedEntries = new List<Entry>();
        List<Entry> addedEntries = new List<Entry>();

        public EventAdjustMenu() {
            InitializeComponent();
        }

        // Called by EventTab when the page loads
        public void Setup(Meet m, Event e) {
            meet = m;
            evt = e;
            
            // Get all entries for an event in one list
            // Unseeded events need to deal with heats
            List<Entry> flatEntries = new List<Entry>();
            if (e.isSeeded()) {
                foreach (Heat h in e.EventHeats)
                    flatEntries.AddRange(h.EntriesWithEmpties());
            } else
                flatEntries = e.EventUnseededEntries;

            // Sort the entries by position if the event is seeded
            EventEntriesList.ItemsSource = flatEntries;
            if (e.isSeeded()) {
                CollectionViewSource.GetDefaultView(EventEntriesList.ItemsSource).SortDescriptions.Add(new SortDescription("EntryHeat", ListSortDirection.Ascending));
                CollectionViewSource.GetDefaultView(EventEntriesList.ItemsSource).SortDescriptions.Add(new SortDescription("EntryPosition", ListSortDirection.Ascending));
            }

            // Populate list of eligible athletes for the event
            List<Athlete> eligibleAthletes = m.getEligibleAthletesForEvent(e);
            EligibleAthletesList.ItemsSource = eligibleAthletes;
        }

        // Called when a character is added or removed from the athlete search bar
        private void AthleteSearch_TextChanged(object sender, TextChangedEventArgs e) {
            // Create new list of only athletes that fit the query
            List<Athlete> filteredAthletes = new List<Athlete>();
            foreach (Athlete a in meet.getEligibleAthletesForEvent(evt))
                if (a.filter(AthleteSearch.Text))
                    filteredAthletes.Add(a);
            // Set the item source to the filtered list
            EligibleAthletesList.ItemsSource = filteredAthletes;
        }

        // Called when an item in the entry list or athlete list is clicked
        private void DataList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (sender is ListViewItem) {
                // Do a dragdrop event with the DataContext
                // Can either be an athlete or entry
                ListViewItem data = sender as ListViewItem;
                data.IsSelected = true;
                DragDrop.DoDragDrop(data, data.DataContext, DragDropEffects.Move);
            }
        }

        // Called when an item is dropped onto the list of entries
        // dropped items can be Athlete or Entry
        private void EntryList_Drop(object sender, DragEventArgs e) {
            // The 'as' keyword makes the variable null if the cast fails
            Athlete draggedAthlete = e.Data.GetData(typeof(Athlete)) as Athlete;
            Entry draggedEntry = e.Data.GetData(typeof(Entry)) as Entry;

            // Get the item that was dropped onto
            Entry swapEntry = ((ListViewItem)sender).DataContext as Entry;

            // If an entry was dragged
            if (draggedEntry != null) {
                // Swap the positions of the entries
                int draggedPos = draggedEntry.EntryPosition;
                draggedEntry.EntryPosition = swapEntry.EntryPosition;
                swapEntry.EntryPosition = draggedPos;

                // Swap the heats of the entries
                int draggedHeat = draggedEntry.EntryHeat;
                draggedEntry.EntryHeat = swapEntry.EntryHeat;
                swapEntry.EntryHeat = draggedHeat;

                // Add both entries to the list of entries that will need to be updated in the DB
                updatedEntries.Add(draggedEntry);
                updatedEntries.Add(swapEntry);

            // If an athlete was dragged
            } else if (draggedAthlete != null) {
                // Create an entry to house the athlete using the swapped entry's data
                Entry newEntry = new Entry(swapEntry.EntryPosition, swapEntry.EntryHeat, draggedAthlete.AthletePointer, swapEntry.EntryEvent);
                newEntry.EntryAthlete = draggedAthlete;
                // List to act as the new item source for the listview
                List<Entry> newEntries = new List<Entry>();

                // Add the new entry to the new itemsource
                newEntries.Add(newEntry);
                // Add the new entry to the list of entries that need to be added to the DB
                addedEntries.Add(newEntry);
                // Add the old entry to the list of entries that need to be removed from the DB
                removedEntries.Add(swapEntry);

                // Populate the new item source with the entries that were unchanged
                foreach (Entry ent in EventEntriesList.ItemsSource)
                    if (ent != swapEntry)
                        newEntries.Add(ent);

                // Set the item source
                EventEntriesList.ItemsSource = newEntries;
            }

            // If the event is seeded, it needs to be re-sorted
            if (evt.isSeeded()) {
                CollectionViewSource.GetDefaultView(EventEntriesList.ItemsSource).SortDescriptions.Add(new SortDescription("EntryHeat", ListSortDirection.Ascending));
                CollectionViewSource.GetDefaultView(EventEntriesList.ItemsSource).SortDescriptions.Add(new SortDescription("EntryPosition", ListSortDirection.Ascending));
            }
        }

        // Called when the 'save' button is clicked
        private void Save_Click(object sender, RoutedEventArgs e) {
            // Update the DB with the entry changes
            meet.removeEntries(removedEntries);
            meet.updateEntries(updatedEntries);
            meet.addEntries(addedEntries);
            // Return to the EventTab main page
            ReturnTo.PageFinished();
        }

        // Called when the 'cancel' button is clicked
        private void Cancel_Click(object sender, RoutedEventArgs e) {
            // Return to the EventTab main page
            ReturnTo.PageFinished();
        }
    }
}
