
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
    public class TransactionDetailVM : BaseViewModel
	{
        private readonly IGetTransactionsBySearchUseCase searchTransactionUseCase;
        private readonly NotificationSystem notifications;

        public TransactionDetailVM(
			IGetTransactionsBySearchUseCase searchTransactionUseCase,
			NotificationSystem notifications) : base()
		{
            this.searchTransactionUseCase = searchTransactionUseCase;
            this.notifications = notifications;
            this.notifications.TransactionsModified += Notifications_TransactionsModified;


			this.Reload();
        }

        private void Notifications_TransactionsModified(Guid debitAccountId, Guid creditAccountId)
        {
			this.Reload();
        }

        private ObservableCollection<JournalEntryVM> _listRecords = new ObservableCollection<JournalEntryVM>();

		public ObservableCollection<JournalEntryVM> DisplayRecordsList { get { return _listRecords; } }

		#region Filter Related

		private TransDetailFilter _filter = new TransDetailFilter();

		public IJournalAccount? FilterAccount
		{
			get { return _filter.Account; }
		}

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

		private bool _isCloseVis;

		public bool IsCloseButtonVisible
		{
			get { return _isCloseVis; }
			set
			{
				_isCloseVis = value;
				NotifyPropertyChanged(nameof(IsCloseButtonVisible));
			}
		}

		private bool _isFiltered;

		public bool IsFiltersVisible
		{
			get { return _isFiltered; }
			set
			{
				_isFiltered = value;
				NotifyPropertyChanged(nameof(IsFiltersVisible));
			}
		}

		#endregion Filter Related

		#region Commands

		private RelayCommand _cmdRefresh;

		public RelayCommand CommandRefresh
		{
			get { return _cmdRefresh ?? (_cmdRefresh = new RelayCommand((o) => this.Reload())); }
		}

		private RelayCommand _cmdFilter;

		public RelayCommand CommandSearch
		{
			get { return _cmdFilter ?? (_cmdFilter = new RelayCommand((o) => this.Reload())); }
		}

		private RelayCommand _cmdResetFilter;
        

        public RelayCommand CommandResetFilter
		{
			get
			{
				return _cmdResetFilter ??
					(
					_cmdResetFilter = new RelayCommand((o) =>
					{
						_filter ??= new TransDetailFilter();
						_filter.Clear();
					}

					));
			}
		}

		#endregion Commands

		/// <summary>
		/// Copies the filter options into the model's filter.
		/// By setting the values in this manner, we avoid the issue of zombie memory objects
		/// with classes being passed by reference.
		/// </summary>
		/// <param name="filter"></param>
		public void ApplyFilters(TransDetailFilter filter)
		{
			_filter.Account = filter.Account;
			this.FilterBeginDate = filter.FilterDates?.Begin ?? DateTime.MinValue;
			this.FilterEndDate = filter.FilterDates?.End ?? DateTime.MaxValue;
			this.FilterText = filter.SearchText?.Trim() ?? string.Empty;
			_filter.UseBudgetLogic = filter.UseBudgetLogic;
		}

		
		public void Reload()
		{
			this.Clear();
			var records = searchTransactionUseCase.Execute(_filter.FilterDates, this.FilterText, this.FilterAccount);
			if (records?.Any() != true) return;

			this.LoadRecords(records);
		}

		public void Clear()
		{
			_listRecords.Clear();
		}

		private void LoadRecords(IEnumerable<IMoneyTransaction> records)
		{
			if (records?.Any() != true) return;

			foreach (var rec in records.OrderBy(o => o.TransactionDate).ThenBy(o => o.Description))
			{
				if(_filter.UseBudgetLogic)
				{
					// If either account is set to "NotSet", this typically means it's using the Special Accounts and aren't reasonable to show on the budget list
					if (rec.CreditAccount.JournalType == LedgerType.NotSet || rec.DebitAccount.JournalType == LedgerType.NotSet) continue;
				}

				if (rec is IMoneyTransaction je)
				{
					_listRecords.Add(new JournalEntryVM(je));
				}
			}
		}
	}

	public class TransDetailFilter
	{
		public IJournalAccount? Account;
		public DateRange FilterDates;
		public string SearchText;
		public bool AreFilterControlsVisible;
		public bool UseBudgetLogic;

		public bool IsFilterEnabled
		{
			get
			{
				if (this.FilterDates != null) return true;
				if (this.FilterDates?.Begin > DateTime.MinValue || this.FilterDates?.End < DateTime.MaxValue) return true;
				if (!string.IsNullOrWhiteSpace(this.SearchText)) return true;

				return false;
			}
		}

		public TransDetailFilter()
		{
			this.FilterDates = new DateRange(DateTime.MinValue, DateTime.MaxValue);
			this.SearchText = string.Empty;
			this.AreFilterControlsVisible = true;
			this.UseBudgetLogic = false;
		}

		public TransDetailFilter(IJournalAccount account, DateRange dates, string search, bool useBudget = false)
		{
			this.Account = account;
			this.FilterDates = dates ?? new DateRange(DateTime.MinValue, DateTime.MaxValue);
			this.SearchText = search;
			this.AreFilterControlsVisible = true;
			this.UseBudgetLogic = useBudget;
		}

		public void Clear()
		{
			this.FilterDates = new DateRange(DateTime.MinValue, DateTime.MaxValue);
			this.SearchText = string.Empty;
		}
	}
}