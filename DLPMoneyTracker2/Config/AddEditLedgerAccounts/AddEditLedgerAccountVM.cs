﻿using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.Config.AddEditLedgerAccounts
{
    public class AddEditLedgerAccountVM : BaseViewModel
    {
        private readonly ITrackerConfig _config;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public AddEditLedgerAccountVM(ITrackerConfig config) : base()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _config = config;
            _editAccount = new LedgerAccountVM(config);
            this.JournalTypeList = new List<SpecialDropListItem<JournalAccountType>>
            {
                new SpecialDropListItem<JournalAccountType>("Receivable", JournalAccountType.Receivable),
                new SpecialDropListItem<JournalAccountType>("Payable", JournalAccountType.Payable)
            };
        }


        private ObservableCollection<LedgerAccountVM> _listAccounts = new ObservableCollection<LedgerAccountVM>();
        public ObservableCollection<LedgerAccountVM> AccountList { get { return _listAccounts; } }

        private LedgerAccountVM _editAccount;
        public LedgerAccountVM EditAccount { get { return _editAccount; } }

        public bool CanEdit { get { return _editAccount?.DateClosedUTC == null; } }

        public List<SpecialDropListItem<JournalAccountType>> JournalTypeList { get; set; }


        #region Commands
        private RelayCommand _cmdSave;
        public RelayCommand CommandSave
        {
            get
            {
                return _cmdSave ?? (_cmdSave = new RelayCommand((o) =>
                {
                    _editAccount.SaveAccount();
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
                    if (act.GetType() != typeof(IJournalAccount)) throw new InvalidCastException(string.Format("Cannot Load type [{0}", act.GetType().FullName));

                    _editAccount.LoadAccount((IJournalAccount)act);
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
                    if (act.GetType() != typeof(IJournalAccount)) throw new InvalidCastException(string.Format("Cannot Load type [{0}", act.GetType().FullName));
                    _config.RemoveJournalAccount(((IJournalAccount)act).Id);
                    this.ReloadAccounts();
                    _editAccount.Clear();
                }));
            }
        }
        #endregion


        /// <summary>
        /// Reloads the listing of accounts
        /// </summary>
        public void ReloadAccounts()
        {
            this.AccountList.Clear();
            foreach (var act in _config.LedgerAccountsList.Where(x => LedgerAccountVM.ValidTypes.Contains(x.JournalType)))
            {
                this.AccountList.Add(new LedgerAccountVM(_config, act));
            }

        }
    }
}