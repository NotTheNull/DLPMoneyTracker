using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.Config.AddEditLedgerAccounts
{
    public class LedgerAccountVM : BaseViewModel
    {
        private readonly ITrackerConfig _config;


        private static readonly List<JournalAccountType> _listValidTypes = new List<JournalAccountType>() { JournalAccountType.Payable, JournalAccountType.Receivable };
        public static List<JournalAccountType> ValidTypes { get { return _listValidTypes; } }


        public LedgerAccountVM(ITrackerConfig config) : base()
        {
            _config = config;            
        }
        public LedgerAccountVM(ITrackerConfig config, IJournalAccount act) : base()
        {
            _config = config;
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


        private decimal _budget;

        public decimal MonthlyBudget
        {
            get { return _budget; }
            set 
            {
                _budget = value; 
                NotifyPropertyChanged(nameof(MonthlyBudget));
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





        public void Clear()
        {
            Id = Guid.Empty;
            Description = string.Empty;
            MonthlyBudget = decimal.Zero;
            JournalType = JournalAccountType.NotSet;
            this.DateClosedUTC = null;
        }


        public void LoadAccount(IJournalAccount account)
        {
            if (account is null) throw new ArgumentNullException("Ledger Account");
            if (!_listValidTypes.Contains(account.JournalType)) throw new InvalidCastException(string.Format("[{0} - {1}] is not a valid Money Account", account.JournalType.ToString(), account.Description));

            Id = account.Id;
            Description = account.Description;
            JournalType = account.JournalType;
            DateClosedUTC = account.DateClosedUTC;
            MonthlyBudget = account.MonthlyBudgetAmount;
        }

        public void SaveAccount()
        {
            if (string.IsNullOrWhiteSpace(_desc)) return;
            if (JournalType == JournalAccountType.NotSet) return;


            var acct = _config.LedgerAccountsList.FirstOrDefault(x => x.Id == Id);
            if (acct is null)
            {
                acct = JournalAccountFactory.Build(this.Description, this.JournalType, this.MonthlyBudget);
                _config.AddJournalAccount(acct);
            }
            else
            {
                JournalAccountFactory.Update(ref acct, this.Description, this.MonthlyBudget);
            }
            _config.SaveJournalAccounts();
        }

    }
}
