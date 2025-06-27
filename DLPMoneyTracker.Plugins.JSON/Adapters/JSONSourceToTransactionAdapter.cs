using DLPMoneyTracker.BusinessLogic.AdapterInterfaces;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.JSON.Models;

namespace DLPMoneyTracker.Plugins.JSON.Adapters
{
    internal class JSONSourceToTransactionAdapter(ILedgerAccountRepository accountRepository) : ISourceToTransactionAdapter<JournalEntryJSON>
    {
        private readonly ILedgerAccountRepository accountRepository = accountRepository;

        public Guid UID { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType JournalEntryType { get; set; } = TransactionType.NotSet;
        public string Description { get; set; } = string.Empty;
        public decimal TransactionAmount { get; set; }

        private IJournalAccount? _debit;
        public IJournalAccount DebitAccount { get { return _debit ?? SpecialAccount.InvalidAccount; } set { _debit = value; } }
        public Guid DebitAccountId => DebitAccount?.Id ?? Guid.Empty;
        public string DebitAccountName => DebitAccount?.Description ?? string.Empty;
        public DateTime? DebitBankDate { get; set; }

        private IJournalAccount? _credit;
        public IJournalAccount CreditAccount { get { return _credit ?? SpecialAccount.InvalidAccount; } set { _credit = value; } }
        public Guid CreditAccountId => CreditAccount?.Id ?? Guid.Empty;
        public string CreditAccountName => CreditAccount?.Description ?? string.Empty;
        public DateTime? CreditBankDate { get; set; }

        public void ExportSource(ref JournalEntryJSON acct)
        {
            ArgumentNullException.ThrowIfNull(acct);

            acct.Id = this.UID;
            acct.TransactionDate = this.TransactionDate;
            acct.JournalEntryType = this.JournalEntryType;
            acct.Description = this.Description;
            acct.TransactionAmount = this.TransactionAmount;
            acct.DebitAccountId = this.DebitAccountId;
            acct.DebitBankDate = this.DebitBankDate;
            acct.CreditAccountId = this.CreditAccountId;
            acct.CreditBankDate = this.CreditBankDate;
        }

        public void Copy(IMoneyTransaction transaction)
        {
            ArgumentNullException.ThrowIfNull(transaction);

            this.UID = transaction.UID;
            this.TransactionDate = transaction.TransactionDate;
            this.JournalEntryType = transaction.JournalEntryType;
            this.Description = transaction.Description;
            this.TransactionAmount = transaction.TransactionAmount;
            this.DebitBankDate = transaction.DebitBankDate;
            this.CreditBankDate = transaction.CreditBankDate;

            this.DebitAccount = accountRepository.GetAccountByUID(transaction.DebitAccountId);
            this.CreditAccount = accountRepository.GetAccountByUID(transaction.CreditAccountId);
        }

        public void ImportSource(JournalEntryJSON acct)
        {
            ArgumentNullException.ThrowIfNull(acct);

            this.UID = acct.Id;
            this.TransactionDate = acct.TransactionDate;
            this.JournalEntryType = acct.JournalEntryType;
            this.Description = acct.Description;
            this.TransactionAmount = acct.TransactionAmount;
            this.DebitBankDate = acct.DebitBankDate;
            this.CreditBankDate = acct.CreditBankDate;

            if (acct.DebitAccountId == Guid.Empty)
            {
                this.DebitAccount = SpecialAccount.InitialBalance;
                this.DebitBankDate = Common.MINIMUM_DATE;
            }
            else
            {
                this.DebitAccount = accountRepository.GetAccountByUID(acct.DebitAccountId);
            }

            if (acct.CreditAccountId == Guid.Empty)
            {
                this.CreditAccount = SpecialAccount.InitialBalance;
                this.CreditBankDate = Common.MINIMUM_DATE;
            }
            else
            {
                this.CreditAccount = accountRepository.GetAccountByUID(acct.CreditAccountId);
            }
        }
    }
}