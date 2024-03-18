using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DLPMoneyTracker2.Config.AddEditMoneyAccounts
{
    public class MoneyAccountVM : BaseViewModel, IJournalAccount
    {
        
        private readonly ISaveJournalAccountUseCase saveAccountUseCase;
        private readonly List<LedgerType> _listValidTypes = new List<LedgerType>() { LedgerType.Bank, LedgerType.LiabilityCard, LedgerType.LiabilityLoan };

        //public static List<LedgerType> ValidTypes        { get { return _listValidTypes; } }

        public MoneyAccountVM(ISaveJournalAccountUseCase saveAccountUseCase) : base()
        {
            this.saveAccountUseCase = saveAccountUseCase;
            this.Clear();
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
        public int OrderBy { get { return DisplayOrder; } }

        public void Clear()
        {
            Id = Guid.Empty;
            Description = string.Empty;
            JournalType = LedgerType.NotSet;
            this.DateClosedUTC = null;
        }

        public void Copy(IJournalAccount account)
        {
            if (account is null) throw new ArgumentNullException("Money Account");
            if (!_listValidTypes.Contains(account.JournalType)) throw new InvalidCastException(string.Format("[{0} - {1}] is not a valid Money Account", account.JournalType.ToString(), account.Description));

            Id = account.Id;
            Description = account.Description;
            JournalType = account.JournalType;
            DateClosedUTC = account.DateClosedUTC;
            DisplayOrder = account.OrderBy;
        }

        public void SaveAccount()
        {
            if (string.IsNullOrWhiteSpace(_desc)) return;
            if (JournalType == LedgerType.NotSet) return;

            saveAccountUseCase.Execute(this);
            
        }
    }
}