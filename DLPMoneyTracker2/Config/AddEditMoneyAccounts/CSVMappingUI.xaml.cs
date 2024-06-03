using DLPMoneyTracker2.Core;
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
using System.Windows.Shapes;

namespace DLPMoneyTracker2.Config.AddEditMoneyAccounts
{
    /// <summary>
    /// Interaction logic for CSVMappingUI.xaml
    /// </summary>
    public partial class CSVMappingUI : Window
    {
        private CSVMappingVM _viewModel;

        public CSVMappingUI(CSVMappingVM viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            this.DataContext = viewModel;
        }

        public void LoadMoneyAccount(MoneyAccountVM money)
        {
            _viewModel.LoadMoneyAccount(money);
        }


        private void btnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
