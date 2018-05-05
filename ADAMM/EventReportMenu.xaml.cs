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
    /// Submenu of EventTab for creating reports for events
    /// </summary>
    public partial class EventReportMenu : Page {

        public EventTab ReturnTo;

        public EventReportMenu() {
            InitializeComponent();
        }

        // Called by EventTab on page load
        public void Setup(Meet m, List<Event> e) {
            // Create the report
            Report report = new Report(e, m);
            
            // Create the paginator for the report
            IDocumentPaginatorSource paginatorSource = report.Document;

            // Display the document to the user
            docDisplay.Document = paginatorSource;
            
            // Set the data of the report to the list of events
            report.Document.DataContext = e;
        }
    }
}
