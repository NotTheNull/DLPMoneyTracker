using DLPMoneyTracker.BusinessLogic.AdapterInterfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Core.Models.Source;
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

        public void Copy(IJournalAccount cpy)
        {
            ArgumentNullException.ThrowIfNull(cpy);

            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.JournalType = cpy.JournalType;
            this.OrderBy = cpy.OrderBy;
            this.DateClosedUTC = cpy.DateClosedUTC;
        }

        public void ExportSource(ref JournalAccountJSON acct)
        {
            ArgumentNullException.ThrowIfNull(acct);

            acct.Id = this.Id;
            acct.Description = this.Description;
            acct.JournalType = this.JournalType;
            acct.OrderBy = this.OrderBy;
            acct.DateClosedUTC = this.DateClosedUTC;

            // All other fields in the JSON are lost in this conversion as I am rethinking them
        }
                

        public void ImportSource(JournalAccountJSON acct)
        {
            ArgumentNullException.ThrowIfNull(acct);

            this.Id = acct.Id;
            this.Description = acct.Description;
            this.JournalType = acct.JournalType;
            this.OrderBy = acct.OrderBy;
            this.DateClosedUTC = acct.DateClosedUTC;
        }
    }
}
