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


        public AddEditMoneyAccountVM(
            MoneyAccountVM viewModel,
            IGetMoneyAccountsUseCase getMoneyAccountsUseCase,
            IDeleteJournalAccountUseCase deleteAccountUseCase) : base()
        {
            this.getMoneyAccountsUseCase = getMoneyAccountsUseCase;
            this.deleteAccountUseCase = deleteAccountUseCase;
            _editAccount = viewModel;

            this.ReloadAccounts();
        }

        private readonly ObservableCollection<MoneyAccountVM> _listAccounts = [];
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

        public List<SpecialDropListItem<LedgerType>> JournalTypeList { get; } =
        [
            new("Bank", LedgerType.Bank),
            new("Credit Card", LedgerType.LiabilityCard),
            new("Loan", LedgerType.LiabilityLoan)
        ];



        #region Commands

        public RelayCommand CommandSave => 
            new ((o) =>
            {
                _editAccount.SaveAccount();
                this.ReloadAccounts();
                this.NotifyAll();
            });
            
        

        public RelayCommand CommandClear =>
            new((o) =>
            {
                _editAccount.Clear();
                this.NotifyAll();
            });


        public RelayCommand CommandLoad =>
            new((act) =>
            {
                ArgumentNullException.ThrowIfNull(act);
                
                if (act is MoneyAccountVM vm)
                {
                    _editAccount = vm;
                }

                this.NotifyAll();
            });

        public RelayCommand CommandRemove =>
            new((act) =>
            {
                ArgumentNullException.ThrowIfNull(act);
                
                if (act is MoneyAccountVM vm)
                {
                    deleteAccountUseCase.Execute(vm.Id);
                }
                this.ReloadAccounts();
                _editAccount.Clear();
                this.NotifyAll();
            });


        public RelayCommand CommandEditMapping => 
            new((o) =>
            {
                if (_editAccount is null) return;

                var mappingUI = UICore.DependencyHost.GetRequiredService<CSVMappingUI>();
                mappingUI.LoadMoneyAccount(_editAccount);
                mappingUI.ShowDialog();
            });


        #endregion Commands

        public void ReloadAccounts()
        {
            this.AccountList.Clear();
            var listValidAccounts = getMoneyAccountsUseCase.Execute(true); 
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