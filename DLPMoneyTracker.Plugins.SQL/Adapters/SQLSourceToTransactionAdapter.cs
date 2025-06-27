using DLPMoneyTracker.BusinessLogic.AdapterInterfaces;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.SQL.Data;

namespace DLPMoneyTracker.Plugins.SQL.Adapters
{
    public class SQLSourceToTransactionAdapter(DataContext context, ILedgerAccountRepository accountRepository) : ISourceToTransactionAdapter<TransactionBatch>
    {
        private readonly DataContext context = context;
        private readonly ILedgerAccountRepository accountRepository = accountRepository;

        public Guid UID { get; set; }

        public DateTime TransactionDate { get; set; }

        public TransactionType JournalEntryType { get; set; }

        public IJournalAccount DebitAccount { get; set; } = SpecialAccount.InvalidAccount;

        public Guid DebitAccountId => this.DebitAccount.Id;

        public string DebitAccountName => this.DebitAccount.Description;

        public DateTime? DebitBankDate { get; set; }

        public IJournalAccount CreditAccount { get; set; } = SpecialAccount.InvalidAccount;

        public Guid CreditAccountId => this.CreditAccount.Id;

        public string CreditAccountName => this.CreditAccount.Description;

        public DateTime? CreditBankDate { get; set; }

        public string Description { get; set; } = string.Empty;

        public decimal TransactionAmount { get; set; }

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
                var debit = src.Details.First(x => x.Amount > decimal.Zero);
                debit.Amount = this.TransactionAmount;
                debit.LedgerAccount = context.Accounts.FirstOrDefault(x => x.AccountUID == this.DebitAccountId);

                var credit = src.Details.First(x => x.Amount < decimal.Zero);
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
                    this.CreditAccount = accountRepository.GetAccountByUID(detail.LedgerAccount?.AccountUID ?? Guid.Empty);
                    this.CreditBankDate = detail.BankReconciliationDate;
                }
                else
                {
                    this.DebitAccount = accountRepository.GetAccountByUID(detail.LedgerAccount?.AccountUID ?? Guid.Empty);
                    this.DebitBankDate = detail.BankReconciliationDate;
                }
            }
        }
    }
}