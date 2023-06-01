using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    [Obsolete("Only needed for Conversion")]
    public interface IMoneyAccountReference
    {
        Guid Id { get; }
        string Description { get; set; }
        int OrderBy { get; set; }
        DateTime? DateClosedUTC { get; set; }
        string MoneyAccountId { get; }
        MoneyAccountType AccountType { get; }

        void Convert(MoneyAccount act);

    }
}
