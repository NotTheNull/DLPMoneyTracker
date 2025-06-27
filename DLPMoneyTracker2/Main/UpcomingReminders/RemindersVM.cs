

using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DLPMoneyTracker2.Main.UpcomingReminders
{
    public class RemindersVM : BaseViewModel
    {
        private readonly IGetBudgetPlanListByDateRangeUseCase getPlansByDatesUseCase;
        private readonly IGetJournalAccountByUIDUseCase getAccountByUIDUseCase;
        private readonly IFindTransactionForBudgetPlanUseCase findTransactionByPlanUseCase;
        private readonly NotificationSystem notifications;

        public RemindersVM(
            IGetBudgetPlanListByDateRangeUseCase getPlansByDatesUseCase,
            IGetJournalAccountByUIDUseCase getAccountByUIDUseCase,
            IFindTransactionForBudgetPlanUseCase findTransactionByPlanUseCase,
            NotificationSystem notifications)
        {
            this.getPlansByDatesUseCase = getPlansByDatesUseCase;
            this.getAccountByUIDUseCase = getAccountByUIDUseCase;
            this.findTransactionByPlanUseCase = findTransactionByPlanUseCase;
            this.notifications = notifications;
            this.notifications.TransactionsModified += Notifications_TransactionsModified;
        }

        private void Notifications_TransactionsModified(Guid debitAccountId, Guid creditAccountId)
        {
            this.Load();
        }

        private readonly ObservableCollection<BillDetailVM> _listBills = [];
        public ObservableCollection<BillDetailVM> RemindersList => _listBills;

        public void Load()
        {
            this.RemindersList.Clear();

            DateRange range = new(DateTime.Today.AddDays(-7), DateTime.Today.AddDays(30));
            var listPlans = getPlansByDatesUseCase.Execute(range);
            if (listPlans?.Any() != true) return;

            foreach (var plan in listPlans.OrderBy(o => o.NextOccurrence))
            {
                // See if we already have a transaction for this plan; either account Id should be sufficient
                var account = getAccountByUIDUseCase.Execute(plan.CreditAccountId);
                var transactions = findTransactionByPlanUseCase.Execute(plan, account);
                if (transactions != null) continue;

                this.RemindersList.Add(new BillDetailVM(plan));
            }
        }
    }
}