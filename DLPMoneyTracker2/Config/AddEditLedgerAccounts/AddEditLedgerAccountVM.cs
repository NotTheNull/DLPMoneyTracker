using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DLPMoneyTracker2.Config.AddEditLedgerAccounts
{
    public class AddEditLedgerAccountVM : BaseViewModel
    {
        private readonly IGetNominalAccountsUseCase _getLedgerAccountsUseCase;
        private readonly IDeleteJournalAccountUseCase _deleteAccountUseCase;
        private readonly IGetSummaryAccountListByType _getSummaryListUseCase;
        private readonly NotificationSystem _notifications;
        private readonly LedgerAccountVM _editAccount;

        public AddEditLedgerAccountVM(
            IGetNominalAccountsUseCase getLedgerAccountsUseCase,
            IDeleteJournalAccountUseCase deleteAccountUseCase,
            IGetSummaryAccountListByType getSummaryListUseCase,
            NotificationSystem notifications, LedgerAccountVM viewModel) : base()
        {
            _getLedgerAccountsUseCase = getLedgerAccountsUseCase;
            _deleteAccountUseCase = deleteAccountUseCase;
            _getSummaryListUseCase = getSummaryListUseCase;
            _notifications = notifications;
            _editAccount = viewModel;
            
            this.ReloadAccounts();
        }

        private readonly ObservableCollection<LedgerAccountVM> _listAccounts = [];
        public ObservableCollection<LedgerAccountVM> AccountList => _listAccounts;

        public bool CanEdit { get { return _editAccount?.DateClosedUTC == null; } }

        public List<SpecialDropListItem<LedgerType>> JournalTypeList { get; } =
            [
                new SpecialDropListItem<LedgerType>("Receivable", LedgerType.Receivable),
                new SpecialDropListItem<LedgerType>("Payable", LedgerType.Payable)
            ];

        public List<SpecialDropListItem<BudgetTrackingType>> BudgetTypeList { get; } =
        [
            new SpecialDropListItem<BudgetTrackingType>("DO NOT TRACK", BudgetTrackingType.DO_NOT_TRACK),
            new SpecialDropListItem<BudgetTrackingType>("Fixed Expense/Income", BudgetTrackingType.Fixed),
            new SpecialDropListItem<BudgetTrackingType>("Variable Expense/Income", BudgetTrackingType.Variable)
        ];

        public ObservableCollection<SpecialDropListItem<IJournalAccount?>> SummaryAccountList { get; set; } = [];


        #region Editing Data Fields

        

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
                this.LoadSummaryAccounts();
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

        public IJournalAccount? SelectedSummaryAccount
        {
            get { return _editAccount.SummaryAccount; }
            set {
                _editAccount.SummaryAccount = value;
                NotifyPropertyChanged(nameof(SelectedSummaryAccount));
            }
        }

        #endregion


        #region Commands

        public RelayCommand CommandSave => 
            new((o) =>
            {
                _editAccount.SaveAccount();
                _editAccount.Clear();
                this.SummaryAccountList.Clear();

                // Even if no changes were made, best to trigger any recalculations regarding budget
                _notifications.TriggerBudgetAmountChanged(_editAccount.Id);

                this.NotifyAll();
                this.ReloadAccounts();
            });

        public RelayCommand CommandClear => 
            new((o) =>
            {
                _editAccount.Clear();
                this.SummaryAccountList.Clear();
                this.NotifyAll();
            });

        public RelayCommand CommandLoad => 
            new((act) =>
            {
                if (act is LedgerAccountVM vm)
                {
                    _editAccount.Copy(vm);
                    this.LoadSummaryAccounts();
                    this.NotifyAll();
                }
            });

        public RelayCommand CommandRemove => 
            new((act) =>
            {
                ArgumentNullException.ThrowIfNull(act);

                if (act is LedgerAccountVM vm)
                {
                    _deleteAccountUseCase.Execute(vm.Id);
                }
                _editAccount.Clear();
                this.NotifyAll();
                this.ReloadAccounts();
            });



        #endregion Commands

        /// <summary>
        /// Reloads the listing of accounts
        /// </summary>
        public void ReloadAccounts()
        {
            this.AccountList.Clear();
            var listAccounts = _getLedgerAccountsUseCase.Execute(true); //_config.GetJournalAccountList(new JournalAccountSearch(LedgerAccountVM.ValidTypes));
            if (listAccounts?.Any() != true) return;

            foreach (var act in listAccounts.OrderBy(o => o.Description))
            {
                LedgerAccountVM vm = UICore.DependencyHost.GetRequiredService<LedgerAccountVM>();
                vm.Copy(act);
                this.AccountList.Add(vm);
            }

        }

        private void LoadSummaryAccounts()
        {
            this.SummaryAccountList.Clear();
            this.SummaryAccountList.Add(new SpecialDropListItem<IJournalAccount?>("SET AS SUMMARY ACCOUNT", null));

            var listAccounts = _getSummaryListUseCase.Execute(this.AccountType);
            if (listAccounts?.Any() != true) return;

            foreach(IJournalAccount account in listAccounts.OrderBy(o => o.Description))
            {
                if (account.Id == _editAccount.Id) continue;
                this.SummaryAccountList.Add(new SpecialDropListItem<IJournalAccount?>(account.Description, account));
            }
        }

        private void NotifyAll()
        {
            NotifyPropertyChanged(nameof(Description));
            NotifyPropertyChanged(nameof(AccountType));
            NotifyPropertyChanged(nameof(BudgetType));
            NotifyPropertyChanged(nameof(MonthlyBudget));
            NotifyPropertyChanged(nameof(SelectedSummaryAccount));
        }
    }
}