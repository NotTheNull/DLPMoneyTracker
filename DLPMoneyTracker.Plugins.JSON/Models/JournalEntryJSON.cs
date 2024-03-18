using DLPMoneyTracker.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.JSON.Models
{

    internal class JournalEntryJSON
    {
        public Guid Id { get; set; }

        public DateTime TransactionDate { get; set; }
        public TransactionType JournalEntryType { get; set; } = TransactionType.NotSet;


        public Guid DebitAccountId { get; set; }
        public DateTime? DebitBankDate { get; set; }


        public Guid CreditAccountId { get; set; }
        public DateTime? CreditBankDate { get; set; }


        public string Description { get; set; }
        public decimal TransactionAmount { get; set; }
    }
}
