﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.Models
{
    public class ConversionDTO
    {
        public int JournalAccountCount { get; set; } = 0;
        public int BudgetPlanCount { get; set; } = 0;
        public int BankReconciliationCount { get; set; } = 0;
        public long TransactionCount { get; set; } = 0;
    }
}