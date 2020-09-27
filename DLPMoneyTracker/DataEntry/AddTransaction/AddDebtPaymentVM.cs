using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLPMoneyTracker.DataEntry.AddTransaction
{
    class AddDebtPaymentVM : BaseViewModel
    {
        private ILedger _ledger;
        private ITrackerConfig _config;


        public AddDebtPaymentVM(ILedger ledger, ITrackerConfig config)
        {
            _ledger = ledger;
            _config = config;
        }

    }
}
