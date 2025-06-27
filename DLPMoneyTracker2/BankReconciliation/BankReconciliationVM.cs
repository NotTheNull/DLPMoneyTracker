
using DLPMoneyTracker.BusinessLogic.UseCases.BankReconciliation.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.BankReconciliation;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DLPMoneyTracker2.BankReconciliation
{
	public class BankReconciliationVM : BaseViewModel, IDisposable
	{
        private readonly IGetReconciliationTransactionsUseCase getBankRecTransactionsUseCase;
        private readonly ISaveReconciliationUseCase saveReconciliationUseCase;
        private readonly NotificationSystem notifications;

        public BankReconciliationVM(
			IGetReconciliationTransactionsUseCase getBankRecTransactionsUseCase,
			ISaveReconciliationUseCase saveReconciliationUseCase,
			NotificationSystem notifications)
		{
            this.getBankRecTransactionsUseCase = getBankRecTransactionsUseCase;
            this.saveReconciliationUseCase = saveReconciliationUseCase;
            this.notifications = notifications;
            this.notifications.TransactionsModified += Notifications_TransactionsModified;
            this.notifications.BankDateChanged += Notifications_BankDateChanged;
        }
        ~BankReconciliationVM() { this.Dispose(); }



        private void Notifications_BankDateChanged(Guid moneyAccountUID)
        {
			NotifyReconciledChange();   
        }

        private void Notifications_TransactionsModified(Guid debitAccountId, Guid creditAccountId)
        {
            this.LoadCurrentTransactions();
        }

		
		private IJournalAccount _account = null!;
		private IMoneyAccount MoneyAccount { get { return (IMoneyAccount)_account; } }
		public string AccountDescription { get { return _account.Description; } }


		private readonly DateRange _statementDate = new();
		public DateTime StartingDate 
		{
			get { return _statementDate.Begin; }
			set
			{
				_statementDate.Begin = value;
				NotifyPropertyChanged(nameof(StartingDate));
			}
		}
	
		public DateTime EndingDate 
		{
			get { return _statementDate.End; }
			set
			{
				_statementDate.End = value;
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



		private readonly ObservableCollection<SingleAccountDetailVM> _listTrans = [];
		public ObservableCollection<SingleAccountDetailVM> TransactionList { get { return _listTrans; }  }

		private List<SingleAccountDetailVM> ReconcileList 
		{
			get
			{ 
				return [.. _listTrans.Where(x => x.BankDate.HasValue && _statementDate.IsWithinRange(x.BankDate.Value))]; 
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


		public RelayCommand CommandLoadTransactions => 
			new((o) =>
			{
				this.LoadCurrentTransactions();
				NotifyReconciledChange();
			});




		public void LoadAccount(IJournalAccount account)
		{
			if (account is not IMoneyAccount) throw new InvalidOperationException("Selected Account MUST be a Money Account");
			
			_account = account;
		
			this.LoadCurrentTransactions();
			NotifyPropertyChanged(nameof(AccountDescription));
			NotifyReconciledChange();
		}

		

		private void LoadCurrentTransactions()
		{
			if (_statementDate is null) return;

			_listTrans.Clear();
			var listRecords = getBankRecTransactionsUseCase.Execute(_account.Id, _statementDate);
			foreach(var t in listRecords)
			{
				_listTrans.Add(new SingleAccountDetailVM(_account, t, notifications));
			}
		}


		public void AddTransaction(IMoneyTransaction record)
		{
			SingleAccountDetailVM vm = new(_account, record, notifications);					

            if (_listTrans.Contains(vm)) return;
            _listTrans.Add(vm);
        }

		public void RemoveTransaction(SingleAccountDetailVM record)
		{
			if (!_listTrans.Contains(record)) return;

			record.BankDate = null;
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

			BankReconciliationDTO reconciliation = new()
			{
				BankAccount = _account,
				StatementDate = _statementDate,
				StartingBalance = this.StartingBalance,
				EndingBalance = this.EndingBalance
			};
			saveReconciliationUseCase.Execute(reconciliation);

		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
            this.notifications.TransactionsModified -= Notifications_TransactionsModified;
            this.notifications.BankDateChanged -= Notifications_BankDateChanged;
        }
	}
}
