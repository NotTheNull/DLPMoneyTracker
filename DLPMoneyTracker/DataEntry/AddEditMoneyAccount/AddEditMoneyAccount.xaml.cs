using DLPMoneyTracker.Data;
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

namespace DLPMoneyTracker.DataEntry.AddEditMoneyAccount
{
    /// <summary>
    /// Interaction logic for AddEditMoneyAccount.xaml
    /// </summary>
    public partial class AddEditMoneyAccount : Window
    {
        private ITrackerConfig _config;
        private AddEditMoneyAccountVM ViewModel 
        { 
            get 
            {
                if (this.DataContext is null) return null;
                return this.DataContext as AddEditMoneyAccountVM;
            } 
        }

        public AddEditMoneyAccount(ITrackerConfig config, AddEditMoneyAccountVM viewModel)
        {
            InitializeComponent();

            _config = config;
            this.DataContext = viewModel;

            this.ViewModel.NotifyAll();
        }

        private void btnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Dispose();
            this.Close();
        }
    }
}
