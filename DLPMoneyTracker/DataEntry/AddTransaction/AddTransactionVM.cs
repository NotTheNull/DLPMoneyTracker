using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLPMoneyTracker.DataEntry.AddTransaction
{
    public class AddTransactionVM : BaseViewModel
    {
        private ILedger _ledger;
        private ITrackerConfig _config;








        public AddTransactionVM(ILedger ledger, ITrackerConfig config)
        {
            _ledger = ledger;
            _config = config;
        }

    }
}
