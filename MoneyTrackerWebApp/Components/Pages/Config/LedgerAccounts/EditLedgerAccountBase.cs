using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using Microsoft.AspNetCore.Components;
using MoneyTrackerWebApp.Components.Pages.Config.MoneyAccounts;
using MoneyTrackerWebApp.Models.Config.LedgerAccounts;
using MoneyTrackerWebApp.Models.Core;
using MoneyTrackerWebApp.Services;

namespace MoneyTrackerWebApp.Components.Pages.Config.LedgerAccounts
{
    public class EditLedgerAccountBase : ComponentBase
    {
        [Parameter]
        public Guid? AccountUID { get; set; }

        [Inject]
        public IJournalAccountService AccountService { get; set; }

        [Inject]
        public IGetSummaryAccountListByType GetSummaryAccounts { get; set; }

        [Inject]
        public StorageService<IJournalAccount>  Storage { get; set; }

        [Inject]
        public INavigationHistoryService Navigation { get; set; }

        [Inject]
        public ILogger<EditMoneyAccountBase> Logger { get; set; }

        protected List<IJournalAccount> listSummaryAccounts = new List<IJournalAccount>();
        protected readonly BudgetTrackingType[] listBudgetTypes = [BudgetTrackingType.DO_NOT_TRACK, BudgetTrackingType.Fixed, BudgetTrackingType.Variable];
        protected readonly LedgerType[] listLedgerTypes = [LedgerType.Payable, LedgerType.Receivable];
        protected EditLedgerAccountVM Account { get; set; } = new EditLedgerAccountVM();

        private readonly string URL_ACCOUNTLIST = "/config/ledgeraccounts";

        protected override void OnParametersSet()
        {
            Logger.LogInformation("Setting parameters");
            if (AccountUID is null || AccountUID == Guid.Empty) return;

            Logger.LogInformation($"Loading account with UID {this.AccountUID}");
            var acct = AccountService.GetAccount(this.AccountUID.Value);
            this.Storage.Data = acct;
            this.Account.Copy(acct);
        }

        protected override void OnInitialized()
        {
            Logger.LogInformation("Initializing");
            if (this.Storage.Data == null) return;

            Logger.LogInformation("Copying account information from storage");
            this.Account.Copy(this.Storage.Data);

        }

        #region Form Events


        protected void OnLedgerTypeChanged(ChangeEventArgs e)
        {
            Account.JournalType = (LedgerType)e.Value;

            var list = GetSummaryAccounts.Execute(Account.JournalType);
            listSummaryAccounts.Clear();
            
            if(list?.Any() == true)
            {
                listSummaryAccounts.AddRange(list);
            }
        }

        protected void OnDescriptionChanged(ChangeEventArgs e)
        {
            Account.Description = e.Value.ToString();
        }

        protected void OnBudgetTypeChanged(ChangeEventArgs e)
        {
            Account.BudgetType = (BudgetTrackingType)e.Value;
        }

        protected void OnDefaultBudgetAmountChanged(ChangeEventArgs e)
        {
            Account.DefaultMonthlyBudgetAmount = e.Value?.ToString().ToDecimal() ?? decimal.Zero;
        }

        protected void OnSummaryAccountIdChanged(ChangeEventArgs e)
        {
            Guid uid = Guid.Parse(e.Value.ToString());
            Account.SummaryAccountId = uid == Guid.Empty ? null : uid;
            
        }

        #endregion


        protected void SaveChanges()
        {
            AccountService.SaveAccount(Account);
            this.ReturnToList();
        }

        protected void Reset()
        {
            if(Storage.Data != null)
            {
                Account.Copy(Storage.Data);
            } else
            {
                this.ReturnToList();
            }
        }

        protected void ReturnToList()
        {
            this.Storage.Data = null;
            Navigation.NavigateBack(URL_ACCOUNTLIST);
        }


    }
}
