using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker2.Core;
using System;
using System.Linq;

namespace DLPMoneyTracker2.Main.ExpensePlanner
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

            Load();
        }

        public string Name => plan.Description;
        public decimal Amount => plan.ExpectedAmount;
        public DateTime NextDueDate => plan.NextOccurrence;

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
            DateRange range = new()
            {
                Begin = new DateTime(DateTime.Today.Year, 1, 1),
                End = DateTime.Today
            };
            var transactions = searchTransactionUseCase.Execute(range, plan.Description, plan.DebitAccount);

            Reset();
            foreach (var t in transactions.OrderBy(o => o.TransactionDate))
            {
                switch (t.TransactionDate.Month)
                {
                    case 1:
                        January = t.TransactionDate;
                        break;

                    case 2:
                        February = t.TransactionDate;
                        break;

                    case 3:
                        March = t.TransactionDate;
                        break;

                    case 4:
                        April = t.TransactionDate;
                        break;

                    case 5:
                        May = t.TransactionDate;
                        break;

                    case 6:
                        June = t.TransactionDate;
                        break;

                    case 7:
                        July = t.TransactionDate;
                        break;

                    case 8:
                        August = t.TransactionDate;
                        break;

                    case 9:
                        September = t.TransactionDate;
                        break;

                    case 10:
                        October = t.TransactionDate;
                        break;

                    case 11:
                        November = t.TransactionDate;
                        break;

                    case 12:
                        December = t.TransactionDate;
                        break;
                }
            }

            NotifyChanges();
        }

        private void Reset()
        {
            January = February = March = April = May = June = July = August = September = October = November = December = null;
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