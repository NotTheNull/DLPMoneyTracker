using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DLPMoneyTracker.Data.TransactionModels.JournalPlan
{
    public sealed class ReceivablePlan : IJournalPlan
    {
        public ReceivablePlan()
        {
            UID = Guid.NewGuid();
        }

        private List<LedgerType> _validDebits = new List<LedgerType>()
        {
            LedgerType.Bank
        };

        public List<LedgerType> ValidDebitAccountTypes
        { get { return _validDebits; } }

        private List<LedgerType> _validCredits = new List<LedgerType>()
        {
            LedgerType.Receivable
        };

        public List<LedgerType> ValidCreditAccountTypes
        { get { return _validCredits; } }

        public Guid UID { get; set; }

        public IJournalAccount DebitAccount { get; set; }

        public Guid DebitAccountId
        { get { return DebitAccount?.Id ?? Guid.Empty; } }

        public string DebitAccountName
        { get { return DebitAccount?.Description ?? string.Empty; } }

        public IJournalAccount CreditAccount { get; set; }

        public Guid CreditAccountId
        { get { return CreditAccount?.Id ?? Guid.Empty; } }

        public string CreditAccountName
        { get { return CreditAccount?.Description ?? string.Empty; } }

        public JournalPlanType PlanType
        { get { return JournalPlanType.Receivable; } }

        public int PriorityOrder
        { get { return 1; } }

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
        public RecurrenceFrequency Frequency
        { get { return this.Recurrence.Frequency; } }

        [JsonIgnore]
        public DateTime NotificationDate
        { get { return this.Recurrence?.NotificationDate.AddDays(1).AddMilliseconds(-1) ?? DateTime.MinValue; } }

        [JsonIgnore]
        public DateTime NextOccurrence
        { get { return this.Recurrence?.NextOccurence ?? DateTime.MaxValue; } }

        public decimal ExpectedAmount { get; set; }

        /// <summary>
        /// Verifies that the account setup is valid for a Receivable Plan.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if (this.DebitAccount is null || this.CreditAccount is null) return false;
            if (!this.ValidDebitAccountTypes.Contains(DebitAccount.JournalType)) return false;
            if (!this.ValidCreditAccountTypes.Contains(CreditAccount.JournalType)) return false;
            if (string.IsNullOrWhiteSpace(this.Description)) return false;
            if (ExpectedAmount <= decimal.Zero) return false;

            return true;
        }
    }
}