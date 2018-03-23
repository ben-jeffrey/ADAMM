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
    /// Interaction logic for EventSeedMenu.xaml
    /// </summary>
    public partial class EventSeedMenu : Page {

        Meet m;
        Event evt;
        List<Entry> breakPoints = new List<Entry>();
        public EventTab ReturnTo { get; set; }

        public EventSeedMenu() {
            InitializeComponent();
        }

        public void Setup(Meet m, Event e) {
            this.m = m;
            evt = e;

            SeedEntryList.ItemsSource = e.EventUnseededEntries;
            ListSortDirection marks = e is FieldEvent ? ListSortDirection.Descending : ListSortDirection.Ascending;
            CollectionViewSource.GetDefaultView(SeedEntryList.ItemsSource).SortDescriptions.Add(new SortDescription("EntrySeedMark", marks));
        }

        private void Seed_Click(object sender, RoutedEventArgs e) {
            if (SeedEntryList.SelectedItems.Count == 0)
                seedWithoutBreaks();
            else {
                Entry lastEntry = SeedEntryList.Items.OfType<Entry>().Last();

                if (!SeedEntryList.SelectedItems.Contains(lastEntry))
                    SeedEntryList.SelectedItems.Add(lastEntry);
                seedWithBreaks();
            }

            m.addEventEntries(evt);

            ReturnTo.PageFinished();
        }

        private void seedWithoutBreaks() {
            int numHeats = 1;
            List<Heat> heats = new List<Heat>();
            List<Entry> entries = new List<Entry>();
            entries.AddRange(evt.EventUnseededEntries);

            if (evt.EventPositionCount > 0)
                numHeats = (int)Math.Ceiling((double)entries.Count / evt.EventPositionCount);

            for (int i = 0; i < numHeats; i++)
                heats.Add(new Heat(evt, i+1));

            Heat currentHeat = heats[0];

            switch (((ComboBoxItem)SeedTypeCombo.SelectedItem).Content) {
                case "Random":
                    while (entries.Count > 0) {
                        while (!currentHeat.full() && entries.Count > 0) {
                            entries.Remove(currentHeat.addRandomEntry(entries));
                        }
                        currentHeat = currentHeat.HeatNumber == heats.Count ? currentHeat : heats[currentHeat.HeatNumber];
                    }
                    break;
                case "Standard":
                    entries = entries.OrderBy(ent => ent.EntrySeedMark).ToList();
                    while (entries.Count > 0) {
                        while (!currentHeat.full() && entries.Count > 0) {
                            entries.Remove(currentHeat.addEntryToNext(entries[0]));
                        }
                        currentHeat = currentHeat.HeatNumber == heats.Count ? currentHeat : heats[currentHeat.HeatNumber];
                    }
                    break;
                case "Snake":
                    entries = entries.OrderBy(ent => ent.EntrySeedMark).ToList();
                    while (entries.Count > 0) {
                        entries.Remove(currentHeat.addEntryToNext(entries[0]));
                        currentHeat = currentHeat.HeatNumber == heats.Count ? heats[0] : heats[currentHeat.HeatNumber];
                    }
                    break;
                default:
                    break;
            }

            evt.EventHeats = heats;
            evt.EventStatus = "1";
        }

        private void seedWithBreaks() {

            List<Heat> heats = new List<Heat>();
            List<Entry> entries = new List<Entry>();
            entries.AddRange(evt.EventUnseededEntries);

            for (int i = 0; i < SeedEntryList.SelectedItems.Count; i++)
                heats.Add(new Heat(evt, i+1));

            Heat currentHeat = heats[0];

            while (entries.Count > 0) {
                Entry currentEntry = entries[0];
                entries.Remove(currentHeat.addEntryToNext(currentEntry));
                if (SeedEntryList.SelectedItems.Contains(currentEntry) && entries.Count > 0)
                    currentHeat = heats[currentHeat.HeatNumber];
            }

            evt.EventHeats = heats;
            evt.EventStatus = "1";
        }
    }
}
