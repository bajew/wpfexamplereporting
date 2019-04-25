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
using System.IO;
using System.Windows.Markup;
using System.Xml;
using System.Windows.Threading;
using System.IO.Packaging;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using System.Printing;

namespace WpfXpsReporting
{
    public class Data
    {
        public string Heading { get; set; }
        public DateTime CurrentDate { get; set; }
        public string Name { get; set; }
        public string[] DotPoints { get; set; }
        public bool GiveDiscount { get; set; }
    }

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static void InjectData(FrameworkElement document, object dataSource)
        {
            document.DataContext = dataSource;

            // we need to give the binding infrastructure a push as we
            // are operating outside of the intended use of WPF
            var dispatcher = Dispatcher.CurrentDispatcher;
            dispatcher.Invoke(
               DispatcherPriority.SystemIdle,
               new DispatcherOperationCallback(delegate { return null; }),
               null);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FixedDocument doc = new FixedDocument();
            // A4 Standard: 8.27 x 11.69 inch; 96 dpi
            Size documentSize = new Size(96 * 8.27, 96 * 11.69);
            doc.DocumentPaginator.PageSize = documentSize;

            using (FileStream fs = new FileStream("Template.xaml", FileMode.Open, FileAccess.Read))
            {
                FixedPage fixedPage = XamlReader.Load(fs) as FixedPage;
                //TODO Add Data (injection)

                //set page size
                fixedPage.Width = doc.DocumentPaginator.PageSize.Width;
                fixedPage.Height = doc.DocumentPaginator.PageSize.Height;

                PageContent pageContent = new PageContent();
                ((IAddChild)pageContent).AddChild(fixedPage);
                doc.Pages.Add(pageContent);

                //XpsDocument xpsDocument = new XpsDocument("output.xps", FileAccess.Write);
                //XpsDocumentWriter xpsDocumentWriter = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
                //xpsDocumentWriter.Write(doc);
                //xpsDocument.Close();

                docViewer.Document = doc;

                var pq = LocalPrintServer.GetDefaultPrintQueue();
                var writer = PrintQueue.CreateXpsDocumentWriter(pq);
                var paginator = doc.DocumentPaginator;
                writer.Write(paginator);

            }
        }
    }
}
