using AudioToolbox;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.TransactionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTrackerWeb.Data
{
    internal class EditMoneyAccountService
    {
        private readonly ITrackerConfig _config;
        private readonly IJournal _journal;
        public EditMoneyAccountService(ITrackerConfig config, IJournal journal)
        {
            _config = config;
            _journal = journal;
            this.ReloadAccounts();
        }


        private List<JournalAccountType> listMoneyAccountTypes = new List<JournalAccountType>()
        {
            JournalAccountType.Bank,
            JournalAccountType.LiabilityCard,
            JournalAccountType.LiabilityLoan
        };


        public IReadOnlyList<IJournalAccount> MoneyAccounts 
        { 
            get 
            { 
                return _config.LedgerAccountsList
                    .Where(x => listMoneyAccountTypes.Contains(x.JournalType))
                    .ToList()
                    .AsReadOnly(); 
            } 
        }

        public EditMoneyAccountRecord GetAccount(Guid id)
        {
            var account = this.MoneyAccounts.FirstOrDefault(x => x.Id == id);
            if (account is null) throw new InvalidOperationException(string.Format("Account #{0} not found", id));

            EditMoneyAccountRecord editThis = new EditMoneyAccountRecord(account);
            var initBalRecord = _journal.TransactionList
                .FirstOrDefault(x =>
                    (x.CreditAccountId == id || x.DebitAccountId == id) &&
                    (x.DebitAccountId == SpecialAccount.InitialBalance.Id || x.CreditAccountId == SpecialAccount.InitialBalance.Id)
                    );
            editThis.InitialBalance = initBalRecord?.TransactionAmount ?? decimal.Zero;

            return editThis;
        }

        public void SaveAccount(EditMoneyAccountRecord account)
        {
            if (account is null) throw new ArgumentNullException("Money Account");
            if (!listMoneyAccountTypes.Contains(account.JournalType)) throw new InvalidOperationException(string.Format("JType [{0}] is not a valid money type", account.JournalType.ToString()));

            var money = _config.LedgerAccountsList.FirstOrDefault(x => x.Id == account.Id);
            if(money is null)
            {
                money = JournalAccountFactory.Build(account.Description, account.JournalType, orderBy: account.DisplayOrder);
                _config.AddJournalAccount(money);
            }
            else
            {
                JournalAccountFactory.Update(ref money, account.Description, orderBy: account.DisplayOrder);
            }
            _config.SaveJournalAccounts();


            var initBalRecord = (JournalEntry?)_journal.TransactionList
                .FirstOrDefault(x =>
                    (x.CreditAccountId == money.Id || x.DebitAccountId == money.Id) &&
                    (x.DebitAccountId == SpecialAccount.InitialBalance.Id || x.CreditAccountId == SpecialAccount.InitialBalance.Id)
                    );
            if (initBalRecord is null)
            {
                initBalRecord = new JournalEntry(_config)
                {
                    TransactionDate = DateTime.MinValue,
                    Description = "*INITIAL BALANCE*"
                };

                if (money.JournalType == JournalAccountType.Bank)
                {
                    initBalRecord.DebitAccount = money;
                    initBalRecord.CreditAccount = SpecialAccount.InitialBalance;
                }
                else
                {
                    initBalRecord.DebitAccount = SpecialAccount.InitialBalance;
                    initBalRecord.CreditAccount = money;
                }
            }
            initBalRecord.TransactionAmount = account.InitialBalance;
            _journal.AddTransaction(initBalRecord);

        }

        public void ReloadAccounts()
        {
            _config.LoadJournalAccounts();
        }

        public void DeleteAccount(Guid id) 
        {
            _config.RemoveJournalAccount(id);
        }
                
    }

    public class EditMoneyAccountRecord
    {
        public EditMoneyAccountRecord()
        {
            Id = Guid.Empty;
        }
        public EditMoneyAccountRecord(IJournalAccount cpy)
        {
            this.Id = cpy.Id;
            this.JournalType = cpy.JournalType;
            this.Description = cpy.Description;
            this.DisplayOrder = cpy.OrderBy;
        }

        public Guid Id { get; set; }
        public JournalAccountType JournalType { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public decimal InitialBalance { get; set; }

    }
}
