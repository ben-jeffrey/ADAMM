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

    }
}
