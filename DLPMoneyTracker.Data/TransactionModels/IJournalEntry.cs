using DLPMoneyTracker.Data.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.TransactionModels
{
    public interface IJournalEntry
    {
        Guid Id { get; }
        DateTime TransactionDate { get; }
        Guid DebitAccountId { get; } 
        Guid CreditAccountId { get; }
        string Description { get; }
        decimal TransactionAmount { get; }

        void Copy(IJournalEntry cpy);
    }

    public class JournalEntryJSON : IJournalEntry
    {

        public Guid Id { get; set; }

        public DateTime TransactionDate { get; set; }

        public Guid DebitAccountId { get; set; } 

        public Guid CreditAccountId { get; set; }

        public string Description { get; set; }

        public decimal TransactionAmount { get; set; }

        public void Copy(IJournalEntry cpy)
        {
            Id = cpy.Id;
            TransactionDate = cpy.TransactionDate;
            DebitAccountId = cpy.CreditAccountId;
            CreditAccountId = cpy.CreditAccountId;
            Description = cpy.Description;
            TransactionAmount = cpy.TransactionAmount;  
        }
    }

    public class JournalEntry : IJournalEntry
    {
        private readonly ITrackerConfig _config;

        public JournalEntry(ITrackerConfig config)
        {
            this.Id = Guid.NewGuid();
            this.TransactionDate = DateTime.Now;
            this.Description = string.Empty;
            this.TransactionAmount = decimal.Zero;
            _config = config;
        }
        public JournalEntry(ITrackerConfig config, IJournalEntry cpy)
        {
            _config = config;
            this.Copy(cpy);
        }

        public Guid Id { get; set; }
        public DateTime TransactionDate { get; set; }

        public ILedgerAccount DebitAccount { get; set; }
        public Guid DebitAccountId { get { return DebitAccount.Id; } }
        public ILedgerAccount CreditAccount { get; set; }
        public Guid CreditAccountId { get { return CreditAccount.Id; } }

        public string Description { get; set; }
        public decimal TransactionAmount { get; set; }

        public void Copy(IJournalEntry cpy)
        {
            if(cpy is JournalEntryJSON json)
            {
                this.Id = json.Id;
                this.TransactionDate = json.TransactionDate;
                this.Description = json.Description;
                this.TransactionAmount = json.TransactionAmount;

                this.DebitAccount = _config.LedgerAccountsList.FirstOrDefault(x => x.Id == json.DebitAccountId);
                this.CreditAccount = _config.LedgerAccountsList.FirstOrDefault(x => x.Id == json.CreditAccountId);
            }
            else if(cpy is JournalEntry je)
            {
                this.Id = je.Id;
                this.TransactionDate= je.TransactionDate;
                this.Description = je.Description;
                this.TransactionAmount = je.TransactionAmount;
                this.DebitAccount = je.DebitAccount;
                this.CreditAccount = je.CreditAccount;
            }
        }
    }
}
