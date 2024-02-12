﻿using System;
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

namespace DLPMoneyTracker2.Main.BankReconciliation
{
	/// <summary>
	/// Interaction logic for BankReconciliationListingUI.xaml
	/// </summary>
	public partial class BankReconciliationListingUI : UserControl
	{
		public BankReconciliationListingUI(BankReconciliationListingVM viewModel)
		{
			InitializeComponent();
			this.DataContext = viewModel;
		}
	}
}
