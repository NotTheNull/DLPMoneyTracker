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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DLPMoneyTracker.ReportViews
{
    /// <summary>
    /// Interaction logic for MoneyAccountsOverview.xaml
    /// </summary>
    public partial class MoneyAccountsOverview : UserControl
    {
        private MoneyAccountOverviewVM ViewModel { get { return this.DataContext as MoneyAccountOverviewVM; } }

        public MoneyAccountsOverview(MoneyAccountOverviewVM viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;

        }

        public void Refresh()
        {
            this.ViewModel.Refresh();
        }
    }
}
