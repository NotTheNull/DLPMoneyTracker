using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DLPMoneyTracker2.Config.AddEditMoneyAccounts
{
    public class AddEditMoneyAccountVM : BaseViewModel
    {
        private readonly IGetMoneyAccountsUseCase getMoneyAccountsUseCase;
        private readonly IDeleteJournalAccountUseCase deleteAccountUseCase;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public AddEditMoneyAccountVM(IGetMoneyAccountsUseCase getMoneyAccountsUseCase, IDeleteJournalAccountUseCase deleteAccountUseCase) : base()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            this.getMoneyAccountsUseCase = getMoneyAccountsUseCase;
            this.deleteAccountUseCase = deleteAccountUseCase;
            _editAccount = UICore.DependencyHost.GetRequiredService<MoneyAccountVM>();

            this.JournalTypeList = new List<SpecialDropListItem<LedgerType>>
            {
                new SpecialDropListItem<LedgerType>("Bank", LedgerType.Bank),
                new SpecialDropListItem<LedgerType>("Credit Card", LedgerType.LiabilityCard),
                new SpecialDropListItem<LedgerType>("Loan", LedgerType.LiabilityLoan)
            };
            this.ReloadAccounts();
        }

        private ObservableCollection<MoneyAccountVM> _listAccounts = new ObservableCollection<MoneyAccountVM>();

        public ObservableCollection<MoneyAccountVM> AccountList { get { return _listAccounts; } }


        #region Editing Data Fields

        private MoneyAccountVM _editAccount;

        public LedgerType AccountType
        {
            get { return _editAccount.JournalType; }
            set
            {
                _editAccount.JournalType = value;
                NotifyPropertyChanged(nameof(AccountType));
            }
        }

        public string Description
        {
            get { return _editAccount.Description; }
            set
            {
                _editAccount.Description = value;
                NotifyPropertyChanged(nameof(Description));
            }
        }

        public int DisplayOrder
        {
            get { return _editAccount.DisplayOrder; }
            set
            {
                _editAccount.DisplayOrder = value;
                NotifyPropertyChanged(nameof(DisplayOrder));
            }
        }
        #endregion

        public bool CanEdit { get { return _editAccount?.DateClosedUTC == null; } }
        public bool CanEditMapping { get { return this.AccountType == LedgerType.Bank || this.AccountType == LedgerType.LiabilityCard; } }

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
                    this.NotifyAll();
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
                    this.NotifyAll();
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

                    this.NotifyAll();
                }));
            }
        }

        private RelayCommand _cmdDel;

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
                        deleteAccountUseCase.Execute(vm.Id);
                    }
                    this.ReloadAccounts();
                    _editAccount.Clear();
                    this.NotifyAll();
                }));
            }
        }


        private RelayCommand _cmdEditMap;
        public RelayCommand CommandEditMapping
        {
            get
            {
                return _cmdEditMap ??= new RelayCommand((o) =>
                {
                    if (_editAccount is null) return;

                    var mappingUI = UICore.DependencyHost.GetRequiredService<CSVMappingUI>();
                    mappingUI.LoadMoneyAccount(_editAccount);
                    mappingUI.Show();
                });
            }
        }
        

        #endregion Commands

        /// <summary>
        /// Reloads the listing of accounts
        /// </summary>
        public void ReloadAccounts()
        {
            this.AccountList.Clear();
            var listValidAccounts = getMoneyAccountsUseCase.Execute(true); //_config.GetJournalAccountList(new JournalAccountSearch(MoneyAccountVM.ValidTypes));
            foreach (var act in listValidAccounts)
            {
                MoneyAccountVM vm = UICore.DependencyHost.GetRequiredService<MoneyAccountVM>();
                vm.Copy(act);
                this.AccountList.Add(vm);
            }

            this.NotifyAll();
        }

        private void NotifyAll()
        {
            NotifyPropertyChanged(nameof(Description));
            NotifyPropertyChanged(nameof(AccountType));
            NotifyPropertyChanged(nameof(DisplayOrder));
            NotifyPropertyChanged(nameof(CanEdit));
            NotifyPropertyChanged(nameof(CanEditMapping));
        }
    }
}