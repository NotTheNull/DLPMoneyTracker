using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
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
        private readonly IGetMoneyAccountsUseCase getMoneyAccountsUseCase;
        private readonly IGetNextSubLedgerIdUseCase getNextSubLedgerIdUseCase;
        private readonly IDeleteJournalAccountUseCase deleteAcountUseCase;

        public AddEditLedgerAccountVM(
            IGetNominalAccountsUseCase getLedgerAccountsUseCase,
            IGetMoneyAccountsUseCase getMoneyAccountsUseCase,
            IGetNextSubLedgerIdUseCase getNextSubLedgerIdUseCase,
            IDeleteJournalAccountUseCase deleteAcountUseCase) : base()
        {
            this.getLedgerAccountsUseCase = getLedgerAccountsUseCase;
            this.getMoneyAccountsUseCase = getMoneyAccountsUseCase;
            this.getNextSubLedgerIdUseCase = getNextSubLedgerIdUseCase;
            this.deleteAcountUseCase = deleteAcountUseCase;
            _editAccount = UICore.DependencyHost.GetRequiredService<LedgerAccountVM>();
            this.JournalTypeList = new List<SpecialDropListItem<LedgerType>>
            {
                new SpecialDropListItem<LedgerType>("Receivable", LedgerType.Receivable),
                new SpecialDropListItem<LedgerType>("Payable", LedgerType.Payable)
            };
            this.ReloadAccounts();
        }

        private ObservableCollection<LedgerAccountVM> _listAccounts = new ObservableCollection<LedgerAccountVM>();
        public ObservableCollection<LedgerAccountVM> AccountList { get { return _listAccounts; } }

        public bool CanEdit { get { return _editAccount?.DateClosedUTC == null; } }

        public List<SpecialDropListItem<LedgerType>> JournalTypeList { get; set; }

        private LedgerAccountVM _editAccount;

        public LedgerAccountVM EditAccount { get { return _editAccount; } }

        public string Description
        {
            get { return _editAccount?.Description ?? string.Empty; }
            set
            {
                _editAccount.Description = value;
                NotifyPropertyChanged(nameof(Description));
            }
        }

        public LedgerType AccountType
        {
            get { return _editAccount?.JournalType ?? LedgerType.NotSet; }
            set
            {
                _editAccount.JournalType = value;
                NotifyPropertyChanged(nameof(AccountType));
            }
        }

        public IMoneyAccount SelectedMoneyAccount
        {
            get { return _editAccount.BankAccount; }
            set
            {
                _editAccount.BankAccount = value;
                NotifyPropertyChanged(nameof(SelectedMoneyAccount));
            }
        }

        public bool IsSubLedger { get { return _editAccount.IsSubLedger; } }



        List<SpecialDropListItem<IMoneyAccount>> listMoneyAccounts = new List<SpecialDropListItem<IMoneyAccount>>();
        public List<SpecialDropListItem<IMoneyAccount>> MoneyAccountList { get { return listMoneyAccounts; } }



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

        private RelayCommand _cmdAddSubLedger;
        public RelayCommand CommandAddSubLedger
        {
            get
            {
                return _cmdAddSubLedger ?? (_cmdAddSubLedger = new RelayCommand((objAccount) =>
                {
                    if (objAccount is IJournalAccount account)
                    {
                        _editAccount.Clear();
                        _editAccount.Description = string.Format("{0} - Subledger Name", account.Description);
                        _editAccount.JournalType = account.JournalType;
                        _editAccount.CategoryId = account.CategoryId;
                        _editAccount.SubLedgerId = getNextSubLedgerIdUseCase.Execute(_editAccount.JournalType, _editAccount.CategoryId);
                        _editAccount.IsSubLedger = true;
                        this.NotifyAll();
                    }
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

            this.MoneyAccountList.Clear();
            var listMoneyAccounts = getMoneyAccountsUseCase.Execute(false);
            foreach (var account in listMoneyAccounts)
            {
                if (account is IMoneyAccount money)
                {
                    this.MoneyAccountList.Add(new SpecialDropListItem<IMoneyAccount>(money.Description, money));
                }
            }
        }

        private void NotifyAll()
        {
            NotifyPropertyChanged(nameof(EditAccount));
            NotifyPropertyChanged(nameof(Description));
            NotifyPropertyChanged(nameof(AccountType));
            NotifyPropertyChanged(nameof(SelectedMoneyAccount));
            NotifyPropertyChanged(nameof(IsSubLedger));
        }
    }
}