using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DLPMoneyTracker2.Main.TransactionList
{
    public class AccountTransactionDetailVM : BaseViewModel
    {
        private readonly IGetTransactionsBySearchUseCase searchTransactionsUseCase;
        private readonly NotificationSystem notifications;

        public AccountTransactionDetailVM(
            IGetTransactionsBySearchUseCase searchTransactionsUseCase,
            NotificationSystem notifications)
        {
            this.searchTransactionsUseCase = searchTransactionsUseCase;
            this.notifications = notifications;
            this.notifications.TransactionsModified += Notifications_TransactionsModified;
        }

        private void Notifications_TransactionsModified(Guid debitAccountId, Guid creditAccountId)
        {
            this.Reload();
        }

        public string HeaderText => string.Format("{0}: {1}", this.FilterAccount?.JournalType.ToDisplayText().ToUpper(), this.FilterAccount?.Description);

        private readonly ObservableCollection<SingleAccountDetailVM> _listRecords = [];

        public ObservableCollection<SingleAccountDetailVM> DisplayRecordsList => _listRecords;

        #region Filter Related

        private TransDetailFilter _filter = new();

        public IJournalAccount? FilterAccount => _filter.Account;

        public DateTime FilterBeginDate
        {
            get { return _filter.FilterDates.Begin; }
            set
            {
                _filter.FilterDates.Begin = value;
                NotifyPropertyChanged(nameof(FilterBeginDate));
            }
        }

        public DateTime FilterEndDate
        {
            get { return _filter.FilterDates.End; }
            set
            {
                _filter.FilterDates.End = value;
                NotifyPropertyChanged(nameof(FilterEndDate));
            }
        }

        public string FilterText
        {
            get { return _filter.SearchText; }
            set
            {
                _filter.SearchText = value;
                NotifyPropertyChanged(nameof(FilterText));
            }
        }

        public bool AreFiltersVisible => _filter.AreFilterControlsVisible;
        public bool ShowBankDate => this.FilterAccount is IMoneyAccount;

        #endregion Filter Related

        #region Commands

        public RelayCommand CommandRefresh =>
            new((o) => this.Reload());

        public RelayCommand CommandSearch =>
            new((o) => this.Reload());

        public RelayCommand CommandResetFilter =>
            new((o) =>
            {
                _filter ??= new TransDetailFilter();
                _filter.Clear();
            });

        #endregion Commands

        /// <summary>
        /// Copies the filter options into the model's filter.
        /// By setting the values in this manner, we avoid the issue of zombie memory objects
        /// with classes being passed by reference.
        /// </summary>
        /// <param name="filter"></param>
        public void ApplyFilters(TransDetailFilter filter)
        {
            _filter = filter;
            NotifyPropertyChanged(nameof(this.FilterAccount));
            NotifyPropertyChanged(nameof(this.FilterBeginDate));
            NotifyPropertyChanged(nameof(this.FilterEndDate));
            NotifyPropertyChanged(nameof(this.FilterText));
            this.Reload();
        }

        public void Reload()
        {
            this.Clear();
            var records = searchTransactionsUseCase.Execute(_filter.FilterDates, this.FilterText, this.FilterAccount);
            if (records?.Any() != true) return;

            this.LoadRecords(records);
        }

        public void Clear()
        {
            _listRecords.Clear();
        }

        private void LoadRecords(IEnumerable<IMoneyTransaction> records)
        {
            if (this.FilterAccount is null) return;
            if (records?.Any() != true) return;

            foreach (var rec in records.OrderBy(o => o.TransactionDate).ThenBy(o => o.Description))
            {
                if (_filter.UseBudgetLogic)
                {
                    if (rec.CreditAccount.JournalType == LedgerType.NotSet || rec.DebitAccount.JournalType == LedgerType.NotSet) continue;
                }

                SingleAccountDetailVM vm = new(FilterAccount, rec, notifications);
                _listRecords.Add(vm);
            }
        }
    }
}