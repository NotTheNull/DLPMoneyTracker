using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.Main.ExpenseOverview
{
    public class MonthlyExpenseDetailVM : BaseViewModel
    {
        private readonly IBudgetPlan plan;
        private readonly IGetTransactionsBySearchUseCase searchTransactionUseCase;

        public MonthlyExpenseDetailVM(IBudgetPlan plan, IGetTransactionsBySearchUseCase searchTransactionUseCase) : base()
        {
            if (plan.Recurrence.Frequency != DLPMoneyTracker.Core.Models.ScheduleRecurrence.RecurrenceFrequency.Monthly) throw new ArgumentException("Budget plan must be Monthly");

            this.plan = plan;
            this.searchTransactionUseCase = searchTransactionUseCase;

            this.Load();
        }


        public string Name { get { return plan.Description; } }
        public decimal Amount { get { return plan.ExpectedAmount; } }
        public DateTime NextDueDate { get { return plan.NextOccurrence; } }


        public DateTime? January { get; set; }
        public DateTime? February { get; set; }
        public DateTime? March { get; set; }
        public DateTime? April { get; set; }
        public DateTime? May { get; set; }
        public DateTime? June { get; set; }
        public DateTime? July { get; set; }
        public DateTime? August { get; set; }
        public DateTime? September { get; set; }
        public DateTime? October { get; set; }
        public DateTime? November { get; set; }
        public DateTime? December { get; set; }




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

            this.Reset();
            foreach (var t in transactions.OrderBy(o => o.TransactionDate))
            {
                switch (t.TransactionDate.Month)
                {
                    case 1:
                        this.January = t.TransactionDate;
                        break;
                    case 2:
                        this.February = t.TransactionDate;
                        break;
                    case 3:
                        this.March = t.TransactionDate;
                        break;
                    case 4:
                        this.April = t.TransactionDate;
                        break;
                    case 5:
                        this.May = t.TransactionDate;
                        break;
                    case 6:
                        this.June = t.TransactionDate;
                        break;
                    case 7:
                        this.July = t.TransactionDate;
                        break;
                    case 8:
                        this.August = t.TransactionDate;
                        break;
                    case 9:
                        this.September = t.TransactionDate;
                        break;
                    case 10:
                        this.October = t.TransactionDate;
                        break;
                    case 11:
                        this.November = t.TransactionDate;
                        break;
                    case 12:
                        this.December = t.TransactionDate;
                        break;
                }
            }

            this.NotifyChanges();
        }

        private void Reset()
        {
            this.January = this.February = this.March = this.April = this.May = this.June = this.July = this.August = this.September = this.October = this.November = this.December = null;
        }

        private void NotifyChanges()
        {
            NotifyPropertyChanged(nameof(January));
            NotifyPropertyChanged(nameof(February));
            NotifyPropertyChanged(nameof(March));
            NotifyPropertyChanged(nameof(April));
            NotifyPropertyChanged(nameof(May));
            NotifyPropertyChanged(nameof(June));
            NotifyPropertyChanged(nameof(July));
            NotifyPropertyChanged(nameof(August));
            NotifyPropertyChanged(nameof(September));
            NotifyPropertyChanged(nameof(October));
            NotifyPropertyChanged(nameof(November));
            NotifyPropertyChanged(nameof(December));
            NotifyPropertyChanged(nameof(NextDueDate));
        }
    }
}
