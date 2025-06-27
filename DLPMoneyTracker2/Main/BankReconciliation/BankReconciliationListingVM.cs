using DLPMoneyTracker.BusinessLogic.UseCases.BankReconciliation.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;

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

        ~BankReconciliationListingVM()
        {
            this.Dispose();
        }

        private void Notifications_BankReconciliationChanged(Guid bankAccountUID)
        {
            // Currently, we'll just reload the full listing
            this.Load();
        }

        public ObservableCollection<BankFileVM> BankReconciliationList { get; } = [];

        private void Load()
        {
            this.BankReconciliationList.Clear();
            var listReconciliations = getReconciliationListUseCase.Execute();
            foreach (var record in listReconciliations.OrderBy(o => o.BankAccount.OrderBy))
            {
                BankFileVM vm = new();
                vm.LoadBankReconciliation(record);

                this.BankReconciliationList.Add(vm);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.notifications.BankReconciliationChanged -= Notifications_BankReconciliationChanged;
        }
    }
}