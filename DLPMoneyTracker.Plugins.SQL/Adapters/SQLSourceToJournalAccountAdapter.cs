using DLPMoneyTracker.BusinessLogic.AdapterInterfaces;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.SQL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.SQL.Adapters
{
    public class SQLSourceToJournalAccountAdapter : ISourceToJournalAccountAdapter<Account>
    {
        private readonly ILedgerAccountRepository accountRepository;

        public SQLSourceToJournalAccountAdapter(ILedgerAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public Guid Id { get; set; }

        public string Description { get; set; }

        public LedgerType JournalType { get; set; }

        public int OrderBy { get; set; }

        public DateTime? DateClosedUTC { get; set; }

        public BudgetTrackingType BudgetType { get; set; } = BudgetTrackingType.DO_NOT_TRACK;
        public decimal DefaultMonthlyBudgetAmount { get; set; } = decimal.Zero;
        public decimal CurrentBudgetAmount { get; set; } = decimal.Zero;

        public IJournalAccount SummaryAccount { get; set; }

        public ICSVMapping Mapping { get; set; } = null;

        public void Copy(IJournalAccount cpy)
        {
            ArgumentNullException.ThrowIfNull(cpy);

            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.JournalType = cpy.JournalType;
            this.OrderBy = cpy.OrderBy;
            this.DateClosedUTC = cpy.DateClosedUTC;
            
            if(cpy is INominalAccount nominal)
            {
                this.BudgetType = nominal.BudgetType;
                this.DefaultMonthlyBudgetAmount = nominal.DefaultMonthlyBudgetAmount;
                this.CurrentBudgetAmount = nominal.CurrentBudgetAmount;
            }
            else if (cpy is IMoneyAccount money)
            {
                if (this.Mapping is null) this.Mapping = new CSVMapping();

                this.Mapping.Copy(money.Mapping);
            }
        }

        // TODO: Finish integration of CSVMapping into SQL Database
        public void ExportSource(ref Account acct)
        {
            ArgumentNullException.ThrowIfNull(acct);

            acct.AccountUID = this.Id;
            acct.Description = this.Description;
            acct.AccountType = this.JournalType;
            acct.MainTabSortingId = this.OrderBy;
            acct.DateClosedUTC = this.DateClosedUTC;
            acct.BudgetType = this.BudgetType;
            acct.DefaultBudget = this.DefaultMonthlyBudgetAmount;
            acct.CurrentBudget = this.CurrentBudgetAmount;

        }

        public void ImportSource(Account acct)
        {
            ArgumentNullException.ThrowIfNull(acct);

            this.Id = acct.AccountUID;
            this.Description = acct.Description;
            this.JournalType = acct.AccountType;
            this.OrderBy = acct.MainTabSortingId;
            this.DateClosedUTC = acct.DateClosedUTC;
            this.BudgetType = acct.BudgetType;
            this.DefaultMonthlyBudgetAmount = acct.DefaultBudget;
            this.CurrentBudgetAmount = acct.CurrentBudget;
        }


    }
}
