

using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.Core;
using DLPMoneyTracker2.LedgerEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2
{
	public class JournalEntryVM : BaseViewModel
	{
		private readonly IMoneyTransaction _je;

		public JournalEntryVM(IMoneyTransaction entry)
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

		// Currently don't have any transacitons that I don't want to edit; will leave this here in case I change my mind
		public bool CanUserEdit { get { return true; } }


		public DateTime TransactionDate { get { return _je.TransactionDate; } }
		public string TransactionDescription { get { return _je.Description; } }

		public decimal TransactionAmount { get { return _je.TransactionAmount; } }

		public string CreditAccountName { get { return _je.CreditAccountName; } }
		public string DebitAccountName { get { return _je.DebitAccountName; } }



	}


	public class SingleAccountDetailVM : BaseViewModel
	{
		private IMoneyTransaction _je;
        private readonly NotificationSystem notifications;
        private readonly IJournalAccount _parent;

		public SingleAccountDetailVM(IJournalAccount parent, IMoneyTransaction entry, NotificationSystem notifications) : base()
		{
			_je = entry;
            this.notifications = notifications;
            _parent = parent;
		}

		public IMoneyTransaction Source { get { return _je; } }

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

		private RelayCommand _cmdSaveBankDate;
		public RelayCommand CommandSaveBankDate
		{
			get
			{
				return _cmdSaveBankDate ?? (_cmdSaveBankDate = new RelayCommand((o) =>
				{
					this.SaveBankDateChanges();
				}));
			}
		}
		private RelayCommand _cmdEnableEditBankDate;
		public RelayCommand CommandEnableEditBankDate
		{
			get
			{
				return _cmdEnableEditBankDate ?? (_cmdEnableEditBankDate = new RelayCommand((o) =>
				{
					this.CanEditBankDate = true;
					NotifyPropertyChanged(nameof(BankDate));
				}));
			}
		}

		private RelayCommand _cmdCancelRemoveBankDate;
		public RelayCommand CommandCancelRemoveBankDate
		{
			get
			{
				return _cmdCancelRemoveBankDate ?? (_cmdCancelRemoveBankDate = new RelayCommand((o) =>
				{
					this.BankDate = null;
					this.CanEditBankDate = false;
					this.SaveBankDateChanges();
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
				if (_parent.JournalType == LedgerType.LiabilityCard || _parent.JournalType == LedgerType.LiabilityLoan)
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
		public DateTime? BankDate
		{
			get
			{
				return this.IsCredit ? _je.CreditBankDate : _je.DebitBankDate;
			}
			set
			{
				if(_je is MoneyTransaction record)
				{
					if(this.IsCredit)
					{
						record.CreditBankDate = value;
					} else
					{
						record.DebitBankDate = value;
					}
				}
				NotifyPropertyChanged(nameof(BankDate));
			}
		}

		private bool _canEditBankDate;

		public bool CanEditBankDate
		{
			get { return _canEditBankDate; }
			set 
			{
				_canEditBankDate = value;
				NotifyPropertyChanged(nameof(CanEditBankDate));
			}
		}


		private void SaveBankDateChanges()
		{
			var viewModel = JournalEntryVMFactory.BuildViewModel(_je);
			viewModel.SaveTransaction();
			this.CanEditBankDate = false;
			notifications.TriggerBankDateChanged(this.JournalAccountId);
		}


	}
}
