using DLPMoneyTracker.BusinessLogic.AdapterInterfaces;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.JSON.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.JSON.Adapters
{
    internal class JSONSourceToJournalAccountAdapter : ISourceToJournalAccountAdapter<JournalAccountJSON>
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public LedgerType JournalType { get; set; }

        public int OrderBy { get; set; }

        public DateTime? DateClosedUTC { get; set; }

        public BudgetTrackingType BudgetType { get; set; } = BudgetTrackingType.DO_NOT_TRACK;
        public decimal DefaultMonthlyBudgetAmount { get; set; } = decimal.Zero;
        public decimal CurrentBudgetAmount { get; set; } = decimal.Zero;

        public ICSVMapping Mapping { get; set; } = null;

        public void Copy(IJournalAccount cpy)
        {
            ArgumentNullException.ThrowIfNull(cpy);

            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.JournalType = cpy.JournalType;
            this.OrderBy = cpy.OrderBy;
            this.DateClosedUTC = cpy.DateClosedUTC;

            if(this.Mapping != null)
            {
                this.Mapping.Dispose();
                this.Mapping = null;
            }
            if (cpy is INominalAccount nominal)
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

        public void ExportSource(ref JournalAccountJSON acct)
        {
            ArgumentNullException.ThrowIfNull(acct);

            acct.Id = this.Id;
            acct.Description = this.Description;
            acct.JournalType = this.JournalType;
            acct.OrderBy = this.OrderBy;
            acct.DateClosedUTC = this.DateClosedUTC;
            acct.BudgetType = this.BudgetType;
            acct.DefaultMonthlyBudgetAmount = this.DefaultMonthlyBudgetAmount;
            acct.CurrentBudgetAmount = this.CurrentBudgetAmount;

            acct.Mapping = null;
            if(this.Mapping != null)
            {
                acct.Mapping = new CSVMapping();
                acct.Mapping.Copy(this.Mapping);
            }
        }


        public void ImportSource(JournalAccountJSON acct)
        {
            ArgumentNullException.ThrowIfNull(acct);

            this.Id = acct.Id;
            this.Description = acct.Description;
            this.JournalType = acct.JournalType;
            this.OrderBy = acct.OrderBy;
            this.DateClosedUTC = acct.DateClosedUTC;
            this.BudgetType = acct.BudgetType;
            this.DefaultMonthlyBudgetAmount = acct.DefaultMonthlyBudgetAmount;
            this.CurrentBudgetAmount = acct.CurrentBudgetAmount;

            if (this.Mapping != null)
            {
                this.Mapping.Dispose();
                this.Mapping = null;
            }

            if (acct.Mapping != null)
            {
                this.Mapping = new CSVMapping();
                this.Mapping.Copy(acct.Mapping);
            }
        }
    }
}
