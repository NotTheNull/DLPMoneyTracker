using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DLPMoneyTracker2.Config.AddEditLedgerAccounts
{
    public class AddEditLedgerAccountVM : BaseViewModel
    {
        private readonly IGetNominalAccountsUseCase getLedgerAccountsUseCase;
        private readonly IDeleteJournalAccountUseCase deleteAcountUseCase;
        private readonly NotificationSystem notifications;

        public AddEditLedgerAccountVM(
            IGetNominalAccountsUseCase getLedgerAccountsUseCase,
            IDeleteJournalAccountUseCase deleteAcountUseCase,
            NotificationSystem notifications) : base()
        {
            this.getLedgerAccountsUseCase = getLedgerAccountsUseCase;
            this.deleteAcountUseCase = deleteAcountUseCase;
            this.notifications = notifications;
            _editAccount = UICore.DependencyHost.GetRequiredService<LedgerAccountVM>();
            

            this.ReloadAccounts();
        }

        private ObservableCollection<LedgerAccountVM> _listAccounts = new ObservableCollection<LedgerAccountVM>();
        public ObservableCollection<LedgerAccountVM> AccountList { get { return _listAccounts; } }

        public bool CanEdit { get { return _editAccount?.DateClosedUTC == null; } }

        public List<SpecialDropListItem<LedgerType>> JournalTypeList { get; set; } = new List<SpecialDropListItem<LedgerType>>()
            {
                new SpecialDropListItem<LedgerType>("Receivable", LedgerType.Receivable),
                new SpecialDropListItem<LedgerType>("Payable", LedgerType.Payable)
            };
        public List<SpecialDropListItem<BudgetTrackingType>> BudgetTypeList { get; set; } = new List<SpecialDropListItem<BudgetTrackingType>>()
        {
            new SpecialDropListItem<BudgetTrackingType>("DO NOT TRACK", BudgetTrackingType.DO_NOT_TRACK),
            new SpecialDropListItem<BudgetTrackingType>("Fixed Expense/Income", BudgetTrackingType.Fixed),
            new SpecialDropListItem<BudgetTrackingType>("Variable Expense/Income", BudgetTrackingType.Variable)
        };


        #region Editing Data Fields

        private LedgerAccountVM _editAccount;


        public string Description
        {
            get { return _editAccount.Description; }
            set
            {
                _editAccount.Description = value;
                NotifyPropertyChanged(nameof(Description));
            }
        }

        public LedgerType AccountType
        {
            get { return _editAccount.JournalType; }
            set
            {
                _editAccount.JournalType = value;
                NotifyPropertyChanged(nameof(AccountType));
            }
        }

        public BudgetTrackingType BudgetType
        {
            get { return _editAccount.BudgetType; }
            set
            {
                _editAccount.BudgetType = value;
                NotifyPropertyChanged(nameof(BudgetType));
            }
        }

        public decimal MonthlyBudget
        {
            get { return _editAccount.DefaultMonthlyBudgetAmount; }
            set
            {
                _editAccount.DefaultMonthlyBudgetAmount = value;
                NotifyPropertyChanged(nameof(MonthlyBudget));
            }
        }

        #endregion


        #region Commands

        private RelayCommand _cmdSave;

        public RelayCommand CommandSave
        {
            get
            {
                return _cmdSave ?? (_cmdSave = new RelayCommand((o) =>
                {
                    _editAccount.SaveAccount();
                    _editAccount.Clear();

                    // Even if no changes were made, best to trigger any recalculations regarding budget
                    notifications.TriggerBudgetAmountChanged(_editAccount.Id);

                    this.NotifyAll();
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
                    if (act is LedgerAccountVM vm)
                    {
                        _editAccount.Copy(vm);

                        this.NotifyAll();
                    }
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

                    if (act is LedgerAccountVM vm)
                    {
                        deleteAcountUseCase.Execute(vm.Id);
                    }
                    _editAccount.Clear();
                    this.NotifyAll();
                    this.ReloadAccounts();
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
            var listAccounts = getLedgerAccountsUseCase.Execute(true); //_config.GetJournalAccountList(new JournalAccountSearch(LedgerAccountVM.ValidTypes));
            foreach (var act in listAccounts)
            {
                LedgerAccountVM vm = UICore.DependencyHost.GetRequiredService<LedgerAccountVM>();
                vm.Copy(act);
                this.AccountList.Add(vm);
            }

        }

        private void NotifyAll()
        {
            NotifyPropertyChanged(nameof(Description));
            NotifyPropertyChanged(nameof(AccountType));
            NotifyPropertyChanged(nameof(BudgetType));
            NotifyPropertyChanged(nameof(MonthlyBudget));
        }
    }
}