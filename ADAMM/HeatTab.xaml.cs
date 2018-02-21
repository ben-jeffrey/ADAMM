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

        public void SetUpEntries(Heat h) {
            entryList.ItemsSource = h.HeatEntries;
        }

        private void entryList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (sender is ListBoxItem) {
                ListBoxItem data = sender as ListBoxItem;
                DragDrop.DoDragDrop(data, data.DataContext, DragDropEffects.Move);
                data.IsSelected = true;
            }
        }

        private void entryList_Drop(object sender, DragEventArgs e) {
            Entry draggedEntry = e.Data.GetData(typeof(Entry)) as Entry;
            Entry swapEntry = ((ListBoxItem)sender).DataContext as Entry;
            
            int draggedIndex = draggedEntry.EntryPosition - 1;
            int swapIndex = swapEntry.EntryPosition - 1;

            draggedEntry.EntryPosition = swapIndex + 1;
            swapEntry.EntryPosition = draggedIndex + 1;

            entryList.Items[swapIndex] = draggedEntry;
            entryList.Items[draggedIndex] = swapEntry;
        }
    }
}
