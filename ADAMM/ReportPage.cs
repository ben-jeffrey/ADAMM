using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;
using System.IO;

namespace ADAMM {
    class ReportPage {

        public PageContent Page { get; }
        
        public const int DPI = 96;
        public const double width = 8.5 * DPI;
        public const double height = 11 * DPI;

        public ReportPage(Event e) {
            FixedDocument template;
            string absolutePath = Path.GetFullPath("../../Reports/FlowDocument1.xaml");
            string directoryPath = Path.GetDirectoryName(absolutePath);

            using (FileStream inputStream = File.OpenRead(absolutePath)) {
                var pc = new ParserContext {
                    BaseUri = new Uri(directoryPath + "\\")
                };
                template = (FixedDocument)XamlReader.Load(inputStream, pc);
                template.DataContext = e;
                Page = template.Pages[0];
            }

        }

    }
}
