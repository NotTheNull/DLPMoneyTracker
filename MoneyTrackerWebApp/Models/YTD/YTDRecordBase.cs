using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using Microsoft.AspNetCore.Components;

namespace MoneyTrackerWebApp.Models.YTD
{
    public class YTDRecordBase : ComponentBase
    {
        [Parameter]
        public INominalAccount Account { get; set; }

        [Parameter]
        public string ClassOverride { get; set; } = "";


        [Inject]
        public IGetJournalAccountBalanceByMonthUseCase ActionGetMonthBalance { get; set; }

        [Inject]
        public IGetJournalAccountYTDUseCase ActionGetYTDBalance { get; set; }


        protected decimal YTDBalance { get; set; } = decimal.Zero;
        protected List<MonthTotal> Months { get; set; } = new List<MonthTotal>();




        protected override void OnParametersSet()
        {
            if (Account is null) return;

            this.YTDBalance = ActionGetYTDBalance.Execute(Account.Id, DateTime.Today.Year);
            this.FillMonthTotals();
        }

        private void FillMonthTotals()
        {
            for(int i = 0; i < 12; i++)
            {
                DateTime date = DateTime.Today.AddMonths(i * -1);
                decimal total = ActionGetMonthBalance.Execute(Account.Id, date.Year, date.Month);
                this.Months.Add(new MonthTotal()
                {
                    AccountUID = Account.Id,
                    Month = date.Month,
                    Year = date.Year,
                    Balance = total
                });
            }
        }
    }

    public class MonthTotal
    {
        public Guid AccountUID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Balance { get; set; }

        public string Id { get { return $"M_{Year}_{Month}"; } }
    }

}
