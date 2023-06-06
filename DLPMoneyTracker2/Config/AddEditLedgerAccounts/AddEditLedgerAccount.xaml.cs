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
using System.Windows.Shapes;

namespace DLPMoneyTracker2.Config.AddEditLedgerAccounts
{
    /// <summary>
    /// Interaction logic for AddEditLedgerAccount.xaml
    /// </summary>
    public partial class AddEditLedgerAccount : Window
    {
        public AddEditLedgerAccount(AddEditLedgerAccountVM viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void btnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
