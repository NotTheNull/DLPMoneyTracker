﻿using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.TransactionModels.JournalPlan
{
    public sealed class PayablePlan : IJournalPlan
    {
        public PayablePlan()
        {
            UID = Guid.NewGuid();
        }

        public Guid UID { get; set; }

        public IJournalAccount DebitAccount { get; set; }
        public Guid DebitAccountId { get { return DebitAccount?.Id ?? Guid.Empty; } }

        public string DebitAccountName { get { return DebitAccount?.Description ?? string.Empty; } }

        public IJournalAccount CreditAccount { get; set; }
        public Guid CreditAccountId { get { return CreditAccount?.Id ?? Guid.Empty; } }

        public string CreditAccountName { get { return CreditAccount?.Description ?? string.Empty; } }

        public JournalPlanType PlanType { get { return JournalPlanType.Payable; } }

        public int PriorityOrder { get { return 2; } }

        public string Description { get; set; }


        [JsonIgnore]
        public IScheduleRecurrence Recurrence { get; set; }

        public string RecurrenceJSON
        {
            get { return this.Recurrence.GetFileData(); }
            set
            {
                this.Recurrence = ScheduleRecurrenceFactory.Build(value);
            }
        }

        [JsonIgnore]
        public RecurrenceFrequency Frequency { get { return this.Recurrence.Frequency; } }

        [JsonIgnore]
        public DateTime NotificationDate { get { return this.Recurrence?.NotificationDate.AddDays(1).AddMilliseconds(-1) ?? DateTime.MinValue; } }

        [JsonIgnore]
        public DateTime NextOccurrence { get { return this.Recurrence?.NextOccurence ?? DateTime.MaxValue; } }

        public decimal ExpectedAmount { get; set; }

        /// <summary>
        /// Verifies that the plan is valid for Payables
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if (DebitAccount.JournalType == JounalAccountType.Bank || DebitAccount.JournalType == JounalAccountType.Receivable) return false;
            if (CreditAccount.JournalType != JounalAccountType.Bank || CreditAccount.JournalType != JounalAccountType.LiabilityCard) return false;
            if (ExpectedAmount <= decimal.Zero) return false;

            return true;
        }
    }
}
