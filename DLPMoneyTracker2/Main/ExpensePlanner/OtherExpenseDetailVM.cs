using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        

        public string Name { get { return plan.Description; } }
        public decimal Amount { get { return plan.ExpectedAmount; } }
        public DateTime NextDueDate { get { return plan.NextOccurrence; } }

        public DateTime? DatePaid { get; set; } = null;


        public bool HasAccount(Guid accountUID)
        {
            return plan.DebitAccountId == accountUID || plan.CreditAccountId == accountUID;
        }


        public void Load()
        {
            DateRange range = new DateRange()
            {
                Begin = new DateTime(DateTime.Today.Year, 1, 1),
                End = DateTime.Today
            };
            var transactions = searchTransactionUseCase.Execute(range, plan.Description, plan.DebitAccount);
            if(transactions.Any() == true)
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
