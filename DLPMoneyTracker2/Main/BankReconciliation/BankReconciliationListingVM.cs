﻿using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.BankReconciliation;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.Main.BankReconciliation
{
	public class BankReconciliationListingVM : BaseViewModel
	{
		private readonly ITrackerConfig config;
		private readonly IBRManager bankManager;

		public BankReconciliationListingVM(ITrackerConfig config, IBRManager bankManager)
        {
			this.config = config;
			this.bankManager = bankManager;

			this.Load();
		}


		public ObservableCollection<BankFileVM> BankReconciliationList { get; } = new ObservableCollection<BankFileVM>();

		private void Load()
		{
			var listMoneyAccounts = config.GetJournalAccountList(JournalAccountSearch.GetMoneyAccounts()).ToList();

			foreach(var account in listMoneyAccounts.OrderBy(o => o.OrderBy))
			{
				var file = bankManager.GetReconciliationFile(account.Id);
				this.BankReconciliationList.Add(new BankFileVM(config, file));
			}


		}

    }
}