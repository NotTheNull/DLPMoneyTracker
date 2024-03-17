using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;

namespace DLPMoneyTracker2.Config.AddEditLedgerAccounts
{
	public class LedgerAccountVM : BaseViewModel
	{
		private readonly ITrackerConfig _config;

		private static readonly List<LedgerType> _listValidTypes = new List<LedgerType>() { LedgerType.Payable, LedgerType.Receivable };

		public static List<LedgerType> ValidTypes
		{ get { return _listValidTypes; } }

		public LedgerAccountVM(ITrackerConfig config) : base()
		{
			_config = config;
		}

		public LedgerAccountVM(ITrackerConfig config, IJournalAccount act) : base()
		{
			_config = config;
			this.LoadAccount(act);
		}

		public Guid Id { get; private set; }

		private string _desc = string.Empty;

		public string Description
		{
			get { return _desc; }
			set
			{
				_desc = value;
				NotifyPropertyChanged(nameof(Description));
			}
		}

		private LedgerType _acctType;

		public LedgerType JournalType
		{
			get { return _acctType; }
			set
			{
				_acctType = value;
				NotifyPropertyChanged(nameof(JournalType));
			}
		}

		private decimal _budget;

		public decimal MonthlyBudget
		{
			get { return _budget; }
			set
			{
				_budget = value;
				NotifyPropertyChanged(nameof(MonthlyBudget));
			}
		}

		private DateTime? _closeDateUTC;

		public DateTime? DateClosedUTC
		{
			get { return _closeDateUTC?.ToLocalTime(); }
			set
			{
				_closeDateUTC = value;
				NotifyPropertyChanged(nameof(DateClosedUTC));
				NotifyPropertyChanged(nameof(IsClosed));
				NotifyPropertyChanged(nameof(DisplayClosedMessage));
			}
		}

		public bool IsClosed
		{ get { return _closeDateUTC.HasValue; } }

		public string DisplayClosedMessage
		{
			get
			{
				if (this.IsClosed) return string.Format("CLOSED: {0}", _closeDateUTC.Value.ToLocalTime());

				return string.Empty;
			}
		}

		public void Clear()
		{
			Id = Guid.Empty;
			Description = string.Empty;
			MonthlyBudget = decimal.Zero;
			JournalType = LedgerType.NotSet;
			this.DateClosedUTC = null;
		}

		public void Copy(LedgerAccountVM cpy)
		{
			Id = cpy.Id;
			Description = cpy.Description;
			JournalType = cpy.JournalType;
			DateClosedUTC = cpy.DateClosedUTC;
			MonthlyBudget = cpy.MonthlyBudget;
		}

		public void LoadAccount(IJournalAccount account)
		{
			if (account is null) throw new ArgumentNullException("Ledger Account");
			if (!_listValidTypes.Contains(account.JournalType)) throw new InvalidCastException(string.Format("[{0} - {1}] is not a valid Money Account", account.JournalType.ToString(), account.Description));

			Id = account.Id;
			Description = account.Description;
			JournalType = account.JournalType;
			DateClosedUTC = account.DateClosedUTC;

			if (account is ILedgerAccount ledger)
			{
				MonthlyBudget = ledger.MonthlyBudgetAmount;
			}
		}

		public void SaveAccount()
		{
			if (string.IsNullOrWhiteSpace(_desc)) return;
			if (JournalType == LedgerType.NotSet) return;

			IJournalAccount? acct = null;
			if (this.Id != Guid.Empty) acct = _config.GetJournalAccount(this.Id);

			if (acct is null)
			{
				acct = JournalAccountFactory.Build(this.Description, this.JournalType, this.MonthlyBudget);
				_config.AddJournalAccount(acct);
			}
			else
			{
				JournalAccountFactory.Update(ref acct, this.Description, this.MonthlyBudget);
			}
			_config.SaveJournalAccounts();
		}
	}
}