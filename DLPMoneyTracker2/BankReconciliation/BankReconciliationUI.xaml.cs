
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

namespace DLPMoneyTracker2.BankReconciliation
{
	/// <summary>
	/// Interaction logic for BankReconciliation.xaml
	/// </summary>
	public partial class BankReconciliationUI : Window
	{
		BankReconciliationVM _viewModel;

		public BankReconciliationUI(BankReconciliationVM viewModel)
		{
			InitializeComponent();
			this.DataContext = viewModel;
			_viewModel = viewModel;
		}


		public void LoadAccount(IJournalAccount account)
		{
			_viewModel.LoadAccount(account);
		}

		private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
		{
			_viewModel.Save();
			this.Close();
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			_viewModel.Dispose();
			this.Close();
        }
    }
}
