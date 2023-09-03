using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLPMoneyTracker2.Core;
using DLPMoneyTracker.Data.TransactionModels;

namespace DLPMoneyTracker2.Config.AddEditMoneyAccounts
{
    public class MoneyAccountVM : BaseViewModel
    {
        private readonly ITrackerConfig _config;
        private readonly IJournal _journal;
        
        private static readonly List<JournalAccountType> _listValidTypes = new List<JournalAccountType>() { JournalAccountType.Bank, JournalAccountType.LiabilityCard, JournalAccountType.LiabilityLoan };
        public static List<JournalAccountType> ValidTypes { get { return _listValidTypes; } }


        public MoneyAccountVM(ITrackerConfig config, IJournal journal) : base()
        {
            _config = config;
            _journal = journal;
            this.Clear();
        }
        public MoneyAccountVM(ITrackerConfig config, IJournal journal, IJournalAccount act) : base()
        {
            _config = config;
            _journal = journal;
            this.LoadAccount(act);
        }


        public Guid Id { get; private set; }

        private string _desc = string.Empty;

        public string Description
        {
            get { return _desc; }
            set
            {
                _desc = value;
                NotifyPropertyChanged(nameof(Description));
            }
        }

        private JournalAccountType _acctType;

        public JournalAccountType JournalType
        {
            get { return _acctType; }
            set
            {
                _acctType = value;
                NotifyPropertyChanged(nameof(JournalType));
            }
        }

        private decimal _initBal;

        public decimal InitialBalance
        {
            get { return _initBal; }
            set
            {
                _initBal = value;
                NotifyPropertyChanged(nameof(InitialBalance));
            }
        }

        private DateTime? _closeDateUTC;

        public DateTime? DateClosedUTC
        {
            get { return _closeDateUTC; }
            set 
            {
                _closeDateUTC = value; 
                NotifyPropertyChanged(nameof(DateClosedUTC));
            }
        }

        private int _order;

        public int DisplayOrder
        {
            get { return _order; }
            set 
            { 
                _order = value; 
                NotifyPropertyChanged(nameof(DisplayOrder));
            }
        }



        public void Clear()
        {
            Id = Guid.Empty;
            Description = string.Empty;
            InitialBalance = decimal.Zero;
            JournalType = JournalAccountType.NotSet;
            this.DateClosedUTC = null;
        }


        public void LoadAccount(IJournalAccount account)
        {
            if (account is null) throw new ArgumentNullException("Money Account");
            if (!_listValidTypes.Contains(account.JournalType)) throw new InvalidCastException(string.Format("[{0} - {1}] is not a valid Money Account", account.JournalType.ToString(), account.Description));

            Id = account.Id;
            Description = account.Description;
            JournalType = account.JournalType;
            DateClosedUTC = account.DateClosedUTC;
            DisplayOrder = account.OrderBy;

            var initBalRecord = _journal.TransactionList
                .FirstOrDefault(x =>
                    (x.CreditAccountId == Id || x.DebitAccountId == Id) &&
                    (x.DebitAccountId == SpecialAccount.InitialBalance.Id || x.CreditAccountId == SpecialAccount.InitialBalance.Id)
                    );
            this.InitialBalance = initBalRecord?.TransactionAmount ?? decimal.Zero;
        }

        public void SaveAccount()
        {
            if (string.IsNullOrWhiteSpace(_desc)) return;
            if (JournalType == JournalAccountType.NotSet) return;


            IJournalAccount? acct = null;
            if(this.Id != Guid.Empty) acct = _config.LedgerAccountsList.FirstOrDefault(x => x.Id == this.Id);

            if (acct is null)
            {
                acct = JournalAccountFactory.Build(this.Description, this.JournalType, orderBy: this.DisplayOrder);
                _config.AddJournalAccount(acct);
            }
            else 
            {
                JournalAccountFactory.Update(ref acct, this.Description, orderBy: this.DisplayOrder);
            }
            _config.SaveJournalAccounts();

            var initBalRecord = (JournalEntry?)_journal.TransactionList
                .FirstOrDefault(x =>
                    (x.CreditAccountId == Id || x.DebitAccountId == Id) &&
                    (x.DebitAccountId == SpecialAccount.InitialBalance.Id || x.CreditAccountId == SpecialAccount.InitialBalance.Id)
                    );
            if (initBalRecord is null)
            {
                initBalRecord = new JournalEntry(_config)
                {
                    TransactionDate = DateTime.MinValue,
                    Description = "*INITIAL BALANCE*"
                };

                if (this.JournalType == JournalAccountType.Bank)
                {
                    initBalRecord.DebitAccount = acct;
                    initBalRecord.CreditAccount = SpecialAccount.InitialBalance;
                }
                else
                {
                    initBalRecord.DebitAccount = SpecialAccount.InitialBalance;
                    initBalRecord.CreditAccount = acct;
                }
            }
            initBalRecord.TransactionAmount = this.InitialBalance;
            _journal.AddTransaction(initBalRecord);

        }
    }
}
