using DLPMoneyTracker.Data.Common;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.TransactionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DLPMoneyTracker.Data.BankReconciliation
{
	public interface IBankReconciliation
	{
		IJournalAccount BankAccount { get; } // Can be Bank or Credit card
		DateRange StatementDateRange { get; }
		decimal StartingBalance { get; }
		decimal EndingBalance { get; }

		List<IJournalEntry> TransactionList { get; }


		void Copy(IBankReconciliation cpy);
		void AddTransaction(IJournalEntry trans);
		void RemoveTransaction(IJournalEntry trans);
		void ClearTransactions();

	}

	// Intended to write to individual JSON files by Account and Ending Date
	public class BankReconciliationJSON : IBankReconciliation
	{
		public IJournalAccount BankAccount { get; set; }

		public DateRange StatementDateRange { get; set; }

		public decimal StartingBalance { get; set; }

		public decimal EndingBalance { get; set; }


		private List<IJournalEntry> listTransactions = new List<IJournalEntry>();

		public List<IJournalEntry> TransactionList { get { return listTransactions; } }






		public void AddTransaction(IJournalEntry trans)
		{
			if (listTransactions.Contains(trans)) return;
			listTransactions.Add(trans);
		}
		public void RemoveTransaction(IJournalEntry trans)
		{
			if (!listTransactions.Contains(trans)) return;
			listTransactions.Remove(trans);
		}

		public void ClearTransactions()
		{
			listTransactions.Clear();
		}

		public void Copy(IBankReconciliation cpy)
		{
			this.BankAccount = cpy.BankAccount;
			this.StatementDateRange = cpy.StatementDateRange;
			this.StartingBalance = cpy.StartingBalance;
			this.EndingBalance = cpy.EndingBalance;

			listTransactions.Clear();
			if (cpy.TransactionList?.Any() == true)
			{
				listTransactions.AddRange(cpy.TransactionList);
			}
		}

	}


	public class BankReconciliation : IBankReconciliation
	{
		private readonly IJournalAccount account;
		private readonly DateRange dateRange;

		public BankReconciliation(IJournalAccount account, DateRange dateRange)
		{
			if (!(account is IMoneyAccount)) throw new InvalidCastException($"Account [{account.Description}] is NOT a Money Account");
			if (dateRange is null) throw new ArgumentNullException(nameof(dateRange));

			this.account = account;
			this.dateRange = dateRange;
		}
        public BankReconciliation(IBankReconciliation cpy) : this(cpy.BankAccount, cpy.StatementDateRange)
        {
			this.Copy(cpy);
        }



        public IJournalAccount BankAccount { get { return account; } }

		public DateRange StatementDateRange { get { return dateRange; } }

		public decimal StartingBalance { get; set; }

		public decimal EndingBalance { get; set; }

		private List<IJournalEntry> _listTrans = new List<IJournalEntry>();
		public List<IJournalEntry> TransactionList { get { return _listTrans; } }

		public void Copy(IBankReconciliation cpy)
		{
			if (cpy.BankAccount != account) throw new InvalidOperationException("Bank accounts do not match");

			this.StartingBalance = cpy.StartingBalance;
			this.EndingBalance = cpy.EndingBalance;

			_listTrans.Clear();
			if(cpy.TransactionList?.Any() == true)
			{
				_listTrans.AddRange(cpy.TransactionList);
			}
		}

		public void AddTransaction(IJournalEntry trans)
		{
			if (_listTrans.Contains(trans)) return;
			_listTrans.Add(trans);
		}

		public void RemoveTransaction(IJournalEntry trans)
		{
			if (!_listTrans.Contains(trans)) return;
			_listTrans.Remove(trans);
		}

		public void ClearTransactions()
		{
			_listTrans.Clear();
		}
	}

}
