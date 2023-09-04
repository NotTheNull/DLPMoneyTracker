using DLPMoneyTracker.Data.TransactionModels.JournalPlan;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.Main.UpcomingReminders
{
    public class BillDetailVM : BaseViewModel
    {
        private readonly IJournalPlan _plan;

        public BillDetailVM(IJournalPlan plan)
        {
            _plan = plan;
        }

        public DateTime DateDue
        {
            get
            {
                return _plan.NextOccurrence;
            }
        }

        public string Description
        {
            get { return _plan.Description; }
        }

        public decimal Amount
        {
            get { return _plan.ExpectedAmount; }
        }
    }
}
