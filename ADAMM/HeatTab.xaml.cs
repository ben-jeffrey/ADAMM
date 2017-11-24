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
    /// Interaction logic for HeatTab.xaml
    /// </summary>
    public partial class HeatTab : Page {
        public HeatTab() {
            InitializeComponent();
        }

        public void SetUpEntries(List<Entry> entries) {
            foreach (Entry e in entries)
                entryList.Items.Add(e);
        }
    }
}
