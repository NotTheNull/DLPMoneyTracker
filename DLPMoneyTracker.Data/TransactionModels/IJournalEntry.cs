using DLPMoneyTracker.Data.LedgerAccounts;
using System;
using System.Text.Json.Serialization;

namespace DLPMoneyTracker.Data.TransactionModels
{
	public enum TransactionType
	{
		NotSet,
		Expense,
		Income,
		DebtPayment,
		DebtAdjustment,
		Correction,
		Transfer
	}

	public interface IJournalEntry
	{
		Guid Id { get; }
		DateTime TransactionDate { get; }
		TransactionType JournalEntryType { get; }

		Guid DebitAccountId { get; }
		string DebitAccountName { get; }
		DateTime? DebitBankDate { get; }

		Guid CreditAccountId { get; }
		string CreditAccountName { get; }
		DateTime? CreditBankDate { get; }

		string Description { get; }
		decimal TransactionAmount { get; }

		void Copy(IJournalEntry cpy);
	}

	public class JournalEntryJSON : IJournalEntry
	{
		public Guid Id { get; set; }

		public DateTime TransactionDate { get; set; }
		public TransactionType JournalEntryType { get; set; } = TransactionType.NotSet;


		public Guid DebitAccountId { get; set; }

		[JsonIgnore]
		public string DebitAccountName { get; set; }
		public DateTime? DebitBankDate { get; set; }


		public Guid CreditAccountId { get; set; }

		[JsonIgnore]
		public string CreditAccountName { get; set; }
		public DateTime? CreditBankDate { get; set; }



		public string Description { get; set; }

		public decimal TransactionAmount { get; set; }

		public void Copy(IJournalEntry cpy)
		{
			Id = cpy.Id;
			TransactionDate = cpy.TransactionDate;
			JournalEntryType = cpy.JournalEntryType;
			DebitAccountId = cpy.CreditAccountId;
			CreditAccountId = cpy.CreditAccountId;
			Description = cpy.Description;
			TransactionAmount = cpy.TransactionAmount;
		}
	}

	public class JournalEntry : IJournalEntry
	{
		private readonly ITrackerConfig _config;

		public JournalEntry(ITrackerConfig config)
		{
			this.Id = Guid.NewGuid();
			this.TransactionDate = DateTime.Now;
			this.Description = string.Empty;
			this.TransactionAmount = decimal.Zero;
			_config = config;
		}

		public JournalEntry(ITrackerConfig config, IJournalEntry cpy)
		{
			_config = config;
			this.Copy(cpy);
		}

		public Guid Id { get; set; }
		public DateTime TransactionDate { get; set; }
		public TransactionType JournalEntryType { get; set; }


		public IJournalAccount DebitAccount { get; set; }

		public Guid DebitAccountId { get { return DebitAccount?.Id ?? Guid.Empty; } }

		public string DebitAccountName { get { return DebitAccount?.Description ?? string.Empty; } }

		public DateTime? DebitBankDate { get; set; }



		public IJournalAccount CreditAccount { get; set; }

		public Guid CreditAccountId { get { return CreditAccount?.Id ?? Guid.Empty; } }

		public string CreditAccountName { get { return CreditAccount?.Description ?? string.Empty; } }

		public DateTime? CreditBankDate { get; set; }



		public string Description { get; set; }
		public decimal TransactionAmount { get; set; }

		public void Copy(IJournalEntry cpy)
		{
			if (cpy is JournalEntryJSON json)
			{
				this.Id = json.Id;
				this.TransactionDate = json.TransactionDate;
				this.JournalEntryType = json.JournalEntryType;

				this.Description = json.Description;
				this.TransactionAmount = json.TransactionAmount;

				this.DebitAccount = _config.GetJournalAccount(json.DebitAccountId);
				this.CreditAccount = _config.GetJournalAccount(json.CreditAccountId);

				this.DebitBankDate = json.DebitBankDate;
				this.CreditBankDate = json.CreditBankDate;
			}
			else if (cpy is JournalEntry je)
			{
				this.Id = je.Id;
				this.TransactionDate = je.TransactionDate;
				this.JournalEntryType = je.JournalEntryType;

				this.Description = je.Description;
				this.TransactionAmount = je.TransactionAmount;

				this.DebitAccount = je.DebitAccount;
				this.DebitBankDate = je.DebitBankDate;

				this.CreditAccount = je.CreditAccount;
				this.CreditBankDate = je.CreditBankDate;

			}
		}
	}
}