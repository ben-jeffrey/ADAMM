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

        public  Report(List<Event> e) {
            Document = new FixedDocument();
            FixedDocument template;
            string absolutePath = Path.GetFullPath("../../Reports/FlowDocument1.xaml");
            string directoryPath = Path.GetDirectoryName(absolutePath);

            using (FileStream inputStream = File.OpenRead(absolutePath)) {
                var pc = new ParserContext {
                    BaseUri = new Uri(directoryPath + "\\")
                };
                template = (FixedDocument)XamlReader.Load(inputStream, pc);
                template.DataContext = e;
            }

            UIElement TopLevel = template.Pages[0].Child.Children[1];
            TopLevel.Measure(new Size(double.MaxValue, double.MaxValue));
            TopLevel.Arrange(new Rect(new Point(0, 0), TopLevel.DesiredSize));
            TopLevel.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);

            double yOffset = 0;
            while (yOffset < TopLevel.DesiredSize.Height) {
                VisualBrush vb = new VisualBrush(TopLevel);
                vb.Stretch = Stretch.None;
                vb.AlignmentX = AlignmentX.Left;
                vb.AlignmentY = AlignmentY.Top;
                vb.ViewboxUnits = BrushMappingMode.Absolute;
                vb.TileMode = TileMode.None;
                vb.Viewbox = new Rect(0, yOffset, width, height);

                PageContent pc = new PageContent();
                FixedPage page = new FixedPage();
                pc.Child = page;
                Document.Pages.Add(pc);
                page.Width = width;
                page.Height = height;

                Canvas canvas = new Canvas();
                FixedPage.SetLeft(canvas, 0);
                FixedPage.SetTop(canvas, 0);
                canvas.Width = width;
                canvas.Height = height;
                canvas.Background = vb;
                page.Children.Add(canvas);

                yOffset += height;
            }
        }
    }
}
