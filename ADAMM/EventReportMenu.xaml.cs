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
using System.Printing;
using System.Windows.Threading;

namespace ADAMM {
    /// <summary>
    /// Interaction logic for EventReportMenu.xaml
    /// </summary>
    public partial class EventReportMenu : Page {

        public EventTab ReturnTo;

        public EventReportMenu() {
            InitializeComponent();
        }

        public void Setup(Meet m, List<Event> e) {
            //PrintDialog printDialog = new PrintDialog();

            Report report = new Report(e, m);
            
            //doc.Name = "TEST";
            IDocumentPaginatorSource paginatorSource = report.Document;

            docDisplay.Document = paginatorSource;
            report.Document.DataContext = e;
            //paginatorSource.DocumentPaginator.PageSize

            //printDialog.PrintDocument(paginatorSource.DocumentPaginator, "Hello");
        }
    }
}
