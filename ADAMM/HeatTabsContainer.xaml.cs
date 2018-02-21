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
    /// Interaction logic for HeatTabsContainer.xaml
    /// </summary>
    public partial class HeatTabsContainer : Page {
        public HeatTabsContainer() {
            InitializeComponent();
        }

        public void SetUpHeats(List<Heat> entries) {
            heatTabs.Items.Clear();

            foreach (Heat h in entries) {
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

        void HeatTab_LoadCompleted(object sender, NavigationEventArgs e) {
            Heat h = (Heat)e.ExtraData;
            ((HeatTab)e.Content).SetUpEntries(h);
        }
    }
}
