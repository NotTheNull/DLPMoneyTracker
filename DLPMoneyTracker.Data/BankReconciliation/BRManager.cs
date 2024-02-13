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
	public delegate void ReconciliationChangedHandler(Guid accountId, IBankReconciliationFile reconciliation);

	public interface IBRManager
	{
		event ReconciliationChangedHandler ReconciliationChanged;
		IBankReconciliation Build(IJournalAccount account, DateRange dates);
		string GetPath(Guid accountId);
		IBankReconciliation GetReconciliation(Guid accountId, DateTime statementDate);
		void WriteToFile(IBankReconciliation rec);
		IBankReconciliationFile GetReconciliationFile(Guid accountId);
	}

	public class BRManager : IBRManager
	{
		public event ReconciliationChangedHandler ReconciliationChanged;

		private readonly IJournal journal;
		private readonly ITrackerConfig config;

		public BRManager(IJournal journal, ITrackerConfig config)
		{
			this.journal = journal;
			this.config = config;
		}

		private string ReconcileFolderPath
		{
			get
			{
				return AppConfigSettings.RECONCILE_FOLDER_PATH.Replace(AppConfigSettings.YEAR_FOLDER_PLACEHOLDER, DateTime.Today.Year.ToString());
			}
		}

		public string GetPath(Guid accountId)
		{
			
			if (!Directory.Exists(this.ReconcileFolderPath))
			{
				Directory.CreateDirectory(this.ReconcileFolderPath);
			}

			return string.Format("{0}{1}.json", this.ReconcileFolderPath, accountId);
		}

		public IBankReconciliation Build(IJournalAccount account, DateRange dates)
		{
			if (account is null) throw new ArgumentNullException(nameof(IJournalAccount));
			if (!(account is IMoneyAccount)) throw new InvalidOperationException("Account is not a Money Account");
			if (dates is null) throw new ArgumentNullException(nameof(DateRange));

			IBankReconciliation rec = this.GetReconciliation(account.Id, dates.End);
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

			var jsonFile = ReadFile(rec.BankAccountId);
			if(jsonFile is null)
			{
				jsonFile = new BankReconciliationFileJSON() { AccountId = rec.BankAccountId };
			}
			jsonFile.Update(rec);

			string json = JsonSerializer.Serialize(jsonFile);
			string pathFile = this.GetPath(rec.BankAccountId);
			File.WriteAllText(pathFile, json);

			ReconciliationChanged?.Invoke(rec.BankAccountId, ConvertJSONToFile(jsonFile));
		}

		public IBankReconciliation GetReconciliation(Guid accountId, DateTime dateStatement)
		{
			var file = this.GetReconciliationFile(accountId);
			return file.ReconciliationList.FirstOrDefault(s => s.EndingDate == dateStatement);
		}

		public IBankReconciliationFile GetReconciliationFile(Guid accountId)
		{
			var file = this.ReadFile(accountId);
			if (file is null) file = new BankReconciliationFileJSON() { AccountId = accountId };

			return this.ConvertJSONToFile(file);
		}


		private BankReconciliationFileJSON ReadFile(Guid accountId)
		{
			string filePath = this.GetPath(accountId);
			if (!File.Exists(filePath)) return null;

			string json = File.ReadAllText(filePath);
			if (string.IsNullOrWhiteSpace(json)) return null;

			return JsonSerializer.Deserialize<BankReconciliationFileJSON>(json);
		}


		private IBankReconciliationFile ConvertJSONToFile(BankReconciliationFileJSON json)
		{
			IJournalAccount account = config.GetJournalAccount(json.AccountId);
			BankReconciliationFile file = new BankReconciliationFile(account);

			foreach(var t in json.ReconciliationList)
			{
				BankReconciliation record = new BankReconciliation(t, config);
				file.ReconciliationList.Add(record);
			}

			return file;
		}

		private BankReconciliationFileJSON ConvertFileToJSON(IBankReconciliationFile file)
		{
			BankReconciliationFileJSON jsonFile = new BankReconciliationFileJSON() { AccountId = file.AccountId };

			foreach(var t in file.ReconciliationList)
			{
				BankReconciliationJSON jsonRec = new BankReconciliationJSON();
				jsonRec.Copy(t);

				jsonFile.ReconciliationList.Add(jsonRec);
			}

			return jsonFile;
		}
	}
}
