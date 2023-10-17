using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTrackerWeb.Pages.Config.EditLedgerAccounts
{
    public class LedgerAccountVM
    {
        private readonly ITrackerConfig _config;

        public LedgerAccountVM(ITrackerConfig config)
        {
            _config = config;             
        }

        public Guid UId { get; private set; }
        public string Description { get; set; }
        public JournalAccountType AccountType { get; set; }
        public decimal? MonthlyBudget { get; set; }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Description)
                    && this.AccountType != JournalAccountType.NotSet;
            }
        }


        public void LoadData(IJournalAccount account)
        {
            if(account is null)
            {
                this.Clear();
                return;
            }

            this.UId = account.Id;
            this.Description = account.Description;
            this.AccountType = account.JournalType;
            this.MonthlyBudget = account.MonthlyBudgetAmount;
        }

        public void Clear()
        {
            this.UId = Guid.Empty;
            this.Description = string.Empty;
            this.AccountType = JournalAccountType.NotSet;
            this.MonthlyBudget = decimal.Zero;
        }

        public void Save()
        {
            if (!this.IsValid) return;

            IJournalAccount account = null;
            if (this.UId != Guid.Empty)
            {
                account = _config.GetJournalAccount(this.UId);
            }

            if (account is null)
            {
                account = JournalAccountFactory.Build(this.Description, this.AccountType, this.MonthlyBudget.Value);
                _config.AddJournalAccount(account);
            }
            else
            {
                JournalAccountFactory.Update(ref account, this.Description, this.MonthlyBudget.Value);
            }
            _config.SaveJournalAccounts();
        }

    }
}
