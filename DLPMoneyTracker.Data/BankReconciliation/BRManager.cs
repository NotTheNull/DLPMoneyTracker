using DLPMoneyTracker.Data.Common;
using DLPMoneyTracker.Data.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.BankReconciliation
{
	public interface IBRManager
	{
		IBankReconciliation Build(IJournalAccount account, DateRange dates);
		string GetPath(IJournalAccount account, DateTime statementDate);
		IBankReconciliation ReadFromFile(IJournalAccount account, DateTime statementDate);
		void WriteToFile(IBankReconciliation rec);
	}

	public class BRManager : IBRManager
	{
		private readonly IJournal journal;

		public BRManager(IJournal journal)
		{
			this.journal = journal;
		}

		public string GetPath(IJournalAccount account, DateTime statementDate)
		{
			if (!Directory.Exists(AppConfigSettings.RECONCILE_FOLDER_PATH))
			{
				Directory.CreateDirectory(AppConfigSettings.RECONCILE_FOLDER_PATH);
			}

			return string.Format("{0}{1}_{2:yyyyMMdd}.json", AppConfigSettings.RECONCILE_FOLDER_PATH, account.Id, statementDate);
		}

		public IBankReconciliation Build(IJournalAccount account, DateRange dates)
		{
			if (account is null) throw new ArgumentNullException(nameof(IJournalAccount));
			if (!(account is IMoneyAccount)) throw new InvalidOperationException("Account is not a Money Account");
			if (dates is null) throw new ArgumentNullException(nameof(DateRange));

			IBankReconciliation rec = this.ReadFromFile(account, dates.End);
			if (rec is null)
			{
				rec = new BankReconciliation(account, dates);
			}

			var listTransactions = journal.GetReconciledRecords(account, dates);
            foreach (var trans in listTransactions)
            {
				rec.AddTransaction(trans);
            }

            return rec;
		}

		public void WriteToFile(IBankReconciliation rec)
		{
			if (rec is null) throw new ArgumentNullException(nameof(IBankReconciliation));

			string json = JsonSerializer.Serialize(rec);
			string pathFile = this.GetPath(rec.BankAccount, rec.StatementDateRange.End);
			File.WriteAllText(pathFile, json);
		}

		public IBankReconciliation ReadFromFile(IJournalAccount account, DateTime dateStatement)
		{
			string filePath = this.GetPath(account, dateStatement);
			if (!File.Exists(filePath)) return null;

			string json = File.ReadAllText(filePath);
			if (string.IsNullOrWhiteSpace(json)) return null;

			BankReconciliationJSON jsonCopy = JsonSerializer.Deserialize<BankReconciliationJSON>(json);
			if (jsonCopy is null) return null;

			return new BankReconciliation(jsonCopy);
		}
	}
}
