using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.TransactionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLPMoneyTracker.DataEntry.AddTransaction
{
    public class AddDebtPaymentVM : BaseViewModel
    {
        private ILedger _ledger;
        private ITrackerConfig _config;


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



        private MoneyAccount _actMoney;

        public MoneyAccount BankAccount
        {
            get { return _actMoney; }
            set 
            {
                _actMoney = value;
                NotifyPropertyChanged(nameof(this.BankAccount));
            }
        }


        private MoneyAccount _actDebt;

        public MoneyAccount DebtAccount
        {
            get { return _actDebt; }
            set 
            {
                _actDebt = value;
                NotifyPropertyChanged(nameof(this.DebtAccount));
            }
        }


        private decimal _amt;

        public decimal Payment
        {
            get { return _amt; }
            set 
            {
                _amt = value;
                NotifyPropertyChanged(nameof(this.Payment));
            }
        }






        private List<SpecialDropListItem<MoneyAccount>> _listBanks = new List<SpecialDropListItem<MoneyAccount>>();
        public List<SpecialDropListItem<MoneyAccount>> BankAccountList { get { return _listBanks; } }

        private List<SpecialDropListItem<MoneyAccount>> _listDebts = new List<SpecialDropListItem<MoneyAccount>>();
        public List<SpecialDropListItem<MoneyAccount>> DebtAccountList { get { return _listDebts; } }





        public AddDebtPaymentVM(ILedger ledger, ITrackerConfig config) : base()
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
            if(_config.AccountsList.Any(x => bankTypes.Contains(x.AccountType)))
            {
                foreach(var act in _config.AccountsList.Where(x => bankTypes.Contains(x.AccountType)).OrderBy(o => o.Description))
                {
                    _listBanks.Add(new SpecialDropListItem<MoneyAccount>(act.Description, act));
                }
            }

            List<MoneyAccountType> debtTypes = new List<MoneyAccountType>() { MoneyAccountType.CreditCard, MoneyAccountType.Loan };
            _listDebts.Clear();
            if (_config.AccountsList.Any(x => debtTypes.Contains(x.AccountType)))
            {
                foreach(var act in _config.AccountsList.Where(x => debtTypes.Contains(x.AccountType)).OrderBy(o => o.Description))
                {
                    _listDebts.Add(new SpecialDropListItem<MoneyAccount>(act.Description, act));
                }
            }
        }

        public void Clear()
        {
            this.TransactionDate = DateTime.Now;
            this.BankAccount = null;
            this.DebtAccount = null;
            this.Payment = decimal.Zero;
        }


        public void SavePayment()
        {
            if (this.BankAccount is null) throw new InvalidOperationException("Bank Account cannot be NULL");
            if (this.DebtAccount is null) throw new InvalidOperationException("Debt Account cannot be NULL");
            if (this.Payment <= decimal.Zero) throw new InvalidOperationException("Payment MUST be postiive non-zero number");

            _ledger.AddTransaction(new MoneyRecord()
            {
                Account = this.BankAccount,
                Category = TransactionCategory.DebtPayment,
                TransDate = this.TransactionDate,
                TransAmount = this.Payment,
                Description = "** Debt Payment **"
            });

            _ledger.AddTransaction(new MoneyRecord()
            {
                Account = this.DebtAccount,
                Category = TransactionCategory.DebtPayment,
                TransDate = this.TransactionDate,
                TransAmount = this.Payment,
                Description = "** Debt Payment **"
            });

            _ledger.SaveToFile();
            this.Clear();
        }
    }
}
