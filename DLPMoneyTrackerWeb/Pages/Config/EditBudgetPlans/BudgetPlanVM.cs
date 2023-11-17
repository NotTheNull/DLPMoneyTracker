using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using DLPMoneyTracker.Data.TransactionModels.JournalPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTrackerWeb.Pages.Config.EditBudgetPlans
{
    internal class BudgetPlanVM 
    {
        private readonly ITrackerConfig _config;
        private readonly IJournalPlanner _planner;

        public BudgetPlanVM(ITrackerConfig config, IJournalPlanner planner)
        {
            _config = config;
            _planner = planner;            
        }

        public Guid UID { get; set; }

        public JournalPlanType SelectedPlanType { get; set; }
        public string Description { get; set; }

        public Guid SelectedDebitAccountUID { get; set; }
        public Guid SelectedCreditAccountUID { get; set; }
        public decimal ExpectedAmount { get; set; }

        public IScheduleRecurrence Recurrence { get; set; }

        private bool IsValid
        {
            get
            {
                if (this.SelectedCreditAccountUID == Guid.Empty) return false;
                if (this.SelectedDebitAccountUID == Guid.Empty) return false;
                if (this.SelectedPlanType == JournalPlanType.NotSet) return false;
                if (string.IsNullOrWhiteSpace(this.Description)) return false;
                if (this.ExpectedAmount == decimal.Zero) return false;
                if (this.Recurrence is null) return false;

                return true;
            }
        }


        public void Clear()
        {
            this.UID = Guid.Empty;
            this.SelectedDebitAccountUID = Guid.Empty;
            this.SelectedCreditAccountUID = Guid.Empty;
            this.Description = string.Empty;
            this.SelectedPlanType = JournalPlanType.NotSet;
            this.ExpectedAmount = decimal.Zero;
            this.Recurrence = ScheduleRecurrenceFactory.Build(RecurrenceFrequency.Annual, DateTime.Today);
        }



        public void LoadData(IJournalPlan plan)
        {
            if(plan is null)
            {
                this.Clear();
                return;
            }

            this.UID = plan.UID;
            this.Description = plan.Description;
            this.SelectedCreditAccountUID = plan.CreditAccountId;
            this.SelectedDebitAccountUID = plan.DebitAccountId;
            this.ExpectedAmount = plan.ExpectedAmount;

            if (string.IsNullOrWhiteSpace(plan.RecurrenceJSON))
            {
                this.Recurrence = ScheduleRecurrenceFactory.Build(RecurrenceFrequency.Annual, DateTime.Today);
            }
            else
            {
                this.Recurrence = ScheduleRecurrenceFactory.Build(plan.RecurrenceJSON);
            }
        }


        public void Save()
        {
            if (!this.IsValid) return;

            IJournalPlan plan = null;
            if(this.UID != Guid.Empty)
            {
                plan = _planner.GetPlan(this.UID);
            }

            IJournalAccount creditAccount = _config.GetJournalAccount(this.SelectedCreditAccountUID);
            IJournalAccount debitAccount = _config.GetJournalAccount(this.SelectedDebitAccountUID);

            if(plan is null)
            {
                plan = JournalPlanFactory.Build(this.SelectedPlanType, this.Description, creditAccount, debitAccount, this.ExpectedAmount, this.Recurrence);
                _planner.AddPlan(plan);
            }
            else
            {
                JournalPlanFactory.Update(ref plan, this.Description, creditAccount, debitAccount, this.ExpectedAmount, this.Recurrence);
            }
            _planner.SaveToFile();
        }

    }
}
