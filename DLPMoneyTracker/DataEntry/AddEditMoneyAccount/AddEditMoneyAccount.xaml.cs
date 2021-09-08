using DLPMoneyTracker.Data;
using System.Windows;

namespace DLPMoneyTracker.DataEntry.AddEditMoneyAccount
{
    /// <summary>
    /// Interaction logic for AddEditMoneyAccount.xaml
    /// </summary>
    public partial class AddEditMoneyAccount : Window
    {
        private readonly ITrackerConfig _config;

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