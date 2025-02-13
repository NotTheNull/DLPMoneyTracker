using BlazorBootstrap;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using Microsoft.AspNetCore.Components;

namespace MoneyTrackerWebApp.Models.AccountDetail
{
    public class DetailBase : ComponentBase
    {
        [Parameter] public Guid AccountId { get; set; }
        [Inject] public IGetJournalAccountByUIDUseCase ActionGetAccount { get; set; }

        [Inject] public IGetJournalAccountCurrentMonthBalanceUseCase ActionGetBalance { get; set; }


        protected IMoneyAccount account;
        protected decimal currBalance;
        protected DateRange filterDate = new DateRange(new DateTime(DateTime.Today.Year, 1, 1), new DateTime(DateTime.Today.Year, 12, 31));


        protected override void OnParametersSet()
        {
            var a = ActionGetAccount.Execute(this.AccountId);
            if (a is IMoneyAccount money)
            {
                account = money;
                currBalance = ActionGetBalance.Execute(AccountId);
            }
        }



        protected void OnFilterDateBeginChanged(ChangeEventArgs e)
        {
            if (DateTime.TryParse(e.Value.ToString(), out DateTime begin))
            {
                filterDate.Begin = begin;
            }
        }

        protected void OnFilterDateEndChanged(ChangeEventArgs e)
        {
            if (DateTime.TryParse(e.Value.ToString(), out DateTime end))
            {
                filterDate.End = end;
            }
        }

        

    }
}
