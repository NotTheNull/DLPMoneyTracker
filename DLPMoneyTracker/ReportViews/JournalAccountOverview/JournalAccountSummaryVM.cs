﻿using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.ReportViews.JournalAccountOverview
{
    public class JournalAccountSummaryVM : BaseViewModel
    {
        private readonly ITrackerConfig _config;
        private readonly ILedgerAccount _account;
        private readonly IJournal _journal;


        public JournalAccountSummaryVM(ILedgerAccount account, IJournal journal, ITrackerConfig config) : base()
        {
            
        }
    }
}
