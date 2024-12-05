using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.SQL.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client.Extensions.Msal;
using MoneyTrackerWebApp.Models.Config.MoneyAccounts;
using MoneyTrackerWebApp.Models.Core;
using MoneyTrackerWebApp.Services;

namespace MoneyTrackerWebApp.Components.Pages.Config.MoneyAccounts
{
    public class EditMoneyAccountBase : ComponentBase
    {

        [Inject]
        public IJournalAccountService  AccountService { get; set; }

        [Inject]
        public StorageService<IJournalAccount> Storage { get; set; }

        [Inject]
        public INavigationHistoryService Navigation { get; set; }


        protected readonly LedgerType[] listLedgerTypes = [LedgerType.Bank, LedgerType.LiabilityCard, LedgerType.LiabilityLoan];
        protected EditMoneyAccountVM Account { get; set; } = new EditMoneyAccountVM();
        protected ICSVMapping Mapping => Account.Mapping;


        private readonly string URL_MONEYLIST = "/config/moneyaccounts";

        protected void OnLedgerTypeChanged(ChangeEventArgs e)
        {
            Account.JournalType = (LedgerType)e.Value;
        }

        protected void OnDescriptionChanged(ChangeEventArgs e)
        {
            Account.Description = e.Value.ToString();
        }

        protected void OnDisplayOrderChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value.ToString(), out int newOrder))
            {
                Account.OrderBy = newOrder;
            }
        }

        protected void OnStartingRowChanged(ChangeEventArgs e)
        {
            if(int.TryParse(e.Value.ToString(), out int newRow))
            {
                Account.StartingRow = newRow;
            }
        }

        protected void OnTransDateColumnChanged(ChangeEventArgs e)
        {
            if(int.TryParse(e.Value.ToString(), out int newCol))
            {
                Account.TransDateColumn = newCol;
            }
        }

        protected void OnDescriptionColumnChanged(ChangeEventArgs e)
        {
            if(int.TryParse(e.Value.ToString(), out int newCol))
            {
                Account.DescriptionColumn = newCol;
            }
        }
        protected void OnAmountColumnChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value.ToString(), out int newCol))
            {
                Account.AmountColumn = newCol;
            }
        }

        protected void OnIsAmountInvertedChanged(ChangeEventArgs e)
        {
            if(bool.TryParse(e.Value.ToString(), out bool isChecked))
            {
                Account.IsAmountInverted = isChecked;
            }
        }


        protected void SaveChanges()
        {
            AccountService.SaveAccount(Account);
            Navigation.NavigateBack(URL_MONEYLIST);
        }

        protected void Reset()
        {
            if (Storage.Data != null)
            {
                Account.Copy(Storage.Data);
            }
            else
            {
                // If this is a NEW record, discarding means going back to the listing
                Navigation.NavigateBack(URL_MONEYLIST);
            }
        }
    }


}
