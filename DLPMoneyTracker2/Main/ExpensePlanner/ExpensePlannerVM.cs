using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker2.Core;
using DLPMoneyTracker2.Main.ExpenseOverview;
using DLPMoneyTracker2.Main.ExpensePlanner;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.Main.ExpenseDetail
{
    public class ExpensePlannerVM : BaseViewModel, IDisposable
    {
        private readonly IGetBudgetPlanListUseCase getPlanListUseCase;
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
            this.Load();
        }

        private void Notification_TransactionsModified(Guid debitAccountId, Guid creditAccountId)
        {
            var listMonthly = this.MonthlyExpenseList.Where(x => x.HasAccount(debitAccountId) || x.HasAccount(creditAccountId));
            foreach(var m in listMonthly)
            {
                m.Load();
            }

            var listOther = this.OtherExpenseList.Where(x => x.HasAccount(debitAccountId) || x.HasAccount(creditAccountId));
            foreach(var o in listOther)
            {
                o.Load();
            }
        }

        public ObservableCollection<MonthlyExpenseDetailVM> MonthlyExpenseList { get; } = new ObservableCollection<MonthlyExpenseDetailVM>();

        public ObservableCollection<OtherExpenseDetailVM> OtherExpenseList { get; } = new ObservableCollection<OtherExpenseDetailVM>();


        public void Load()
        {
            var listPayablePlan = getBudgetListByTypeUseCase.Execute(BudgetPlanType.Payable);
            var listDebtPlan = getBudgetListByTypeUseCase.Execute(BudgetPlanType.DebtPayment);

            List<IBudgetPlan> listPlans = new List<IBudgetPlan>();
            if(listPayablePlan.Any() == true) listPlans.AddRange(listPayablePlan);
            if(listDebtPlan.Any() == true) listPlans.AddRange(listDebtPlan);

            this.MonthlyExpenseList.Clear();
            this.OtherExpenseList.Clear();
            foreach(var plan in listPlans)
            {
                if(plan.Recurrence.Frequency == DLPMoneyTracker.Core.Models.ScheduleRecurrence.RecurrenceFrequency.Monthly)
                {
                    this.MonthlyExpenseList.Add(new MonthlyExpenseDetailVM(plan, searchTransactionUseCase));
                }
                else
                {
                    this.OtherExpenseList.Add(new OtherExpenseDetailVM(plan, searchTransactionUseCase));
                }
            }
            
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            
            this.MonthlyExpenseList.Clear();
            this.OtherExpenseList.Clear();

            this.notification.TransactionsModified -= Notification_TransactionsModified;
        }        

    }
}
