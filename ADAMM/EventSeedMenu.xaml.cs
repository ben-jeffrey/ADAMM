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
    /// Submenu of EventTab for seeding events
    /// </summary>
    public partial class EventSeedMenu : Page {

        Meet m;
        Event evt;
        // List of entries to seed around if they are provided by the user
        List<Entry> breakPoints = new List<Entry>();
        public EventTab ReturnTo { get; set; }

        public EventSeedMenu() {
            InitializeComponent();
        }

        // Called by EventTab on page load
        public void Setup(Meet m, Event e) {
            this.m = m;
            evt = e;

            // Populate listview with event's entries
            SeedEntryList.ItemsSource = e.EventUnseededEntries;
            // Choose the sorting direction based on event type
            ListSortDirection marks = e is FieldEvent ? ListSortDirection.Descending : ListSortDirection.Ascending;
            // Sort the listview by seed mark
            CollectionViewSource.GetDefaultView(SeedEntryList.ItemsSource).SortDescriptions.Add(new SortDescription("EntrySeedMark", marks));
        }

        // Called when the 'seed' button is clicked
        private void Seed_Click(object sender, RoutedEventArgs e) {
            // If no items are selected as break points
            if (SeedEntryList.SelectedItems.Count == 0)
                seedWithoutBreaks();
            // If some entries are selected as break points
            else {
                // Add the last entry as a break point, in case it was not selected
                Entry lastEntry = SeedEntryList.Items.OfType<Entry>().Last();
                if (!SeedEntryList.SelectedItems.Contains(lastEntry))
                    SeedEntryList.SelectedItems.Add(lastEntry);

                seedWithBreaks();
            }

            // Write new entries out to the DB
            m.addEventEntries(evt);

            // Return to EventTab
            ReturnTo.PageFinished();
        }

        // Called when there are no break points to seed around
        private void seedWithoutBreaks() {
            // Default to one heat
            int numHeats = 1;
            // Prepare lists for heats and entries
            List<Heat> heats = new List<Heat>();
            List<Entry> entries = new List<Entry>();
            // Populate entry list with the entries of the event
            entries.AddRange(evt.EventUnseededEntries);

            // If the event has a maximum amount of positions
            if (evt.EventPositionCount > 0)
                // Calculate the number of heats by dividing the number of entries into the max position count
                numHeats = (int)Math.Ceiling((double)entries.Count / evt.EventPositionCount);

            // Populate heat list with the correct number of empty heats
            for (int i = 0; i < numHeats; i++)
                heats.Add(new Heat(evt, i+1));

            // Start with heat one
            Heat currentHeat = heats[0];

            // Switch on the type of seeding
            switch (((ComboBoxItem)SeedTypeCombo.SelectedItem).Content) {
                // Randomly position all athletes within event
                case "Random":
                    // While there are entries to seed
                    while (entries.Count > 0) {
                        // While there's room in the heat and there are entries left to seed
                        while (!currentHeat.full() && entries.Count > 0) {
                            // Add a random entry to the heat and remove it from the entry list
                            entries.Remove(currentHeat.addRandomEntry(entries));
                        }
                        // If we're not on the last heat, move to the next one
                        currentHeat = currentHeat.HeatNumber == heats.Count ? currentHeat : heats[currentHeat.HeatNumber];
                    }
                    break;
                // "Normal" seeding based on standard positions
                case "Standard":
                    // Sort the entry list by seed mark
                    entries = entries.OrderBy(ent => ent.EntrySeedMark).ToList();
                    
                    // While there are entries to seed
                    while (entries.Count > 0) {
                        // While the head is empty and there are entries to see
                        while (!currentHeat.full() && entries.Count > 0) {
                            // Add the next entry to the heat's next standard position and remove it from the entry list
                            entries.Remove(currentHeat.addEntryToNext(entries[0]));
                        }
                        // If we're not on the last heat, move to the next one
                        currentHeat = currentHeat.HeatNumber == heats.Count ? currentHeat : heats[currentHeat.HeatNumber];
                    }
                    break;
                // "Snake" seeding distributes the best athletes throughout all of the heats
                case "Snake":
                    // Sort the entry lsit by seed mark
                    entries = entries.OrderBy(ent => ent.EntrySeedMark).ToList();

                    // While there are entries to seed
                    while (entries.Count > 0) {
                        // Add the next entry to the heat's next standard position and remove it from the entry list
                        entries.Remove(currentHeat.addEntryToNext(entries[0]));
                        // If we're not on the last heat, move to the next one
                        currentHeat = currentHeat.HeatNumber == heats.Count ? heats[0] : heats[currentHeat.HeatNumber];
                    }
                    break;
                default:
                    break;
            }

            // Give the seeded heats to the event
            evt.EventHeats = heats;
            // Set status to 'seeded'
            evt.EventStatus = "1";
        }

        // Called when there are break points to seed around
        private void seedWithBreaks() {
            // Prepare list of heats and entries
            List<Heat> heats = new List<Heat>();
            List<Entry> entries = new List<Entry>();
            entries.AddRange(evt.EventUnseededEntries);

            // Create an empty heat for each breakpoint
            for (int i = 0; i < SeedEntryList.SelectedItems.Count; i++)
                heats.Add(new Heat(evt, i+1));

            // Start with heat one
            Heat currentHeat = heats[0];

            // While there are entries left to see
            while (entries.Count > 0) {
                // Add next entry to the current heat
                Entry currentEntry = entries[0];
                entries.Remove(currentHeat.addEntryToNext(currentEntry));
                // Move to the next heat when a break point is encountered
                if (SeedEntryList.SelectedItems.Contains(currentEntry) && entries.Count > 0)
                    currentHeat = heats[currentHeat.HeatNumber];
            }

            // Give the seeded heats to the event
            evt.EventHeats = heats;
            // Set status to 'seeded'
            evt.EventStatus = "1";
        }
    }
}
