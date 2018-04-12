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

        public FixedDocument Document { get; }
        public const int DPI = 96;
        public const double width = 8.5 * DPI;
        public const double height = 11 * DPI;

        private static Action EmptyDelegate = delegate () { };

        public Report(List<Event> e, Meet m) {
            FixedDocument header = ParseDocFromXAML("../../Reports/Header.xaml");
            header.DataContext = m;
            FixedDocument template = ParseDocFromXAML("../../Reports/MeetProgram.xaml");
            template.DataContext = e;

            UIElement HeaderPanel = header.Pages[0].Child.Children[0];
            UIElement TopLevel = template.Pages[0].Child.Children[0];

            List<UIElement> controls = new List<UIElement>();
            controls.Add(HeaderPanel);
            controls.Add(TopLevel);

            Document = DocumentFromControls(controls);
        }

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

        private FixedDocument DocumentFromControls(List<UIElement> controls) {
            FixedDocument document = new FixedDocument();

            double offsetInPage = .25 * DPI;

            PageContent currentPage = newPageContent();
            document.Pages.Add(currentPage);

            foreach (UIElement control in controls) {
                control.Measure(new Size(double.MaxValue, double.MaxValue));
                control.Arrange(new Rect(new Point(0, 0), control.DesiredSize));
                control.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
                double offsetInControl = 0;

                double trueHeight = Math.Min(height, control.DesiredSize.Height);
                while (offsetInControl < control.DesiredSize.Height) {
                    VisualBrush vb = newVisualBrush(control);
                    vb.Viewbox = new Rect(0, offsetInControl, width, trueHeight);

                    if (offsetInPage > height) {
                        currentPage = newPageContent();
                        document.Pages.Add(currentPage);
                        offsetInPage = .25 * DPI;
                    }

                    Canvas canvas = new Canvas();
                    FixedPage.SetLeft(canvas, .25 * DPI);
                    FixedPage.SetTop(canvas, offsetInPage);
                    canvas.Width = width;
                    canvas.Height = trueHeight;
                    canvas.Background = vb;
                    currentPage.Child.Children.Add(canvas);

                    offsetInPage += trueHeight;
                    offsetInControl += trueHeight;
                }
            }
            return document;
        }

        private VisualBrush newVisualBrush(UIElement control) {
            VisualBrush vb = new VisualBrush(control);
            vb.Stretch = Stretch.None;
            vb.AlignmentX = AlignmentX.Left;
            vb.AlignmentY = AlignmentY.Top;
            vb.ViewboxUnits = BrushMappingMode.Absolute;
            return vb;
        }

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
