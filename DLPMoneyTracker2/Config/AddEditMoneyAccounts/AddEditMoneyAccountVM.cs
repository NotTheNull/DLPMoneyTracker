using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DLPMoneyTracker2.Config.AddEditMoneyAccounts
{
    public class AddEditMoneyAccountVM : BaseViewModel
    {
        private readonly ITrackerConfig _config;
        private readonly IJournal _journal;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public AddEditMoneyAccountVM(ITrackerConfig config, IJournal journal) : base()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _config = config;
            _journal = journal;
            _editAccount = new MoneyAccountVM(config, journal);

            this.JournalTypeList = new List<SpecialDropListItem<LedgerType>>
            {
                new SpecialDropListItem<LedgerType>("Bank", LedgerType.Bank),
                new SpecialDropListItem<LedgerType>("Credit Card", LedgerType.LiabilityCard),
                new SpecialDropListItem<LedgerType>("Loan", LedgerType.LiabilityLoan)
            };
            this.ReloadAccounts();
        }

        private ObservableCollection<MoneyAccountVM> _listAccounts = new ObservableCollection<MoneyAccountVM>();

        public ObservableCollection<MoneyAccountVM> AccountList
        { get { return _listAccounts; } }

        private MoneyAccountVM _editAccount;

        public MoneyAccountVM EditAccount
        { get { return _editAccount; } }

        public bool CanEdit
        { get { return _editAccount?.DateClosedUTC == null; } }

        public List<SpecialDropListItem<LedgerType>> JournalTypeList { get; set; }

        #region Commands

        private RelayCommand _cmdSave;

        public RelayCommand CommandSave
        {
            get
            {
                return _cmdSave ?? (_cmdSave = new RelayCommand((o) =>
                {
                    _editAccount.SaveAccount();
                    this.ReloadAccounts();
                }));
            }
        }

        private RelayCommand _cmdClear;

        public RelayCommand CommandClear
        {
            get
            {
                return _cmdClear ?? (_cmdClear = new RelayCommand((o) =>
                {
                    _editAccount.Clear();
                }));
            }
        }

        private RelayCommand _cmdLoad;

        public RelayCommand CommandLoad
        {
            get
            {
                return _cmdLoad ?? (_cmdLoad = new RelayCommand((act) =>
                {
                    if (act is null) throw new ArgumentNullException("Account");
                    //if (act.GetType() != typeof(IJournalAccount)) throw new InvalidCastException(string.Format("Cannot Load type [{0}", act.GetType().FullName));

                    //_editAccount.LoadAccount((IJournalAccount)act);
                    if (act is MoneyAccountVM vm)
                    {
                        _editAccount = vm;
                    }

                    NotifyPropertyChanged(nameof(EditAccount));
                }));
            }
        }

        public RelayCommand _cmdDel;

        public RelayCommand CommandRemove
        {
            get
            {
                return _cmdDel ?? (_cmdDel = new RelayCommand((act) =>
                {
                    if (act is null) throw new ArgumentNullException("Account");
                    //if (act.GetType() != typeof(IJournalAccount)) throw new InvalidCastException(string.Format("Cannot Load type [{0}", act.GetType().FullName));
                    if (act is MoneyAccountVM vm)
                    {
                        _config.RemoveJournalAccount(vm.Id);
                    }
                    this.ReloadAccounts();
                    _editAccount.Clear();
                }));
            }
        }

        #endregion Commands

        /// <summary>
        /// Reloads the listing of accounts
        /// </summary>
        public void ReloadAccounts()
        {
            this.AccountList.Clear();
            var listValidAccounts = _config.GetJournalAccountList(new JournalAccountSearch(MoneyAccountVM.ValidTypes));
            foreach (var act in listValidAccounts)
            {
                this.AccountList.Add(new MoneyAccountVM(_config, _journal, act));
            }
        }
    }
}