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
    /// Interaction logic for EventSeedMenu.xaml
    /// </summary>
    public partial class EventSeedMenu : Page {

        Meet m;
        Event evt;
        public EventTab ReturnTo { get; set; }

        public EventSeedMenu() {
            InitializeComponent();
        }

        public void Setup(Meet m, Event e) {
            this.m = m;
            evt = e;

            SeedEntryList.ItemsSource = e.EventUnseededEntries;
        }

        private void Seed_Click(object sender, RoutedEventArgs e) {
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
    }
}
