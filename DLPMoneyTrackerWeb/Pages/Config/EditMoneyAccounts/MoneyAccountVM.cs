using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.TransactionModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTrackerWeb.Pages.Config.EditMoneyAccounts
{
    public class MoneyAccountVM
    {
        private readonly ITrackerConfig _config;
        private readonly IJournal _journal;        

        public MoneyAccountVM(ITrackerConfig config, IJournal journal) 
        {
            _config = config;
            _journal = journal;
        }


        public Guid UId { get; private set; }
        
        [MaxLength(100)]
        public string Description { get; set; }
        public JournalAccountType AccountType { get; set; }
        public decimal? InitialBalance { get; set; }
        public int DisplayOrder { get; set; }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Description)
                    && this.AccountType != JournalAccountType.NotSet
                    && this.DisplayOrder >= 0;
            }
        }


        public void LoadData(IJournalAccount account)
        {
            if(account is null)
            {
                this.Clear();
                return;
            }

            this.UId = account.Id;
            this.Description = account.Description;
            this.AccountType = account.JournalType;
            this.DisplayOrder = account.OrderBy;

            var recordInitialBalance = _journal.TransactionList.FirstOrDefault(x =>
                                (x.CreditAccountId == UId && x.DebitAccountId == SpecialAccount.InitialBalance.Id) ||
                                (x.CreditAccountId == SpecialAccount.InitialBalance.Id && x.DebitAccountId == UId));
            this.InitialBalance = recordInitialBalance?.TransactionAmount ?? decimal.Zero;
        }

        public void Clear()
        {
            this.UId = Guid.Empty;
            this.Description = string.Empty;
            this.AccountType = JournalAccountType.NotSet;
            this.InitialBalance = decimal.Zero;
            this.DisplayOrder = 0;
        }

        public void Save()
        {
            if (!this.IsValid) return;

            IJournalAccount account = null;
            if (this.UId != Guid.Empty)
            {
                account = _config.GetJournalAccount(this.UId);
            }
            
            if(account is null)
            {
                account = JournalAccountFactory.Build(this.Description, this.AccountType, orderBy: this.DisplayOrder);
                _config.AddJournalAccount(account);
            }
            else
            {
                JournalAccountFactory.Update(ref account, this.Description, orderBy: this.DisplayOrder);
            }
            _config.SaveJournalAccounts();


            var initBalRecord = (JournalEntry?)_journal.TransactionList
                .FirstOrDefault(x =>
                    (x.CreditAccountId == account.Id || x.DebitAccountId == account.Id) &&
                    (x.DebitAccountId == SpecialAccount.InitialBalance.Id || x.CreditAccountId == SpecialAccount.InitialBalance.Id)
                    );
            if (initBalRecord is null)
            {
                initBalRecord = new JournalEntry(_config)
                {
                    TransactionDate = DateTime.MinValue,
                    Description = "*INITIAL BALANCE*"
                };

                if (this.AccountType == JournalAccountType.Bank)
                {
                    initBalRecord.DebitAccount = account;
                    initBalRecord.CreditAccount = SpecialAccount.InitialBalance;
                }
                else
                {
                    initBalRecord.DebitAccount = SpecialAccount.InitialBalance;
                    initBalRecord.CreditAccount = account;
                }
            }
            initBalRecord.TransactionAmount = this.InitialBalance.HasValue ? this.InitialBalance.Value : decimal.Zero;
            _journal.AddTransaction(initBalRecord);
        }
    }
}
