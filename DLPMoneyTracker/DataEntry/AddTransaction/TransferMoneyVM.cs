using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.TransactionModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DLPMoneyTracker.DataEntry.AddTransaction
{
    public class TransferMoneyVM : BaseViewModel
    {
        private readonly ILedger _ledger;
        private readonly ITrackerConfig _config;

        private DateTime _dateTrans;

        public DateTime TransactionDate
        {
            get { return _dateTrans; }
            set
            {
                _dateTrans = value;
                NotifyPropertyChanged(nameof(this.TransactionDate));
            }
        }

        private MoneyAccount _act1;

        public MoneyAccount BankAccountFrom
        {
            get { return _act1; }
            set
            {
                _act1 = value;
                NotifyPropertyChanged(nameof(this.BankAccountFrom));
            }
        }

        private MoneyAccount _act2;

        public MoneyAccount BankAccountTo
        {
            get { return _act2; }
            set
            {
                _act2 = value;
                NotifyPropertyChanged(nameof(this.BankAccountTo));
            }
        }

        private decimal _amt;

        public decimal Amount
        {
            get { return _amt; }
            set
            {
                _amt = value;
                NotifyPropertyChanged(nameof(this.Amount));
            }
        }

        private List<SpecialDropListItem<MoneyAccount>> _listBanks = new List<SpecialDropListItem<MoneyAccount>>();
        public List<SpecialDropListItem<MoneyAccount>> BankAccountList { get { return _listBanks; } }

        public TransferMoneyVM(ITrackerConfig config, ILedger ledger) : base()
        {
            _ledger = ledger;
            _config = config;

            this.LoadAccounts();
            this.Clear();
        }

        private void LoadAccounts()
        {
            List<MoneyAccountType> bankTypes = new List<MoneyAccountType>() { MoneyAccountType.Checking, MoneyAccountType.Savings };
            _listBanks.Clear();
            if (_config.AccountsList.Any(x => bankTypes.Contains(x.AccountType)))
            {
                foreach (var act in _config.AccountsList.Where(x => bankTypes.Contains(x.AccountType)).OrderBy(o => o.Description))
                {
                    _listBanks.Add(new SpecialDropListItem<MoneyAccount>(act.Description, act));
                }
            }
        }

        public void Clear()
        {
            this.TransactionDate = DateTime.Now;
            this.BankAccountFrom = null;
            this.BankAccountTo = null;
            this.Amount = decimal.Zero;
        }

        public void SaveTransfer()
        {
            if (this.BankAccountFrom is null) throw new InvalidOperationException("Bank Account From cannot be NULL");
            if (this.BankAccountTo is null) throw new InvalidOperationException("Bank Account To cannot be NULL");
            if (this.Amount <= 0m) throw new InvalidOperationException("Amount must be a postivie non-zero value");

            _ledger.AddTransaction(new MoneyRecord()
            {
                Account = this.BankAccountFrom,
                Category = TransactionCategory.TransferFrom,
                TransDate = this.TransactionDate,
                Description = string.Format("** XFER TO {0} **", this.BankAccountTo.Description),
                TransAmount = this.Amount
            });

            _ledger.AddTransaction(new MoneyRecord()
            {
                Account = this.BankAccountTo,
                Category = TransactionCategory.TransferTo,
                TransDate = this.TransactionDate,
                Description = string.Format("** XFER FROM {0} **", this.BankAccountFrom.Description),
                TransAmount = this.Amount
            });

            _ledger.SaveToFile();
            this.Clear();
        }
    }
}