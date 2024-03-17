using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.TransactionModels;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DLPMoneyTracker2.Config.AddEditMoneyAccounts
{
    public class MoneyAccountVM : BaseViewModel
    {
        private readonly ITrackerConfig _config;
        private readonly IJournal _journal;

        private static readonly List<LedgerType> _listValidTypes = new List<LedgerType>() { LedgerType.Bank, LedgerType.LiabilityCard, LedgerType.LiabilityLoan };

        public static List<LedgerType> ValidTypes
        { get { return _listValidTypes; } }

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

        private LedgerType _acctType;

        public LedgerType JournalType
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
                NotifyPropertyChanged(nameof(IsClosed));
                NotifyPropertyChanged(nameof(DisplayClosedMessage));
            }
        }

        public bool IsClosed
        { get { return _closeDateUTC.HasValue; } }

        public string DisplayClosedMessage
        {
            get
            {
                if (this.IsClosed) return string.Format("CLOSED: {0}", _closeDateUTC.Value.ToLocalTime());

                return string.Empty;
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
            JournalType = LedgerType.NotSet;
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
            if (JournalType == LedgerType.NotSet) return;

            IJournalAccount? acct = null;
            if (this.Id != Guid.Empty) acct = _config.GetJournalAccount(this.Id);

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
                    (x.CreditAccountId == acct.Id || x.DebitAccountId == acct.Id) &&
                    (x.DebitAccountId == SpecialAccount.InitialBalance.Id || x.CreditAccountId == SpecialAccount.InitialBalance.Id)
                    );
            if (initBalRecord is null)
            {
                initBalRecord = new JournalEntry(_config)
                {
                    TransactionDate = DateTime.MinValue,
                    Description = "*INITIAL BALANCE*"
                };

                if (this.JournalType == LedgerType.Bank)
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
            _journal.AddUpdateTransaction(initBalRecord);
        }
    }
}