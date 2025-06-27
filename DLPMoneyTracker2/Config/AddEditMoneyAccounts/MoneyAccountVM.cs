using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;

namespace DLPMoneyTracker2.Config.AddEditMoneyAccounts
{
    public class MoneyAccountVM : BaseViewModel, IJournalAccount
    {
        private readonly IGetNextUIDUseCase getNextUIDUseCase;
        private readonly ISaveJournalAccountUseCase _saveAccountUseCase;
        private readonly List<LedgerType> _listValidTypes = [LedgerType.Bank, LedgerType.LiabilityCard, LedgerType.LiabilityLoan];

        public MoneyAccountVM(
            IGetNextUIDUseCase getNextUIDUseCase,
            ISaveJournalAccountUseCase saveAccountUseCase) : base()
        {
            this.getNextUIDUseCase = getNextUIDUseCase;
            this._saveAccountUseCase = saveAccountUseCase;
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

        public bool IsClosed => _closeDateUTC.HasValue;

        public string DisplayClosedMessage
        {
            get
            {
#pragma warning disable CS8629 // Nullability is checked with this.IsClosed
                if (this.IsClosed) return string.Format("CLOSED: {0}", _closeDateUTC.Value.ToLocalTime());
#pragma warning restore CS8629 // Nullable value type may be null.

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

        public int OrderBy => DisplayOrder; 

        public ICSVMapping? Mapping { get; set; }

        public void Clear()
        {
            Id = getNextUIDUseCase.Execute();
            Description = string.Empty;
            JournalType = LedgerType.NotSet;
            this.DateClosedUTC = null;
            this.Mapping = null;
        }

        public void Copy(IJournalAccount account)
        {
            ArgumentNullException.ThrowIfNull(account);
            if (!_listValidTypes.Contains(account.JournalType)) throw new InvalidCastException(string.Format("[{0} - {1}] is not a valid Money Account", account.JournalType.ToString(), account.Description));

            Id = account.Id;
            Description = account.Description;
            JournalType = account.JournalType;
            DateClosedUTC = account.DateClosedUTC;
            DisplayOrder = account.OrderBy;

            if (this.Mapping != null)
            {
                this.Mapping.Dispose();
                this.Mapping = null;
            }

            if (account is IMoneyAccount money)
            {
                this.Mapping = new CSVMapping();
                if (money.Mapping != null) this.Mapping.Copy(money.Mapping);
            }
        }

        public void SaveAccount()
        {
            if (string.IsNullOrWhiteSpace(_description)) return;
            if (JournalType == LedgerType.NotSet) return;

            var account = JournalAccountFactory.Build(this);
            if (account is IMoneyAccount money && this.Mapping != null)
            {
                money.Mapping?.Copy(this.Mapping);
            }

            _saveAccountUseCase.Execute(account);
        }
    }
}