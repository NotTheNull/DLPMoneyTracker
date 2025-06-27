using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Core;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts;

namespace DLPMoneyTracker2.Config.AddEditLedgerAccounts
{
    public class LedgerAccountVM : BaseViewModel, INominalAccount, ISubLedgerAccount
    {

        private readonly List<LedgerType> _listValidTypes = [LedgerType.Payable, LedgerType.Receivable];
        private readonly IGetNextUIDUseCase getNextUIDUseCase;
        private readonly ISaveJournalAccountUseCase saveUseCase;


        public LedgerAccountVM(
            IGetNextUIDUseCase getNextUIDUseCase,
            ISaveJournalAccountUseCase saveUseCase) : base()
        {
            this.getNextUIDUseCase = getNextUIDUseCase;
            this.saveUseCase = saveUseCase;
            this.Clear();
        }

        public Guid Id { get; private set; }

        private string _description = string.Empty;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
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

        public bool IsClosed => _closeDateUTC.HasValue;

        public string DisplayClosedMessage
        {
            get
            {
#pragma warning disable CS8629 // this.IsClosed is already checking the state of the datetime
                if (this.IsClosed) return string.Format("CLOSED: {0}", _closeDateUTC.Value.ToLocalTime());
#pragma warning restore CS8629 // Nullable value type may be null.

                return string.Empty;
            }
        }

        public int OrderBy => 9999;


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


        private decimal _budget;
        public decimal DefaultMonthlyBudgetAmount
        {
            get { return _budget; }
            set
            {
                _budget = value;
                this.CurrentBudgetAmount = value;
                NotifyPropertyChanged(nameof(DefaultMonthlyBudgetAmount));
            }
        }


        private IJournalAccount? _summary;
        public IJournalAccount? SummaryAccount
        {
            get { return _summary; }
            set
            {
                _summary = value;
                NotifyPropertyChanged(nameof(SummaryAccount));
            }
        }


        public decimal CurrentBudgetAmount { get; set; }




        public void Clear()
        {
            Id = getNextUIDUseCase.Execute();
            Description = string.Empty;
            JournalType = LedgerType.NotSet;
            BudgetType = BudgetTrackingType.DO_NOT_TRACK;
            this.DefaultMonthlyBudgetAmount = decimal.Zero;
            this.CurrentBudgetAmount = decimal.Zero;
            this.DateClosedUTC = null;
            this.SummaryAccount = null;
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
                this.DefaultMonthlyBudgetAmount = account.DefaultMonthlyBudgetAmount;
            }

            if (cpy is ISubLedgerAccount sub)
            {
                this.SummaryAccount = sub.SummaryAccount;
            }
        }


        public void SaveAccount()
        {
            if (string.IsNullOrWhiteSpace(_description)) return;
            if (JournalType == LedgerType.NotSet) return;

            saveUseCase.Execute(this);
        }
    }
}