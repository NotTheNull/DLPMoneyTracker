using DLPMoneyTracker.Data.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.BankReconciliation
{
	// Each Money Account will have a separate file
	public interface IBankReconciliationFile
	{
		Guid AccountId { get; }

		List<IBankReconciliation> ReconciliationList {get;}
	}

	public class BankReconciliationFileJSON 
	{
        public Guid AccountId { get; set; }

		public List<BankReconciliationJSON> ReconciliationList { get; set; } = new List<BankReconciliationJSON>();

		public void Copy(IBankReconciliationFile fileData)
		{
			this.AccountId = fileData.AccountId;

			this.ReconciliationList.Clear();
			foreach(var record in fileData.ReconciliationList)
			{
				BankReconciliationJSON json = new BankReconciliationJSON();
				json.Copy(record);

				this.ReconciliationList.Add(json);
			}
		}

		public void Update(IBankReconciliation rec)
		{
			if (rec.BankAccountId != this.AccountId) throw new InvalidOperationException("Bank Accounts do not match");

			var json = this.ReconciliationList.FirstOrDefault(x => x.EndingDate == rec.EndingDate);
			if(json is null)
			{
				json = new BankReconciliationJSON();
				this.ReconciliationList.Add(json);
			}
			json.Copy(rec);
		}
    }


	public class BankReconciliationFile : IBankReconciliationFile
	{
		private readonly IJournalAccount account;

		public BankReconciliationFile(IJournalAccount account)
        {
			this.account = account;
		}

		public Guid AccountId { get { return account.Id; } }

		public List<IBankReconciliation> ReconciliationList { get; set; } = new List<IBankReconciliation>();
	}
}
