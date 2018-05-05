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
    /// Intermediary menu to hold each of the heats of the EventTab selected event
    /// </summary>
    public partial class HeatTabsContainer : Page {
        Meet m;

        public HeatTabsContainer() {
            InitializeComponent();
        }

        // Called by EventTab on page load
        public void SetUp(Meet m, List<Heat> heats) {
            heatTabs.Items.Clear();
            this.m = m;

            // Manually create a tab item for each heat in the given list
            foreach (Heat h in heats) {
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
            
            // Select the first heat
            heatTabs.SelectedIndex = 0;
        }

        // Pass the relevant heat to each tab when they load
        void HeatTab_LoadCompleted(object sender, NavigationEventArgs e) {
            Heat h = (Heat)e.ExtraData;
            ((HeatTab)e.Content).SetUp(m, h);
        }
    }
}
