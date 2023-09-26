using DLPMoneyTracker.Data.TransactionModels.JournalPlan;
using DLPMoneyTracker2.Core;
using System;

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
                return _plan?.NextOccurrence ?? DateTime.MinValue;
            }
        }

        public string DisplayDate
        {
            get { return string.Format("{0:yyyy/MM/dd}", this.DateDue); }
        }

        public string Description
        {
            get { return _plan?.Description ?? string.Empty; }
        }

        public decimal Amount
        {
            get { return _plan?.ExpectedAmount ?? decimal.Zero; }
        }
    }
}