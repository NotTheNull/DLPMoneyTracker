using DLPMoneyTracker.BusinessLogic.AdapterInterfaces;
using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.SQL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.SQL.Adapters
{
    public class SQLSourceToTransactionAdapter : ISourceToTransactionAdapter<TransactionBatch>
    {
        private readonly DataContext context;

        public SQLSourceToTransactionAdapter(DataContext context)
        {
            this.context = context;
        }

        public Guid UID { get; set; }

        public DateTime TransactionDate { get; set; }

        public TransactionType JournalEntryType { get; set; }

        public IJournalAccount DebitAccount { get; set; }

        public Guid DebitAccountId { get { return this.DebitAccount.Id; } }

        public string DebitAccountName { get { return this.DebitAccount.Description; } }

        public DateTime? DebitBankDate { get; set; }

        public IJournalAccount CreditAccount { get; set; }

        public Guid CreditAccountId { get { return this.CreditAccount.Id; } }

        public string CreditAccountName { get { return this.CreditAccount.Description; } }

        public DateTime? CreditBankDate { get; set; }

        public string Description { get; set; }

        public decimal TransactionAmount { get; set; }


        public void Copy(IMoneyTransaction transaction)
        {
            ArgumentNullException.ThrowIfNull(transaction);

            this.UID = transaction.UID;
            this.TransactionDate = transaction.TransactionDate;
            this.JournalEntryType = transaction.JournalEntryType;
            this.DebitAccount = transaction.DebitAccount;
            this.DebitBankDate = transaction.DebitBankDate;
            this.CreditAccount = transaction.CreditAccount;
            this.CreditBankDate = transaction.CreditBankDate;
            this.Description = transaction.Description;
            this.TransactionAmount = transaction.TransactionAmount;
        }

        public void ExportSource(ref TransactionBatch src)
        {
            ArgumentNullException.ThrowIfNull(src);

            src.BatchUID = this.UID;
            src.BatchType = this.JournalEntryType;
            src.TransactionDate = this.TransactionDate;
            src.Description = this.Description;
            
            if(src.Details.Any() != true)
            {
                src.Details.Add(new TransactionDetail()
                {
                    Amount = this.TransactionAmount,
                    LedgerAccount = context.Accounts.FirstOrDefault(x => x.AccountUID == this.DebitAccountId)
                });

                src.Details.Add(new TransactionDetail()
                {
                    Amount = this.TransactionAmount * -1,
                    LedgerAccount = context.Accounts.FirstOrDefault(x => x.AccountUID == this.CreditAccountId)
                });
            }
            else
            {
                var debit = src.Details.FirstOrDefault(x => x.Amount > decimal.Zero);
                debit.Amount = this.TransactionAmount;
                debit.LedgerAccount = context.Accounts.FirstOrDefault(x => x.AccountUID == this.DebitAccountId);

                var credit = src.Details.FirstOrDefault(x => x.Amount < decimal.Zero);
                credit.Amount = this.TransactionAmount * -1;
                credit.LedgerAccount = context.Accounts.FirstOrDefault(x => x.AccountUID == this.CreditAccountId);
            }
        }

        public void ImportSource(TransactionBatch src)
        {
            ArgumentNullException.ThrowIfNull(src);

            this.UID = src.BatchUID;
            this.TransactionDate = src.TransactionDate;
            this.JournalEntryType = src.BatchType;
            this.Description = src.Description;
            
            foreach(var detail in src.Details)
            {
                if(detail.Amount < decimal.Zero)
                {
                    this.CreditAccount = SourceToAccount(detail.LedgerAccount);
                    this.CreditBankDate = detail.BankReconciliationDate;
                }
                else
                {
                    this.DebitAccount = SourceToAccount(detail.LedgerAccount);
                    this.DebitBankDate = detail.BankReconciliationDate;
                }
            }           
        }

        private IJournalAccount SourceToAccount(Account src)
        {
            SQLSourceToJournalAccountAdapter adapter = new SQLSourceToJournalAccountAdapter();
            JournalAccountFactory factory = new JournalAccountFactory();

            adapter.ImportSource(src);
            return factory.Build(adapter);
        }
    }
}
