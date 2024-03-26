using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.SQL.Data;
using DLPMoneyTracker.Core;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts;

namespace DLPMoneyTracker2.Config.AddEditLedgerAccounts
{
    public class LedgerAccountVM : BaseViewModel, INominalAccount
    {

        private readonly List<LedgerType> _listValidTypes = new List<LedgerType>() { LedgerType.Payable, LedgerType.Receivable };
        private readonly ISaveJournalAccountUseCase saveUseCase;


        public LedgerAccountVM(
            ISaveJournalAccountUseCase saveUseCase) : base()
        {
            this.saveUseCase = saveUseCase;
        }

        private IJournalAccount _mainAccount;

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
                NotifyPropertyChanged(nameof(DisplayJournalType));
            }
        }

        public string DisplayJournalType { get { return this.JournalType.ToDisplayText(); } }

        private DateTime? _closeDateUTC;

        public DateTime? DateClosedUTC
        {
            get { return _closeDateUTC?.ToLocalTime(); }
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

        public int OrderBy { get { return 9999; } }



        private IMoneyAccount _bank;

        public IMoneyAccount BankAccount
        {
            get { return _bank; }
            set
            {
                _bank = value;
                NotifyPropertyChanged(nameof(BankAccount));
            }
        }

        private BudgetTrackingType _budgetType;

        public BudgetTrackingType BudgetType
        {
            get { return _budgetType; }
            set
            {
                _budgetType = value;
                NotifyPropertyChanged(nameof(BudgetType));
                NotifyPropertyChanged(nameof(DisplayBudgetType));
            }
        }

        public string DisplayBudgetType { get { return this.BudgetType.ToDisplayText(); } }


        public void Clear()
        {
            Id = Guid.Empty;
            Description = string.Empty;
            JournalType = LedgerType.NotSet;
            this.DateClosedUTC = null;
        }

        public void Copy(IJournalAccount cpy)
        {
            ArgumentNullException.ThrowIfNull(cpy);
            if (!_listValidTypes.Contains(cpy.JournalType)) throw new InvalidCastException(string.Format("[{0} - {1}] is not a valid Ledger Account", cpy.JournalType.ToString(), cpy.Description));

            if (cpy is INominalAccount account)
            {
                Id = account.Id;
                Description = account.Description;
                JournalType = account.JournalType;
                DateClosedUTC = account.DateClosedUTC;
                BudgetType = account.BudgetType;
            }
        }

        public void CreateNewSubLedger(IJournalAccount mainAccount)
        {
            _mainAccount = mainAccount;
            this.Id = Guid.NewGuid();
            this.Description = "Enter Subledger Name";
            this.JournalType = mainAccount.JournalType;
        }

        public void SaveAccount()
        {
            if (string.IsNullOrWhiteSpace(_desc)) return;
            if (JournalType == LedgerType.NotSet) return;

            saveUseCase.Execute(this);
        }
    }
}