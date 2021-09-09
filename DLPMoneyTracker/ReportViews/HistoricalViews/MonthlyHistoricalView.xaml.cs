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

namespace DLPMoneyTracker.ReportViews.HistoricalViews
{
    /// <summary>
    /// Interaction logic for MonthlyHistoricalView.xaml
    /// </summary>
    public partial class MonthlyHistoricalView : Window
    {
        private readonly MonthlyHistoricalVM _viewModel;
        public MonthlyHistoricalView(MonthlyHistoricalVM viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            _viewModel = viewModel;
        }
    }
}
