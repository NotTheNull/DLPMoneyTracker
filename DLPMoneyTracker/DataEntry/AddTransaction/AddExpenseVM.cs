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
    public class AddExpenseVM : BaseViewModel
    {
        private ILedger _ledger;
        private ITrackerConfig _config;


        public Guid TransactionId { get; set; }


        private DateTime _transDate;

        public DateTime TransactionDate
        {
            get { return _transDate; }
            set
            {
                _transDate = value;
                NotifyPropertyChanged(nameof(this.TransactionDate));
            }
        }

        private MoneyAccount _act;

        public MoneyAccount SelectedAccount
        {
            get { return _act; }
            set
            {
                _act = value;
                NotifyPropertyChanged(nameof(this.SelectedAccount));
                NotifyPropertyChanged(nameof(this.SelectedAccountId));
            }
        }

        public string SelectedAccountId
        {
            get { return this.SelectedAccount?.ID ?? string.Empty; }
            set
            {
                this.SelectedAccount = _config.GetAccount(value);
            }
        }


        private TransactionCategory _cat;

        public TransactionCategory SelectedCategory
        {
            get { return _cat; }
            set
            {
                _cat = value;
                NotifyPropertyChanged(nameof(this.SelectedCategory));
                NotifyPropertyChanged(nameof(this.SelectedAccountId));
            }
        }

        public Guid SelectedCategoryId
        {
            get { return this.SelectedCategory?.ID ?? Guid.Empty; }
            set
            {
                this.SelectedCategory = _config.GetCategory(value);
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









        private List<SpecialDropListItem<MoneyAccount>> _listAccts = new List<SpecialDropListItem<MoneyAccount>>();
        public List<SpecialDropListItem<MoneyAccount>> MoneyAccountList { get { return _listAccts; } }


        private List<SpecialDropListItem<TransactionCategory>> _listCat = new List<SpecialDropListItem<TransactionCategory>>();
        public List<SpecialDropListItem<TransactionCategory>> CategoryList { get { return _listCat; } }





        public AddExpenseVM(ILedger ledger, ITrackerConfig config) : base()
        {
            _ledger = ledger;
            _config = config;
            this.TransactionId = Guid.Empty;

            this.FillLists();
            this.Clear();
        }

        public void Clear()
        {
            this.TransactionDate = DateTime.Now;
            this.SelectedAccount = null;
            this.SelectedCategory = null;
            this.Amount = decimal.Zero;
            this.Description = string.Empty;
        }

        private void FillLists()
        {
            // Fill account with Checking & credit card; Savings should be transferred from, not direct pay FOR NOW
            List<MoneyAccountType> validMoneyAccounts = new List<MoneyAccountType>() { MoneyAccountType.Checking, MoneyAccountType.CreditCard };

            _listAccts.Clear();
            foreach (var act in _config.AccountsList.Where(x => validMoneyAccounts.Contains(x.AccountType)))
            {
                _listAccts.Add(new SpecialDropListItem<MoneyAccount>(act.Description, act));
            }
            NotifyPropertyChanged(nameof(this.MoneyAccountList));

            // Fill category with Expenses and Adjustment
            List<CategoryType> validCategories = new List<CategoryType>() { CategoryType.Expense, CategoryType.UntrackedAdjustment };

            _listCat.Clear();
            foreach (var cat in _config.CategoryList.Where(x => validCategories.Contains(x.CategoryType)))
            {
                _listCat.Add(new SpecialDropListItem<TransactionCategory>(cat.Name, cat));
            }
            NotifyPropertyChanged(nameof(this.CategoryList));
        }


        public void SaveTransaction()
        {
            if (this.SelectedAccount is null) throw new InvalidOperationException("You MUST select an Account");
            if (this.SelectedCategory is null) throw new InvalidOperationException("You MUST select a Category");
            if (this.Amount <= decimal.Zero) throw new InvalidOperationException("Amount must be a positive value");

            IMoneyRecord newRecord = null;
            if (this.TransactionId != Guid.Empty)
            {
                newRecord = _ledger.TransactionList.FirstOrDefault(x => x.TransactionId == this.TransactionId);
            }

            if (newRecord is null)
            {
                newRecord = new MoneyRecord()
                {
                    TransDate = this.TransactionDate,
                    Account = this.SelectedAccount,
                    Category = this.SelectedCategory,
                    Description = this.Description.Trim(),
                    TransAmount = this.Amount
                };

                _ledger.AddTransaction(newRecord);
                _ledger.SaveToFile();
            }
            else
            {
                if (newRecord is MoneyRecord transaction)
                {
                    transaction.TransDate = this.TransactionDate;
                    transaction.TransAmount = this.Amount;
                    transaction.Category = this.SelectedCategory;
                    transaction.Account = this.SelectedAccount;
                    transaction.Description = this.Description;
                }
            }
            this.Clear();
        }



    }
}
