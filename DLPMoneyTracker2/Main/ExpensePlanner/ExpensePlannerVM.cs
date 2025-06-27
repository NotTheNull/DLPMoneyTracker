using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DLPMoneyTracker2.Main.ExpensePlanner
{
    public class ExpensePlannerVM : BaseViewModel, IDisposable
    {
        private readonly IGetTransactionsBySearchUseCase searchTransactionUseCase;
        private readonly IGetBudgetPlanListByType getBudgetListByTypeUseCase;
        private readonly NotificationSystem notification;

        public ExpensePlannerVM(IGetTransactionsBySearchUseCase searchTransactionUseCase,
            IGetBudgetPlanListByType getBudgetListByTypeUseCase,
            NotificationSystem notification)
        {
            this.searchTransactionUseCase = searchTransactionUseCase;
            this.getBudgetListByTypeUseCase = getBudgetListByTypeUseCase;
            this.notification = notification;

            this.notification.TransactionsModified += Notification_TransactionsModified;
            Load();
        }

        private void Notification_TransactionsModified(Guid debitAccountId, Guid creditAccountId)
        {
            var listMonthly = MonthlyExpenseList.Where(x => x.HasAccount(debitAccountId) || x.HasAccount(creditAccountId));
            foreach (var m in listMonthly)
            {
                m.Load();
            }

            var listOther = OtherExpenseList.Where(x => x.HasAccount(debitAccountId) || x.HasAccount(creditAccountId));
            foreach (var o in listOther)
            {
                o.Load();
            }
        }

        public ObservableCollection<MonthlyExpenseDetailVM> MonthlyExpenseList { get; } = [];
        public ObservableCollection<OtherExpenseDetailVM> OtherExpenseList { get; } = [];

        public void Load()
        {
            var listPayablePlan = getBudgetListByTypeUseCase.Execute(BudgetPlanType.Payable);
            var listDebtPlan = getBudgetListByTypeUseCase.Execute(BudgetPlanType.DebtPayment);

            List<IBudgetPlan> listPlans = new List<IBudgetPlan>();
            if (listPayablePlan.Any() == true) listPlans.AddRange(listPayablePlan);
            if (listDebtPlan.Any() == true) listPlans.AddRange(listDebtPlan);

            MonthlyExpenseList.Clear();
            OtherExpenseList.Clear();
            foreach (var plan in listPlans)
            {
                if (plan.Recurrence.Frequency == DLPMoneyTracker.Core.Models.ScheduleRecurrence.RecurrenceFrequency.Monthly)
                {
                    MonthlyExpenseList.Add(new MonthlyExpenseDetailVM(plan, searchTransactionUseCase));
                }
                else
                {
                    OtherExpenseList.Add(new OtherExpenseDetailVM(plan, searchTransactionUseCase));
                }
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            MonthlyExpenseList.Clear();
            OtherExpenseList.Clear();

            notification.TransactionsModified -= Notification_TransactionsModified;
        }
    }
}