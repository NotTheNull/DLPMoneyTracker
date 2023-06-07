using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.TransactionModels.JournalPlan
{
    public enum JournalPlanType
    {
        Payable,
        Receivable,
        Transfer,
        DebtPayment,
        NotSet
    }


    public interface IJournalPlan
    {
        Guid UID { get; }

        Guid DebitAccountId { get; }

        [JsonIgnore]
        string DebitAccountName { get; }

        Guid CreditAccountId { get; }

        [JsonIgnore]
        string CreditAccountName { get; }

        [JsonIgnore]
        JournalPlanType PlanType { get; }

        [JsonIgnore]
        int PriorityOrder { get; } // Mainly for sorting Income before Expenses

        string Description { get; }


        string RecurrenceJSON { get; }

        [JsonIgnore]
        RecurrenceFrequency Frequency { get; }

        decimal ExpectedAmount { get; }

        [JsonIgnore]
        DateTime NotificationDate { get; }

        [JsonIgnore]
        DateTime NextOccurrence { get; }

        
        bool IsValid();
    }

    public sealed class JournalPlanJSON : IJournalPlan
    {
        public Guid UID { get; set; }

        public Guid DebitAccountId { get; set; }

        [JsonIgnore]
        public string DebitAccountName { get; set; }

        public Guid CreditAccountId { get; set; }

        [JsonIgnore]
        public string CreditAccountName { get; set; }

        
        public JournalPlanType PlanType { get; set; }

        public int PriorityOrder { get { return 9999999; } }

        public string Description { get; set; }

        public string RecurrenceJSON { get; set; }
        
        public decimal ExpectedAmount { get; set; }

        [JsonIgnore]
        public RecurrenceFrequency Frequency { get { return RecurrenceFrequency.Annual; } }


        [JsonIgnore]
        public DateTime NotificationDate { get { return DateTime.MinValue; } }

        [JsonIgnore]
        public DateTime NextOccurrence { get; set; }


        public bool IsValid() { return true; }
    }
}
