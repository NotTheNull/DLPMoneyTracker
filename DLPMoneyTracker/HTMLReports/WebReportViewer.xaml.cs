using Microsoft.Toolkit.Win32.UI.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
