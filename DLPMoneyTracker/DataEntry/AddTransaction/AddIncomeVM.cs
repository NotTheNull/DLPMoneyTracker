﻿using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.TransactionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace DLPMoneyTracker.DataEntry.AddTransaction
{
    public class AddIncomeVM : BaseViewModel
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

        private MoneyAccount _act;

        public MoneyAccount BankAccount
        {
            get { return _act; }
            set 
            { 
                _act = value;
                NotifyPropertyChanged(nameof(this.BankAccount));
            }
        }


        private TransactionCategory _cat;

        public TransactionCategory Category
        {
            get { return _cat; }
            set 
            { 
                _cat = value;
                NotifyPropertyChanged(nameof(this.Category));
            }
        }




        private string _desc;

        public string Description
        {
            get { return _desc; }
            set 
            { 
                _desc = value;
                NotifyPropertyChanged(nameof(this.Description));
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





        private List<SpecialDropListItem<TransactionCategory>> _listCategories = new List<SpecialDropListItem<TransactionCategory>>();
        public List<SpecialDropListItem<TransactionCategory>> CategoryList { get { return _listCategories; } }


        private List<SpecialDropListItem<MoneyAccount>> _listBanks = new List<SpecialDropListItem<MoneyAccount>>();
        public List<SpecialDropListItem<MoneyAccount>> BankAccountList { get { return _listBanks; } }


        public AddIncomeVM(ITrackerConfig config, ILedger ledger) : base()
        {
            _ledger = ledger;
            _config = config;

            this.LoadAccounts();
            this.LoadCategories();
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

        }

        private void LoadCategories()
        {
            _listCategories.Clear();
            if(_config.CategoryList.Any(x => x.CategoryType == CategoryType.Income))
            {
                foreach(var cat in _config.CategoryList.Where(x => x.CategoryType == CategoryType.Income).OrderBy(o => o.Name))
                {
                    _listCategories.Add(new SpecialDropListItem<TransactionCategory>(cat.Name, cat));
                }
            }
        }

        public void Clear()
        {
            this.TransactionDate = DateTime.Now;
            this.BankAccount = null;
            this.Category = null;
            this.Amount = decimal.Zero;
            this.Description = string.Empty;
        }

        public void SaveTransaction()
        {
            if (this.BankAccount is null) throw new InvalidOperationException("Bank Account cannot be NULL");
            if (this.Category is null) throw new InvalidOperationException("Category cannot be NULL");
            if (this.Amount <= decimal.Zero) throw new InvalidOperationException("Amount must be provided");

            _ledger.AddTransaction(new MoneyRecord()
            {
                Account = this.BankAccount,
                Category = this.Category,
                TransDate = this.TransactionDate,
                Description = this.Description,
                TransAmount = this.Amount
            });
            _ledger.SaveToFile();
            this.Clear();
        }

    }
}
