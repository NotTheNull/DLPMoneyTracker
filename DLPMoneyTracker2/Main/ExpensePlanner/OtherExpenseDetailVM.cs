using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker2.Core;
using System;
using System.Linq;

namespace DLPMoneyTracker2.Main.ExpensePlanner
{
    public class OtherExpenseDetailVM : BaseViewModel
    {
        private readonly IBudgetPlan plan;
        private readonly IGetTransactionsBySearchUseCase searchTransactionUseCase;

        public OtherExpenseDetailVM(IBudgetPlan plan,
            IGetTransactionsBySearchUseCase searchTransactionUseCase)
            : base()
        {
            if (plan.Recurrence.Frequency == DLPMoneyTracker.Core.Models.ScheduleRecurrence.RecurrenceFrequency.Monthly) throw new ArgumentException("Budget plan must be non-Monthly");

            this.plan = plan;
            this.searchTransactionUseCase = searchTransactionUseCase;
            this.Load();
        }

        public string Name => plan.Description;
        public decimal Amount => plan.ExpectedAmount;
        public DateTime NextDueDate => plan.NextOccurrence;

        public DateTime? DatePaid { get; set; }

        public bool HasAccount(Guid accountUID)
        {
            return plan.DebitAccountId == accountUID || plan.CreditAccountId == accountUID;
        }

        public void Load()
        {
            DateRange range = new()
            {
                Begin = new DateTime(DateTime.Today.Year, 1, 1),
                End = DateTime.Today
            };
            var transactions = searchTransactionUseCase.Execute(range, plan.Description, plan.DebitAccount);
            if (transactions.Any() == true)
            {
                this.DatePaid = transactions.Last().TransactionDate;
            }
            else
            {
                this.DatePaid = null;
            }

            this.NotifyChanges();
        }

        private void NotifyChanges()
        {
            NotifyPropertyChanged(nameof(DatePaid));
            NotifyPropertyChanged(nameof(NextDueDate));
        }
    }
}