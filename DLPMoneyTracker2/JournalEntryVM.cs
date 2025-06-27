using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.Core;
using DLPMoneyTracker2.LedgerEntry;
using System;

namespace DLPMoneyTracker2
{
    public class JournalEntryVM(IMoneyTransaction entry) : BaseViewModel
    {
        private readonly IMoneyTransaction _je = entry;

        public RelayCommand CommandEdit =>
            new((o) =>
            {
                IJournalEntryVM viewModel = JournalEntryVMFactory.BuildViewModel(_je);
                RecordJournalEntry window = new RecordJournalEntry(viewModel);
                window.Show();
            });

        // Currently don't have any transacitons that I don't want to edit; will leave this here in case I change my mind
        public bool CanUserEdit => true;

        public DateTime TransactionDate => _je.TransactionDate;
        public string TransactionDescription => _je.Description;
        public decimal TransactionAmount => _je.TransactionAmount;
        public string CreditAccountName => _je.CreditAccountName;
        public string DebitAccountName => _je.DebitAccountName;
    }

    public class SingleAccountDetailVM : BaseViewModel
    {
        private readonly IMoneyTransaction _je;
        private readonly NotificationSystem notifications;
        private readonly IJournalAccount _parent;

        public SingleAccountDetailVM(IJournalAccount parent, IMoneyTransaction entry, NotificationSystem notifications) : base()
        {
            _je = entry;
            this.notifications = notifications;
            _parent = parent;
        }

        public IMoneyTransaction Source => _je;

        public RelayCommand CommandEdit =>
            new((o) =>
            {
                IJournalEntryVM viewModel = JournalEntryVMFactory.BuildViewModel(_je);
                RecordJournalEntry window = new(viewModel);
                window.Show();
            });

        public RelayCommand CommandSaveBankDate =>
            new((o) =>
            {
                this.SaveBankDateChanges();
            });

        public RelayCommand CommandEnableEditBankDate =>
            new((o) =>
            {
                this.CanEditBankDate = true;
                NotifyPropertyChanged(nameof(BankDate));
            });

        public RelayCommand CommandCancelRemoveBankDate =>
            new((o) =>
            {
                this.BankDate = null;
                this.CanEditBankDate = false;
                this.SaveBankDateChanges();
            });

        public bool CanUserEdit => _je.DebitAccountId != SpecialAccount.InitialBalance.Id && _je.CreditAccountId != SpecialAccount.InitialBalance.Id;

        public Guid JournalAccountId => _parent.Id;

        public bool IsCredit => _je.CreditAccountId == JournalAccountId;

        public string AccountName => IsCredit ? _je.DebitAccountName : _je.CreditAccountName;

        public string TransactionDescription => _je.Description;

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

        public DateTime TransactionDate => _je.TransactionDate;

        public DateTime? BankDate
        {
            get
            {
                return this.IsCredit ? _je.CreditBankDate : _je.DebitBankDate;
            }
            set
            {
                if (_je is MoneyTransaction record)
                {
                    if (this.IsCredit)
                    {
                        record.CreditBankDate = value;
                    }
                    else
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