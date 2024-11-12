using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTrackerBlaze.Models.Config.MoneyAccounts
{
    internal class EditMoneyAccountVM : IMoneyAccount
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        public string Description { get; set; } = string.Empty;

        public LedgerType JournalType { get; set; } = LedgerType.NotSet;

        public int OrderBy { get; set; } = 0;

        public DateTime? DateClosedUTC { get; set; } = null;
        public ICSVMapping Mapping { get; set; } = new CSVMapping();

        public void Copy(IJournalAccount cpy)
        {
            if (cpy is null) return;

            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.JournalType = cpy.JournalType;
            this.OrderBy = cpy.OrderBy;
            this.DateClosedUTC = cpy.DateClosedUTC;
            
            if(cpy is IMoneyAccount money)
            {
                this.Mapping.Copy(money.Mapping);
            }
        }
    }
}
