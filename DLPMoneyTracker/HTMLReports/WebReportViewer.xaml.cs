using System.Windows;

namespace DLPMoneyTracker.HTMLReports
{
    /// <summary>
    /// Interaction logic for WebBrowser.xaml
    /// </summary>
    public partial class WebReportViewer : Window
    {
        private IWebViewModel _viewModel;

        public WebReportViewer(IWebViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            _viewModel = viewModel;
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.BuildHTML();
        }
    }
}