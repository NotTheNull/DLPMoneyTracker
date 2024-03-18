using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.SQL.Data;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;

namespace DLPMoneyTracker2.Config.AddEditLedgerAccounts
{
    public class LedgerAccountVM : BaseViewModel, IJournalAccount
    {

        private readonly List<LedgerType> _listValidTypes = new List<LedgerType>() { LedgerType.Payable, LedgerType.Receivable };
        private readonly ISaveJournalAccountUseCase saveUseCase;


        public LedgerAccountVM(ISaveJournalAccountUseCase saveUseCase) : base()
        {
            this.saveUseCase = saveUseCase;
        }

        public LedgerAccountVM(ISaveJournalAccountUseCase saveUseCase, IJournalAccount act) : this(saveUseCase)
        {
            this.Copy(act);
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

            Id = cpy.Id;
            Description = cpy.Description;
            JournalType = cpy.JournalType;
            DateClosedUTC = cpy.DateClosedUTC;
        }

        public void SaveAccount()
        {
            if (string.IsNullOrWhiteSpace(_desc)) return;
            if (JournalType == LedgerType.NotSet) return;

            saveUseCase.Execute(this);
        }
    }
}