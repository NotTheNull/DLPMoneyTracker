using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public interface IMoneyAccountReference
    {
        Guid Id { get; }
        string Description { get; set; }
        int OrderBy { get; set; }
        DateTime? DateClosedUTC { get; set; }
        string MoneyAccountId { get; }
        MoneyAccountType AccountType { get; }

#pragma warning disable CS0612 // Type or member is obsolete
        void Convert(MoneyAccount act);
#pragma warning restore CS0612 // Type or member is obsolete
    }
}
