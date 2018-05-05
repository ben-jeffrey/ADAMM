using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace ADAMM {
    class Report {

        // Document that the report is wrapped around
        public FixedDocument Document { get; }

        // Constants for paper sizes
        public const int DPI = 96;
        public const double width = 8.5 * DPI;
        public const double height = 11 * DPI;

        private static Action EmptyDelegate = delegate () { };

        public Report(List<Event> e, Meet m) {
            // Get the header template and generate the doc for it
            FixedDocument header = ParseDocFromXAML("../../Reports/Header.xaml");
            header.DataContext = m;
            // Get the meet program template and generate the doc for it
            FixedDocument template = ParseDocFromXAML("../../Reports/MeetProgram.xaml");
            template.DataContext = e;

            // Find the top-level elements for the header and meet program docs
            UIElement HeaderPanel = header.Pages[0].Child.Children[0];
            UIElement TopLevel = template.Pages[0].Child.Children[0];

            // Add the top-level controls to the list of what needs to be rendered
            List<UIElement> controls = new List<UIElement>();
            controls.Add(HeaderPanel);
            controls.Add(TopLevel);

            // Generate the document from controls
            Document = DocumentFromControls(controls);
        }

        // Parse a fixed doc from XAML
        private FixedDocument ParseDocFromXAML(string path) {
            FixedDocument template = null;
            string absolutePath = Path.GetFullPath(path);
            string directoryPath = Path.GetDirectoryName(absolutePath);

            using (FileStream inputStream = File.OpenRead(absolutePath)) {
                var pc = new ParserContext {
                    BaseUri = new Uri(directoryPath + "\\")
                };
                template = (FixedDocument)XamlReader.Load(inputStream, pc);
            }
            return template;
        }


        // Create a document that combines multiple controls
        // Solves an issue with page truncation by taking VisualBrush images of controls
        //    and putting THOSE on seperate pages, rather than the controls
        private FixedDocument DocumentFromControls(List<UIElement> controls) {
            FixedDocument document = new FixedDocument();

            // give ourselves a 1/4 inch top margin
            double offsetInPage = .25 * DPI;

            // New page
            PageContent currentPage = newPageContent();
            document.Pages.Add(currentPage);

            // For each conrol that needs to be rendered
            foreach (UIElement control in controls) {
                // Find the size of the control
                control.Measure(new Size(double.MaxValue, double.MaxValue));
                control.Arrange(new Rect(new Point(0, 0), control.DesiredSize));
                // This forces an update to ensure the size is correct
                control.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
                // Start from the top of the control
                double offsetInControl = 0;

                // Height is either the size of the page, or the remainder of the control
                double trueHeight = Math.Min(height, control.DesiredSize.Height);

                // While we're still not done looking at the whole control:
                while (offsetInControl < control.DesiredSize.Height) {
                    // Create a visual brush on the control to take an image
                    VisualBrush vb = newVisualBrush(control);
                    // Set the viewport on a page-sized section below the place we last looked
                    vb.Viewbox = new Rect(0, offsetInControl, width, trueHeight);

                    // If we've gone over a page, make a new one
                    if (offsetInPage > height) {
                        currentPage = newPageContent();
                        document.Pages.Add(currentPage);
                        // 1/4 inch top margin still
                        offsetInPage = .25 * DPI;
                    }
                    
                    // Canvas to draw the image from the brush onto
                    Canvas canvas = new Canvas();
                    // Left margin of 1/4 inch
                    FixedPage.SetLeft(canvas, .25 * DPI);
                    // Place canvas where we are in the page
                    FixedPage.SetTop(canvas, offsetInPage);

                    // Specify size of canvas fit to control image
                    canvas.Width = width;
                    canvas.Height = trueHeight;
                    // Set brush to get image
                    canvas.Background = vb;
                    // Put the canvas on the page
                    currentPage.Child.Children.Add(canvas);

                    // We have now travelled 'trueHeigh' pixels down the control and page
                    offsetInPage += trueHeight;
                    offsetInControl += trueHeight;
                }
            }
            return document;
        }

        // Create visual brush to out specific needs
        private VisualBrush newVisualBrush(UIElement control) {
            VisualBrush vb = new VisualBrush(control);
            vb.Stretch = Stretch.None;
            vb.AlignmentX = AlignmentX.Left;
            vb.AlignmentY = AlignmentY.Top;
            vb.ViewboxUnits = BrushMappingMode.Absolute;
            return vb;
        }

        // Create new page to our specific needs
        private PageContent newPageContent() {
            PageContent pc = new PageContent();
            FixedPage page = new FixedPage();
            pc.Child = page;
            page.Width = width;
            page.Height = height;
            return pc;
        }
    }
}
