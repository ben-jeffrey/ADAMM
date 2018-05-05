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
using System.Windows.Threading;

namespace ADAMM
{
    /// <summary>
    /// Root windo for the app
    /// Primarily consists of 4 tabs that each have a frame that loads a section of the app
    /// Also responsible for initializing the Meet
    /// </summary>
    public partial class MainWindow : Window
    {
        Meet m;

        public MainWindow() {
            InitializeComponent();
            // Create Meet object from given database
            // Hardcoded, but trivial to implement as an input
            m = new Meet("C:\\Users\\PinQiblo2\\Desktop\\db.zip");
            // Set window title to the meet's name
            Title = m.ToString();
            // Load the main app menus into their respective tabs
            EventTabFrame.Navigate(new EventTab(), m);
            AthleteTabFrame.Navigate(new AthleteTab(), m);
            TeamTabFrame.Navigate(new TeamTab(), m);
        }

        // Close the window properly when X is clicked
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            m.close();
        }

        

        // Pass the Meet object to the submenus when they load
        void EventTab_LoadCompleted(object sender, NavigationEventArgs e) {
            m = (Meet)e.ExtraData;
            ((EventTab)e.Content).SetUpMeet(m);
        }
        private void AthleteTabFrame_LoadCompleted(object sender, NavigationEventArgs e) {
            m = (Meet)e.ExtraData;
            ((AthleteTab)e.Content).SetUpMeet(m);
        }
        private void TeamTabFrame_LoadCompleted(object sender, NavigationEventArgs e) {
            m = (Meet)e.ExtraData;
            ((TeamTab)e.Content).SetUpMeet(m);
        }

        // If the Teams tab is selected, refresh the team scores
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.Source is TabControl)
                if (((TabItem)e.AddedItems[0]).Header.ToString() == "Teams")
                    ((TeamTab)TeamTabFrame.Content).Refresh();
        }
    }
}