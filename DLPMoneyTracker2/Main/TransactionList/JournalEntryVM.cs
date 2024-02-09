using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.TransactionModels;
using DLPMoneyTracker2.Core;
using DLPMoneyTracker2.LedgerEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.Main.TransactionList
{
	public class JournalEntryVM : BaseViewModel
	{
		private readonly IJournalEntry _je;

		public JournalEntryVM(IJournalEntry entry)
        {
			_je = entry;
		}



		private RelayCommand _cmdEdit;
		public RelayCommand CommandEdit
		{
			get
			{
				return _cmdEdit ?? (_cmdEdit = new RelayCommand((o) =>
				{
					IJournalEntryVM viewModel = JournalEntryVMFactory.BuildViewModel(_je);
					RecordJournalEntry window = new RecordJournalEntry(viewModel);
					window.Show();
				}));
			}
		}

		public bool CanUserEdit { get { return _je.DebitAccountId != SpecialAccount.InitialBalance.Id && _je.CreditAccountId != SpecialAccount.InitialBalance.Id; } }


		public DateTime TransactionDate { get { return _je.TransactionDate; } }
		public string DisplayTransactionDate { get { return string.Format("{0:yyyy-MM-dd}", _je.TransactionDate); } }
		public string TransactionDescription { get { return _je.Description; } }

		public decimal TransactionAmount { get { return _je.TransactionAmount; } }
		
		public string CreditAccountName { get { return _je.CreditAccountName; } }
		public string DebitAccountName { get { return _je.DebitAccountName; } }



	}


	public class SingleAccountDetailVM : BaseViewModel
	{
		private readonly IJournalEntry _je;
		private readonly IJournalAccount _parent;

		public SingleAccountDetailVM(IJournalAccount parent, IJournalEntry entry) : base()
		{
			_je = entry;
			_parent = parent;
		}



		private RelayCommand _cmdEdit;
		public RelayCommand CommandEdit
		{
			get
			{
				return _cmdEdit ?? (_cmdEdit = new RelayCommand((o) =>
				{
					IJournalEntryVM viewModel = JournalEntryVMFactory.BuildViewModel(_je);
					RecordJournalEntry window = new RecordJournalEntry(viewModel);
					window.Show();
				}));
			}
		}

		public bool CanUserEdit { get { return _je.DebitAccountId != SpecialAccount.InitialBalance.Id && _je.CreditAccountId != SpecialAccount.InitialBalance.Id; } }

		public Guid JournalAccountId { get { return _parent.Id; } }

		public bool IsCredit { get { return _je.CreditAccountId == JournalAccountId; } }

		public string AccountName { get { return IsCredit ? _je.DebitAccountName : _je.CreditAccountName; } }

		public string TransactionDescription { get { return _je.Description; } }

		public decimal TransactionAmount
		{
			get
			{
				if (_parent.JournalType == JournalAccountType.LiabilityCard || _parent.JournalType == JournalAccountType.LiabilityLoan)
				{
					return IsCredit ? _je.TransactionAmount : _je.TransactionAmount * -1;
				}
				else
				{
					return IsCredit ? _je.TransactionAmount * -1 : _je.TransactionAmount;
				}
			}
		}

		public DateTime TransactionDate { get { return _je.TransactionDate; } }
		public string DisplayTransactionDate { get { return string.Format("{0:yyyy-MM-dd}", _je.TransactionDate); } }
		public string DisplayBankDate 
		{ 
			get
			{
				DateTime? bankDate = this.IsCredit ? _je.CreditBankDate : _je.DebitBankDate;
				if (!bankDate.HasValue) return string.Empty;

				return string.Format("{0:yyyy-MM-dd}", bankDate);
			} 
		}
	}
}
