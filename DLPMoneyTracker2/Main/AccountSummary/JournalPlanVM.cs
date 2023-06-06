﻿using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.TransactionModels.JournalPlan;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.Main.AccountSummary
{
    public class JournalPlanVM : BaseViewModel
    {
        private readonly IJournalPlan _plan;
        private readonly IJournalAccount _actParent;

        public JournalPlanVM(IJournalAccount act, IJournalPlan plan)
        {
            _plan = plan;
            _actParent = act;
        }

        private Guid ParentAccountId { get { return _actParent.Id; } }
        public Guid PlanUID { get { return _plan.UID; } }

        public bool IsParentDebit { get { return _plan.DebitAccountId == this.ParentAccountId; } }
        public string PlanAccountDescription
        {
            get
            {
                if (_plan.CreditAccountId == this.ParentAccountId) return _plan.DebitAccountName;
                return _plan.CreditAccountName;
            }
        }

        public JournalPlanType PlanType { get { return _plan.PlanType; } }
        public string PlanTypeDescription { get { return this.PlanType.ToString(); } }

        public decimal Amount { get { return _plan.ExpectedAmount; } }

        public DateTime NextDueDate { get { return _plan.NotificationDate; } }



    }
}