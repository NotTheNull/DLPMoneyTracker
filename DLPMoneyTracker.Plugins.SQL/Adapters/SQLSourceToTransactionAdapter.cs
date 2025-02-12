using DLPMoneyTracker.BusinessLogic.AdapterInterfaces;
using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.SQL.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.SQL.Adapters
{
    public class SQLSourceToTransactionAdapter : ISourceToTransactionAdapter<TransactionBatch>
    {
        private readonly DataContext context;
        private readonly ILedgerAccountRepository accountRepository;

        public SQLSourceToTransactionAdapter(DataContext context, ILedgerAccountRepository accountRepository)
        {
            this.context = context;
            this.accountRepository = accountRepository;
        }


        private SingleAccountTransaction[] _records = { new SingleAccountTransaction(), new SingleAccountTransaction() };
        public SingleAccountTransaction[] Records { get { return _records; } }


        public Guid UID
        {
            get { return _records[0].TransactionUID; }
            set
            {
                _records[0].TransactionUID = value;
                _records[1].TransactionUID = value;
            }
        }
        public DateTime TransactionDate
        {
            get { return _records[0].TransactionDate; }
            set
            {
                _records[0].TransactionDate = value;
                _records[1].TransactionDate = value;
            }
        }
        public TransactionType JournalEntryType
        {
            get { return _records[0].JournalEntryType; }
            set
            {
                _records[0].JournalEntryType = value;
                _records[1].JournalEntryType = value;
            }
        }
        public string Description
        {
            get { return _records[0].Description; }
            set
            {
                _records[0].Description = value;
                _records[1].Description = value;
            }
        }
        public decimal TransactionAmount
        {
            get { return _records[0].TransactionAmount; }
            set
            {
                _records[0].TransactionAmount = value;
                _records[1].TransactionAmount = value * -1;
            }
        }


        public IJournalAccount DebitAccount { get { return _records[0].Account; } set { _records[0].Account = value; } }
        public Guid DebitAccountId { get { return DebitAccount?.Id ?? Guid.Empty; } }
        public string DebitAccountName { get { return DebitAccount?.Description ?? string.Empty; } }
        public DateTime? DebitBankDate { get { return _records[0].BankDate; } set { _records[0].BankDate = value; } }


        public IJournalAccount CreditAccount { get { return _records[1].Account; } set { _records[1].Account = value; } }
        public Guid CreditAccountId { get { return CreditAccount?.Id ?? Guid.Empty; } }
        public string CreditAccountName { get { return CreditAccount?.Description ?? string.Empty; } }
        public DateTime? CreditBankDate { get { return _records[1].BankDate; } set { _records[1].BankDate = value; } }




        public void Copy(IMoneyTransaction transaction)
        {
            ArgumentNullException.ThrowIfNull(transaction);

            this.UID = transaction.UID;
            this.TransactionDate = transaction.TransactionDate < Common.MINIMUM_DATE ? Common.MINIMUM_DATE : transaction.TransactionDate;
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

            if (src.Details.Any() != true)
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

            foreach (var detail in src.Details)
            {
                if (detail.Amount < decimal.Zero)
                {
                    this.CreditAccount = accountRepository.GetAccountByUID(detail.LedgerAccount.AccountUID);
                    this.CreditBankDate = detail.BankReconciliationDate;
                }
                else
                {
                    this.DebitAccount = accountRepository.GetAccountByUID(detail.LedgerAccount.AccountUID);
                    this.DebitBankDate = detail.BankReconciliationDate;
                }
            }
        }

    }
}
