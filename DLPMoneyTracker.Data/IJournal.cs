using DLPMoneyTracker.Data.Common;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.TransactionModels;
using DLPMoneyTracker.Data.TransactionModels.JournalPlan;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DLPMoneyTracker.Data
{
	public delegate void JournalModifiedHandler();

	public class JournalSearchFilter
	{
		public DateRange? DateRange;
		public String? SearchText;
		public IJournalAccount? Account;

		public JournalSearchFilter(IJournalPlan plan, IJournalAccount acct)
		{
			DateRange = new DateRange(plan.NotificationDate, plan.NextOccurrence);
			SearchText = plan.Description;
			Account = acct;
		}
		public JournalSearchFilter(IJournalAccount account, DateRange dates)
		{
			this.DateRange = dates;
			this.Account = account;
			SearchText = null;
		}
	}

	public interface IJournal
	{
		event JournalModifiedHandler JournalModified;

		string FilePath { get; }
		ReadOnlyCollection<IJournalEntry> TransactionList { get; }

		void LoadFromFile(int year);

		void SaveToFile();

		void AddUpdateTransaction(IJournalEntry trans);


		IEnumerable<IJournalEntry> Search(JournalSearchFilter filter);

		/// <summary>
		/// Get list of records for the given journal account that have a Bank Date within the given range
		/// OR does not have the date set.  Will make sure to exclude Initial Balance records
		/// </summary>
		/// <param name="account"></param>
		/// <param name="dates"></param>
		/// <returns></returns>
		IEnumerable<IJournalEntry> GetReconciledRecords(IJournalAccount account, DateRange dates);



		decimal GetAccountBalance(Guid ledgerAccountId, bool isBudgetBalance);

		decimal GetAccountBalance_CurrentMonth(Guid ledgerAccountId, bool isBudgetBalance);

		decimal GetAccountBalance_Month(Guid ledgerAccountId, bool isBudgetBalance, int year, int month);

		decimal GetAccountBalance_Range(Guid ledgerAccountId, bool isBudgetBalance, DateTime beg, DateTime end);

		void BuildInitialBalances(IJournal oldJournal);

		//#pragma warning disable CS0612 // Type or member is obsolete
		//        void Convert(ILedger ledger);
		//#pragma warning restore CS0612 // Type or member is obsolete
	}

	public class DLPJournal : IJournal
	{
		public event JournalModifiedHandler JournalModified;

		private readonly ITrackerConfig _config;
		private int _year;

		public DLPJournal(ITrackerConfig config) : this(config, DateTime.Today.Year)
		{
		}

		public DLPJournal(ITrackerConfig config, int year)
		{
			_config = config;
			this.LoadFromFile(year);
		}

		private string FolderPath
		{ get { return AppConfigSettings.DATA_FOLDER_PATH.Replace(AppConfigSettings.YEAR_FOLDER_PLACEHOLDER, _year.ToString()); } }
		public string FilePath
		{ get { return string.Concat(this.FolderPath, "Journal.json"); } }

		private List<IJournalEntry> _listTransactions = new List<IJournalEntry>();
		public ReadOnlyCollection<IJournalEntry> TransactionList
		{ get { return _listTransactions.AsReadOnly(); } }


		public void AddUpdateTransaction(IJournalEntry trans)
		{
			var record = _listTransactions.FirstOrDefault(x => x.Id == trans.Id);
			if (record is null)
			{
				_listTransactions.Add(trans);
			}
			else
			{
				record.Copy(trans);
			}
			this.SaveToFile();
		}

		



		public IEnumerable<IJournalEntry> Search(JournalSearchFilter filter)
		{
			if (filter is null) return null;

			var listSearch = TransactionList.Where(x => x.Id != Guid.Empty);
			if (filter.DateRange != null)
			{
				listSearch = listSearch.Where(x => filter.DateRange.IsWithinRange(x.TransactionDate));
			}

			if (filter.Account != null)
			{
				listSearch = listSearch.Where(x => x.CreditAccountId == filter.Account.Id || x.DebitAccountId == filter.Account.Id);
			}

			if (!string.IsNullOrWhiteSpace(filter.SearchText))
			{
				listSearch = listSearch.Where(x => x.Description.Contains(filter.SearchText));
			}

			return listSearch.ToList();
		}

		public decimal GetAccountBalance(Guid ledgerAccountId, bool isBudgetBalance)
		{
			return GetAccountBalance_Range(ledgerAccountId, isBudgetBalance, DateTime.MinValue, DateTime.MaxValue);
		}

		public decimal GetAccountBalance_CurrentMonth(Guid ledgerAccountId, bool isBudgetBalance)
		{
			return GetAccountBalance_Month(ledgerAccountId, isBudgetBalance, DateTime.Today.Year, DateTime.Today.Month);
		}

		public decimal GetAccountBalance_Month(Guid ledgerAccountId, bool isBudgetBalance, int year, int month)
		{
			if (month < 1 || month > 12) throw new InvalidOperationException(String.Format("Month #{0} is not valid", month));

			DateTime beg = new DateTime(year, month, 1);
			int dayCount = DateTime.DaysInMonth(year, month);
			DateTime end = new DateTime(year, month, dayCount).AddDays(1);

			return GetAccountBalance_Range(ledgerAccountId, isBudgetBalance, beg, end);
		}

		public decimal GetAccountBalance_Range(Guid ledgerAccountId, bool isBudgetBalance, DateTime beg, DateTime end)
		{
			bool doesFilterApply(IJournalEntry entry)
			{
				return
					(
						entry.DebitAccountId == ledgerAccountId
						|| entry.CreditAccountId == ledgerAccountId
					)
					&& entry.TransactionDate.Date >= beg.Date
					&& entry.TransactionDate.Date <= end.Date;
			};

			decimal balance = decimal.Zero;
			if (_listTransactions.Any(doesFilterApply))
			{
				foreach (var t in _listTransactions.Where(doesFilterApply))
				{
					if (isBudgetBalance)
					{
						var accountCredit = _config.GetJournalAccount(t.CreditAccountId);
						if (accountCredit is ILedgerAccount ledgerCredit)
						{
							if (ledgerCredit.ExcludeFromBudget) continue;
						}

						var accountDebit = _config.GetJournalAccount(t.DebitAccountId);
						if (accountDebit is ILedgerAccount ledgerDebit)
						{
							if (ledgerDebit.ExcludeFromBudget) continue; 
						}
					}

					if (t.DebitAccountId == ledgerAccountId)
					{
						balance += t.TransactionAmount;
					}
					else
					{
						balance -= t.TransactionAmount;
					}
				}
			}

			return balance;
		}

		public void LoadFromFile(int year)
		{
			_year = year;
			if (!Directory.Exists(this.FolderPath))
			{
				Directory.CreateDirectory(this.FolderPath);
			}

			if (_listTransactions is null) _listTransactions = new List<IJournalEntry>();
			_listTransactions.Clear();
			if (!File.Exists(FilePath)) return;

			string json = File.ReadAllText(FilePath);
			if (string.IsNullOrWhiteSpace(json)) return;

			var dataList = (List<JournalEntryJSON>)JsonSerializer.Deserialize(json, typeof(List<JournalEntryJSON>));
			if (dataList?.Any() != true) return;

			foreach (var trans in dataList)
			{
				JournalEntry record = new JournalEntry(_config, trans);
				_listTransactions.Add(record);
			}
		}

		public void SaveToFile()
		{
			string json = JsonSerializer.Serialize(_listTransactions);
			File.WriteAllText(FilePath, json);
			JournalModified?.Invoke();
		}

		/// <summary>
		/// Rebuilds the initial balances for Money Accounts.  Payables and Receivables do NOT carry over
		/// </summary>
		/// <param name="oldJournal"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public void BuildInitialBalances(IJournal oldJournal)
		{
			if (oldJournal is null) throw new ArgumentNullException("Journal");

			JournalAccountSearch search = new JournalAccountSearch(new List<JournalAccountType>() { JournalAccountType.Bank, JournalAccountType.LiabilityCard, JournalAccountType.LiabilityLoan });

			_listTransactions.RemoveAll(x => x.DebitAccountId == SpecialAccount.InitialBalance.Id || x.CreditAccountId == SpecialAccount.InitialBalance.Id);
			var listAccounts = _config.GetJournalAccountList(search);
			foreach (var account in listAccounts)
			{
				JournalEntry record = new JournalEntry(_config)
				{
					TransactionDate = DateTime.MinValue,
					Description = SpecialAccount.InitialBalance.Description,
					TransactionAmount = oldJournal.GetAccountBalance(account.Id, false)
				};

				if (account is BankAccount bank)
				{
					record.DebitAccount = bank;
					record.CreditAccount = SpecialAccount.InitialBalance;
				}
				else if (account is CreditCardAccount creditCard)
				{
					record.DebitAccount = SpecialAccount.InitialBalance;
					record.CreditAccount = creditCard;
					record.TransactionAmount *= -1; // Sum of records will be negative
				}
				else if (account is LoanAccount loan)
				{
					record.DebitAccount = SpecialAccount.InitialBalance;
					record.CreditAccount = loan;
					record.TransactionAmount *= -1; // Sum of records will be negative
				}

				this.AddUpdateTransaction(record);
			}
		}

		public IEnumerable<IJournalEntry> GetReconciledRecords(IJournalAccount account, DateRange dates)
		{
			List<Guid> listExcludeAccountsIDs = new List<Guid>();
			listExcludeAccountsIDs.Add(SpecialAccount.InitialBalance.Id);

			return (from t in _listTransactions
					where
						(
							t.CreditAccountId == account.Id && 
							!listExcludeAccountsIDs.Contains(t.DebitAccountId) &&
							(dates.IsWithinRange(t.CreditBankDate) || !t.CreditBankDate.HasValue)
						)
						|| 
						(
							t.DebitAccountId == account.Id && 
							!listExcludeAccountsIDs.Contains(t.CreditAccountId) &&
							(dates.IsWithinRange(t.DebitBankDate) || t.DebitBankDate.HasValue)
						)
					select t)
					.ToList();

		}
	}
}