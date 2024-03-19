

using DLPMoneyTracker.BusinessLogic.UseCases.BankReconciliation.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Plugins.SQL.Data;
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
	public class BankReconciliationListingVM : BaseViewModel, IDisposable
	{
        private readonly IGetBankReconciliationListUseCase getReconciliationListUseCase;
        private readonly NotificationSystem notifications;

        public BankReconciliationListingVM(
			IGetBankReconciliationListUseCase getReconciliationListUseCase,
			NotificationSystem notifications)
		{
            this.getReconciliationListUseCase = getReconciliationListUseCase;
            this.notifications = notifications;
            this.notifications.BankReconciliationChanged += Notifications_BankReconciliationChanged;
			

			this.Load();
        }

        private void Notifications_BankReconciliationChanged(Guid bankAccountUID)
        {
			// Currently, we'll just reload the full listing
			this.Load();
        }


		public ObservableCollection<BankFileVM> BankReconciliationList { get; } = new ObservableCollection<BankFileVM>();

		private void Load()
		{
			this.BankReconciliationList.Clear();
			var listReconciliations = getReconciliationListUseCase.Execute();
			foreach(var record in listReconciliations.OrderBy(o => o.BankAccount.OrderBy))
			{
				BankFileVM vm = new BankFileVM();
				vm.LoadBankReconciliation(record);

				this.BankReconciliationList.Add(vm);
			}

		}


		public void Dispose()
		{
			this.notifications.BankReconciliationChanged -= Notifications_BankReconciliationChanged;
		}
	}
}
