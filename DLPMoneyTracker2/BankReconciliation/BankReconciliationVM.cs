
using DLPMoneyTracker.Data.BankReconciliation;
using DLPMoneyTracker.Data.Common;

using DLPMoneyTracker.Data.TransactionModels;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DLPMoneyTracker2.BankReconciliation
{
	public class BankReconciliationVM : BaseViewModel, IDisposable
	{
		private readonly ITrackerConfig config;
		private readonly IJournal journal;
		private readonly IBRManager bankManager;

		public BankReconciliationVM(ITrackerConfig config, IJournal journal, IBRManager bankManager)
		{
			this.config = config;
			this.journal = journal;
			this.journal.JournalModified += Journal_JournalModified;
			this.bankManager = bankManager;
		}
		~BankReconciliationVM() { this.Dispose(); }
		
		private IJournalAccount _account;
		private IMoneyAccount MoneyAccount { get { return (IMoneyAccount)_account; } }
		public string AccountDescription { get { return _account.Description; } }


		private DateRange statementDate = new DateRange();
		public DateTime StartingDate 
		{
			get { return statementDate.Begin; }
			set
			{
				statementDate.Begin = value;
				NotifyPropertyChanged(nameof(StartingDate));
			}
		}
	
		public DateTime EndingDate 
		{
			get { return statementDate.End; }
			set
			{
				statementDate.End = value;
				NotifyPropertyChanged(nameof(EndingDate));
				this.LoadCurrentTransactions();
			}
		}
		

		private decimal _startBal;

		public decimal StartingBalance
		{
			get { return _startBal; }
			set 
			{ 
				_startBal = value;
				NotifyPropertyChanged(nameof(StartingBalance));
			}

		}

		private decimal _endBal;

		public decimal EndingBalance
		{
			get { return _endBal; }
			set
			{
				_endBal = value;
				NotifyPropertyChanged(nameof(EndingBalance));
				NotifyReconciledChange();
			}
		}



		private ObservableCollection<SingleAccountDetailVM> _listTrans = new ObservableCollection<SingleAccountDetailVM>();
		public ObservableCollection<SingleAccountDetailVM> TransactionList { get { return _listTrans; }  }

		private List<SingleAccountDetailVM> ReconcileList 
		{
			get
			{ 
				return _listTrans
					.Where(x => x.BankDate.HasValue && statementDate.IsWithinRange(x.BankDate.Value))
					.ToList(); 
			} 
		}


		public decimal ReconciledSum
		{
			get
			{
				return this.ReconcileList.Sum(s => s.TransactionAmount);
			}
		}


		public decimal ReconcileBalance
		{
			get
			{
				return this.StartingBalance + ReconciledSum;
			}
		}

		public decimal ReconcileDiscrepancy
		{
			get
			{
				return this.EndingBalance - this.ReconcileBalance;
			}
		}


		public bool IsBalanced
		{
			get
			{
				return this.ReconcileBalance == this.EndingBalance;
			}
		}



		private RelayCommand _cmdLoadTrans;
		public RelayCommand CommandLoadTransactions
		{
			get
			{
				return _cmdLoadTrans ?? (_cmdLoadTrans = new RelayCommand((o) =>
				{
					this.LoadCurrentTransactions();
					NotifyReconciledChange();
				}));
			}
		}




		public void LoadAccount(IJournalAccount account)
		{
			if (!(account is IMoneyAccount)) throw new InvalidOperationException("Selected Account MUST be a Money Account");
			
			_account = account;
			this.LoadLastReconciliation();
			this.LoadCurrentTransactions();
			NotifyPropertyChanged(nameof(AccountDescription));
			NotifyReconciledChange();
		}

		/// <summary>
		/// Will prefill certain properties with previous reconciliation's data
		/// </summary>
		private void LoadLastReconciliation()
		{
			if (!this.MoneyAccount.PreviousBankReconciliationStatementDate.HasValue) return;

			var prevReconciliation = bankManager.GetReconciliation(_account.Id, this.MoneyAccount.PreviousBankReconciliationStatementDate.Value);
			if (prevReconciliation is null) return;

			this.StartingDate = prevReconciliation.EndingDate.AddDays(1);
			this.StartingBalance = prevReconciliation.EndingBalance;
		}

		private void LoadCurrentTransactions()
		{
			if (statementDate is null) return;

			_listTrans.Clear();
			var listRecords = journal.GetReconciledRecords(_account, statementDate);
			foreach(var t in listRecords)
			{
				_listTrans.Add(new SingleAccountDetailVM(_account, t));
			}
		}


		public void AddTransaction(IMoneyTransaction record)
		{
			SingleAccountDetailVM vm = new SingleAccountDetailVM(_account, record);
			this.AddTransaction(vm);
		}

		public void AddTransaction(SingleAccountDetailVM record)
		{			
			if (_listTrans.Contains(record)) return;
			record.BankDateChanged += LocalBankDateChanged;
			_listTrans.Add(record);
		}

		public void RemoveTransaction(SingleAccountDetailVM record)
		{
			if (!_listTrans.Contains(record)) return;

			record.BankDate = null;
		}


		private void Journal_JournalModified()
		{
			this.LoadCurrentTransactions();
		}

		private void LocalBankDateChanged()
		{
			NotifyReconciledChange();
		}

		private void NotifyReconciledChange()
		{
			NotifyPropertyChanged(nameof(ReconcileList));
			NotifyPropertyChanged(nameof(ReconciledSum));
			NotifyPropertyChanged(nameof(ReconcileBalance));
			NotifyPropertyChanged(nameof(IsBalanced));
			NotifyPropertyChanged(nameof(ReconcileDiscrepancy));
		}


		public void Save()
		{
			if (!IsBalanced) return;

			IBankReconciliation reconciliation = new DLPMoneyTracker.Data.BankReconciliation.BankReconciliation(_account, statementDate)
			{
				StartingBalance = this.StartingBalance,
				EndingBalance = this.EndingBalance
			};

			foreach(var transaction in this.ReconcileList.Select(s => s.Source))
			{
				reconciliation.AddTransaction(transaction);
			}


			bankManager.WriteToFile(reconciliation);

		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			this.journal.JournalModified -= Journal_JournalModified;

			if (this.TransactionList.Any() != true) return;
			foreach(var t in this.TransactionList)
			{
				t.BankDateChanged -= LocalBankDateChanged;
			}
		}
	}
}
