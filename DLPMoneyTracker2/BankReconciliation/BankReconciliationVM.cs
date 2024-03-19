
using DLPMoneyTracker.BusinessLogic.UseCases.BankReconciliation.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.BankReconciliation;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.Core;
using Microsoft.Extensions.DependencyInjection;
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
		
			this.LoadCurrentTransactions();
			NotifyPropertyChanged(nameof(AccountDescription));
			NotifyReconciledChange();
		}

		

		private void LoadCurrentTransactions()
		{
			if (statementDate is null) return;

			_listTrans.Clear();
			var listRecords = getBankRecTransactionsUseCase.Execute(_account.Id, statementDate);
			foreach(var t in listRecords)
			{
				_listTrans.Add(new SingleAccountDetailVM(_account, t, notifications));
			}
		}


		public void AddTransaction(IMoneyTransaction record)
		{
			SingleAccountDetailVM vm = new SingleAccountDetailVM(_account, record, notifications);					

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

			BankReconciliationDTO reconciliation = new BankReconciliationDTO()
			{
				BankAccount = _account,
				StatementDate = statementDate,
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
