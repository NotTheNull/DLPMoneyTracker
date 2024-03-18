using DLPMoneyTracker.BusinessLogic.AdapterInterfaces;
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
        public Guid Id { get; set; }

        public string Description { get; set; }

        public LedgerType JournalType { get; set; }

        public int OrderBy { get; set; }

        public DateTime? DateClosedUTC { get; set; }

        public void Copy(IJournalAccount cpy)
        {
            ArgumentNullException.ThrowIfNull(cpy);

            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.JournalType = cpy.JournalType;
            this.OrderBy = cpy.OrderBy;
            this.DateClosedUTC = cpy.DateClosedUTC;
        }

        public void ExportSource(ref Account acct)
        {
            ArgumentNullException.ThrowIfNull(acct);

            acct.AccountUID = this.Id;
            acct.Description = this.Description;
            acct.AccountType = this.JournalType;
            acct.MainTabSortingId = this.OrderBy;
            acct.DateClosedUTC = this.DateClosedUTC;
        }

        public void ImportSource(Account acct)
        {
            ArgumentNullException.ThrowIfNull(acct);

            this.Id = acct.AccountUID;
            this.Description = acct.Description;
            this.JournalType = acct.AccountType;
            this.OrderBy = acct.MainTabSortingId;
            this.DateClosedUTC = acct.DateClosedUTC;
        }
    }
}
