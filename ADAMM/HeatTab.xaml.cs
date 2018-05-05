using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Submenu of EventTab for showing the entries in a single heat
    /// </summary>
    public partial class HeatTab : Page {
        Heat heat;
        Meet m;

        public HeatTab() {
            InitializeComponent();
        }

        // Called from HeatTabsContainer on page load
        public void SetUp(Meet m, Heat h) {
            heat = h;
            this.m = m;
            // Populate listview with entries
            entryList.ItemsSource = h.HeatEntries;
        }

        // Called when the a key is pressed within a results textbox
        private void TextBox_KeyDown(object sender, KeyEventArgs e) {
            // Get the textbox
            TextBox mark = ((TextBox)sender);

            // If the Enter key was pressed
            if (e.Key == Key.Enter) {
                try {
                    // Get entry from listview item
                    Entry current = mark.DataContext as Entry;
                    
                    // Try to parse the text in the box
                    // Will error if it is formatted improperly
                    current.ParseResultInput(mark.Text);
                    // Update results in DB
                    m.resultEntry(current);

                    // Move to the next textbox down, if there is one
                    TextBox next = FindNextTextBox(current);
                    if (next != null)
                        next.Focus();
                } catch (FormatException error) {
                    // If the parse failed, select the text so the user can delete it
                    mark.SelectAll();
                }
            }
        }

        // Called when the curser needs to move from one textbox to the one below it
        private TextBox FindNextTextBox(Entry current) {
            int index = entryList.Items.IndexOf(current);
            if (index + 1 == entryList.Items.Count) return null;

            ListBoxItem currentListContainer = entryList.ItemContainerGenerator.ContainerFromIndex(index + 1) as ListBoxItem;
            DependencyObject currentChild = VisualTreeHelper.GetChild(currentListContainer, 0);

            while (!(currentChild is Grid)) {
                currentChild = VisualTreeHelper.GetChild(currentChild, 0);
            }
            return VisualTreeHelper.GetChild(currentChild, 4) as TextBox;
        }

        // Called when the 'get times' button is clicked for a heat
        private void getTimes_Click(object sender, RoutedEventArgs e) {
            // Get time dict from Meet
            Dictionary<int, string> times = m.getFinishlynxTimes(heat);
            
            // If the times exist, parse them into the entry and update the DB
            if (times.Count == 0) return;
            foreach (Entry ent in heat.HeatEntries) {
                ent.ParseResultInput(times[ent.EntryPosition]);
                m.resultEntry(ent);
            }
        }
    }
}
