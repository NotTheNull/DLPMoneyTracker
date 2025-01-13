﻿using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.SQL.Data;
using Microsoft.AspNetCore.Components;
using MoneyTrackerWebApp.Models.Core;
using MoneyTrackerWebApp.Services;

namespace MoneyTrackerWebApp.Models.Config.MoneyAccounts
{
    public class EditMoneyAccountBase : ComponentBase, IEditComponentBase
    {
        [Parameter]
        public Guid? AccountUID { get; set; }

        [Inject]
        public IJournalAccountService AccountService { get; set; }

        [Inject]
        public StorageService<IJournalAccount> Storage { get; set; }

        [Inject]
        public INavigationHistoryService Navigation { get; set; }

        [Inject]
        public ILogger<EditMoneyAccountBase> Logger { get; set; }


        protected readonly LedgerType[] listLedgerTypes = [LedgerType.Bank, LedgerType.LiabilityCard, LedgerType.LiabilityLoan];
        protected readonly LedgerType[] listCSVMapTypes = [LedgerType.Bank, LedgerType.LiabilityCard];
        protected EditMoneyAccountVM Account { get; set; } = new EditMoneyAccountVM();
        protected ICSVMapping Mapping => Account.Mapping;


        private readonly string URL_MONEYLIST = "/config/moneyaccounts";

        protected override void OnParametersSet()
        {
            Logger.LogInformation("Setting parameters");
            if (AccountUID is null || AccountUID == Guid.Empty) return;

            Logger.LogInformation($"Loading account with UID {AccountUID}");
            var acct = AccountService.GetAccount(AccountUID.Value);
            Storage.Data = acct;
            Account.Copy(acct);
        }

        protected override void OnInitialized()
        {
            Logger.LogInformation("Initializing");
            if (Storage.Data == null) return;

            Logger.LogInformation("Copying account information from storage");
            Account.Copy(Storage.Data);
        }


        #region Form Events


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
            if (int.TryParse(e.Value.ToString(), out int newRow))
            {
                Account.StartingRow = newRow;
            }
        }

        protected void OnTransDateColumnChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value.ToString(), out int newCol))
            {
                Account.TransDateColumn = newCol;
            }
        }

        protected void OnDescriptionColumnChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value.ToString(), out int newCol))
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
            if (bool.TryParse(e.Value.ToString(), out bool isChecked))
            {
                Account.IsAmountInverted = isChecked;
            }
        }
        #endregion

        public void SaveChanges()
        {
            AccountService.SaveAccount(Account);
            ReturnToList();
        }

        public void Reset()
        {
            if (Storage.Data != null)
            {
                Account.Copy(Storage.Data);
            }
            else
            {
                // If this is a NEW record, discarding means going back to the listing
                ReturnToList();
            }
        }

        public void ReturnToList()
        {
            Storage.Data = null;
            Navigation.NavigateBack(URL_MONEYLIST);
        }
    }


}
