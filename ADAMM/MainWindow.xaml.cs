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

namespace ADAMM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Meet m;

        public MainWindow() {
            InitializeComponent();
            m = new Meet("C:\\Users\\PinQiblo2\\Desktop\\db.zip");
            Title = m.ToString();
            EventTabFrame.Navigate(new EventTab(), m);
            AthleteTabFrame.Navigate(new AthleteTab(), m);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            m.close();
        }

        

        void EventTab_LoadCompleted(object sender, NavigationEventArgs e) {
            
            m = (Meet)e.ExtraData;
            ((EventTab)e.Content).SetUpMeet(m);
        }

        private void AthleteTabFrame_LoadCompleted(object sender, NavigationEventArgs e) {
            m = (Meet)e.ExtraData;
            ((AthleteTab)e.Content).SetUpMeet(m);
        }
    }
}