
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker2.Core;
using System;

namespace DLPMoneyTracker2.Main.UpcomingReminders
{
    public class BillDetailVM(IBudgetPlan plan) : BaseViewModel
    {
        private readonly IBudgetPlan _plan = plan;

        public DateTime DateDue => _plan?.NextOccurrence ?? DateTime.MinValue;
        public string DisplayDate => string.Format("{0:yyyy/MM/dd}", this.DateDue);
        public string Description => _plan?.Description ?? string.Empty; 
        public decimal Amount => _plan?.ExpectedAmount ?? decimal.Zero; 
    }
}