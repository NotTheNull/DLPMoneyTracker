using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.TransactionModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DLPMoneyTracker.Data
{
    public interface ITrackerConfig : IDisposable
    {

        [Obsolete]
        ReadOnlyCollection<MoneyAccount> AccountsList { get; }
        [Obsolete]
        ReadOnlyCollection<TransactionCategory> CategoryList { get; }

        ReadOnlyCollection<ILedgerAccount> LedgerAccountsList { get; }
        ReadOnlyCollection<ILedgerAccount> PaymentAccounts { get; }
        ReadOnlyCollection<ILedgerAccount> AccountsReceivable { get; }
        ReadOnlyCollection<ILedgerAccount> AccountsPayable { get; }



        void LoadFromFile(int year);
        void SaveLedgerAccounts();
        void LoadLedgerAccounts();

        [Obsolete]
        void LoadMoneyAccounts();
        [Obsolete]
        void SaveMoneyAccounts();
        [Obsolete]
        void LoadCategories();
        [Obsolete]
        void SaveCategories();

        [Obsolete]
        TransactionCategory GetCategory(Guid uid);

        [Obsolete]
        MoneyAccount GetAccount(string id);

        [Obsolete]
        void AddCategory(TransactionCategory cat);

        [Obsolete]
        void AddMoneyAccount(MoneyAccount act);

        [Obsolete]
        void RemoveCategory(TransactionCategory cat);

        [Obsolete]
        void RemoveMoneyAccount(MoneyAccount act);

        void Copy(ITrackerConfig config);

        void Convert();
    }

    public class TrackerConfig : ITrackerConfig
    {
        private int _year;
        private string FolderPath { get { return AppConfigSettings.CONFIG_FOLDER_PATH.Replace(AppConfigSettings.YEAR_FOLDER_PLACEHOLDER, _year.ToString()); } }

        private string LedgerAccountsConfig { get { return string.Concat(this.FolderPath, "LedgerAccounts.json"); } }

        #region Obsolete Objects

        private string AccountListConfig { get { return string.Concat(this.FolderPath, "MoneyAccounts.json"); } }

        [Obsolete("Switch to ILedger Accounts")]
        private List<MoneyAccount> _listAccts = new List<MoneyAccount>();
        [Obsolete("Switch to ILedger Accounts")]
        public ReadOnlyCollection<MoneyAccount> AccountsList { get { return _listAccts.OrderBy(o => o.ID).ToList().AsReadOnly(); } }

        private string CategoryListConfig { get { return string.Concat(this.FolderPath, "Categories.json"); } }

        [Obsolete("Switch to ILedger Accounts")]
        private List<TransactionCategory> _listCategories = new List<TransactionCategory>();
        [Obsolete("Switch to ILedger Accounts")]
        public ReadOnlyCollection<TransactionCategory> CategoryList { get { return _listCategories.OrderBy(o => o.Name).ToList().AsReadOnly(); } }
        #endregion

        private List<ILedgerAccount> _listLedgerAccounts = new List<ILedgerAccount>();

        public ReadOnlyCollection<ILedgerAccount> LedgerAccountsList { get { return _listLedgerAccounts.AsReadOnly(); } }
        public ReadOnlyCollection<ILedgerAccount> PaymentAccounts
        {
            get
            {
                return _listLedgerAccounts
                    .Where(x => x.LedgerType == LedgerTypes.Bank || x.LedgerType == LedgerTypes.LiabilityCard)
                    .ToList()
                    .AsReadOnly();
            }
        }

        public ReadOnlyCollection<ILedgerAccount> AccountsReceivable
        {
            get
            {
                return _listLedgerAccounts.Where(x => x.LedgerType == LedgerTypes.Receivable).ToList().AsReadOnly();
            }
        }

        public ReadOnlyCollection<ILedgerAccount> AccountsPayable
        {
            get
            {
                return _listLedgerAccounts.Where(x => x.LedgerType == LedgerTypes.Payable || x.LedgerType == LedgerTypes.LiabilityLoan || x.LedgerType == LedgerTypes.LiabilityCard).ToList().AsReadOnly();
            }
        }



        public TrackerConfig() : this(DateTime.Today.Year)
        {
        }

        public TrackerConfig(int year)
        {
            this.LoadFromFile(year);
        }

        ~TrackerConfig()
        {
            this.Dispose();
        }

        public void LoadFromFile(int year)
        {
            _year = year;

            if (!Directory.Exists(this.FolderPath))
            {
                Directory.CreateDirectory(this.FolderPath);
            }

#pragma warning disable CS0612 // Type or member is obsolete
            // Will have to keep these until the conversion is done
            this.LoadMoneyAccounts();
            this.LoadCategories();
#pragma warning restore CS0612 // Type or member is obsolete
            this.LoadLedgerAccounts();
        }

        public void SaveLedgerAccounts()
        {
            string json = JsonSerializer.Serialize(_listLedgerAccounts, typeof(List<ILedgerAccount>));
            File.WriteAllText(LedgerAccountsConfig, json);
        }

        public void LoadLedgerAccounts()
        {
            _listLedgerAccounts ??= new List<ILedgerAccount>();
            _listLedgerAccounts.Clear();
            _listLedgerAccounts.Add(SpecialAccount.InitialBalance);
            _listLedgerAccounts.Add(SpecialAccount.Unlisted);

            if(File.Exists(LedgerAccountsConfig))
            {
                string json = File.ReadAllText(LedgerAccountsConfig);
                var dataList = (List<ILedgerAccount>)JsonSerializer.Deserialize(json, typeof(List<ILedgerAccount>));
                foreach(var data in dataList)
                {
                    switch(data.LedgerType)
                    {
                        case LedgerTypes.Bank:
                            BankAccount bank = new BankAccount(data);
                            _listLedgerAccounts.Add(bank);
                            break;
                        case LedgerTypes.LiabilityCard:
                            CreditCardAccount card = new CreditCardAccount(data);
                            _listLedgerAccounts.Add(card);
                            break;
                        case LedgerTypes.LiabilityLoan:
                            LoanAccount loan = new LoanAccount(data);
                            _listLedgerAccounts.Add(loan);
                            break; ;
                        case LedgerTypes.Payable:
                            PayableAccount payable = new PayableAccount(data);
                            _listLedgerAccounts.Add(payable);
                            break;
                        case LedgerTypes.Receivable:
                            ReceivableAccount receivable = new ReceivableAccount(data);
                            _listLedgerAccounts.Add(receivable);
                            break;
                        default:
                            throw new NotSupportedException(string.Format("Ledger Type [{0}] is not supported", data.LedgerType.ToString()));
                    }
                }
            }

        }

        #region Obsolete Methods
        [Obsolete]
        public void LoadMoneyAccounts()
        {
            if (_listAccts?.Any() == true) _listAccts.Clear();

            if (File.Exists(AccountListConfig))
            {
                string json = File.ReadAllText(AccountListConfig);
                _listAccts = (List<MoneyAccount>)JsonSerializer.Deserialize(json, typeof(List<MoneyAccount>));
            }
            else
            {
                _listAccts = new List<MoneyAccount>();
            }
        }
        [Obsolete]
        public void SaveMoneyAccounts()
        {
            string json = JsonSerializer.Serialize(_listAccts, typeof(List<MoneyAccount>));
            File.WriteAllText(AccountListConfig, json);
        }
        [Obsolete]
        public void LoadCategories()
        {
            if (!(_listCategories is null) && _listCategories.Any()) _listCategories.Clear();

            if (!File.Exists(CategoryListConfig)) return;

            string json = File.ReadAllText(CategoryListConfig);
            _listCategories = (List<TransactionCategory>)JsonSerializer.Deserialize(json, typeof(List<TransactionCategory>));
            if (_listCategories is null || !_listCategories.Any())
            {
                _listCategories = new List<TransactionCategory>();
                // Could be older version, try Transfer Version
                var dataList = (List<TransactionCategoryJSONTransferVersion>)JsonSerializer.Deserialize(json, typeof(List<TransactionCategoryJSONTransferVersion>));
                if (dataList is null || !dataList.Any()) return;

                foreach (var cat in dataList)
                {
                    _listCategories.Add(new TransactionCategory()
                    {
                        ID = cat.ID,
                        CategoryType = cat.CategoryType,
                        Name = cat.Name,
                        ExcludeFromBudget = false
                    });
                }

                this.SaveCategories();
            }
        }
        [Obsolete]
        public void SaveCategories()
        {
            string json = JsonSerializer.Serialize(_listCategories.Where(x => x.ID != Guid.Empty).ToList(), typeof(List<TransactionCategory>));
            File.WriteAllText(CategoryListConfig, json);
        }
        [Obsolete]
        public TransactionCategory GetCategory(Guid uid)
        {
            if (uid == TransactionCategory.InitialBalance.ID) return TransactionCategory.InitialBalance;
            else if (uid == TransactionCategory.DebtPayment.ID) return TransactionCategory.DebtPayment;
            else if (uid == TransactionCategory.TransferFrom.ID) return TransactionCategory.TransferFrom;
            else if (uid == TransactionCategory.TransferTo.ID) return TransactionCategory.TransferTo;
            else return _listCategories.FirstOrDefault(x => x.ID == uid);
        }
        [Obsolete]
        public MoneyAccount GetAccount(string id)
        {
            return _listAccts.FirstOrDefault(x => x.ID == id);
        }
        [Obsolete]
        public void AddCategory(TransactionCategory cat)
        {
            if (cat is null) return;
            if (cat.ID == Guid.Empty) return;
            if (_listCategories.Any(x => x.ID == cat.ID)) return;
            _listCategories.Add(cat);
        }
        [Obsolete]
        public void AddMoneyAccount(MoneyAccount act)
        {
            if (act is null) return;
            if (string.IsNullOrWhiteSpace(act.ID)) return;
            if (_listAccts.Any(x => x.ID == act.ID)) return;
            _listAccts.Add(act);
        }
        [Obsolete]
        public void RemoveCategory(TransactionCategory cat)
        {
            if (cat is null) return;
            if (cat.ID == Guid.Empty) return;
            if (!_listCategories.Any(x => x.ID == cat.ID)) return;
            _listCategories.Remove(cat);
        }
        [Obsolete]
        public void RemoveMoneyAccount(MoneyAccount act)
        {
            if (act is null) return;
            if (string.IsNullOrWhiteSpace(act.ID)) return;
            if (!_listAccts.Any(x => x.ID == act.ID)) return;
            _listAccts.Remove(act);
        }
        #endregion

        public void AddLedgerAccount(ILedgerAccount act) 
        {
            if(act is null) return;
            if(act.Id == Guid.Empty) return;
            if(_listLedgerAccounts.Any(x => x.Id == act.Id)) return;
            _listLedgerAccounts.Add(act);
        }

#pragma warning disable CS0612 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
        /// <summary>
        /// Converts Money Accounts and Transaction Categories into Ledger Accounts
        /// </summary>
        public void Convert()
        {
            if (_listAccts?.Any() == true)
            {
                var loopAccounts = _listAccts.ToList();
                foreach (var act in loopAccounts)
                {
                    // If the account already exists, still need to remove the legacy
                    if (!LedgerAccountsList.Any(x => x.AccountType != MoneyAccountType.NotSet && x.MoneyAccountId == act.ID))
                    {
                        switch (act.AccountType)
                        {
                            case MoneyAccountType.Checking:
                            case MoneyAccountType.Savings:
                                _listLedgerAccounts.Add(new BankAccount(act));
                                break;
                            case MoneyAccountType.CreditCard:
                                _listLedgerAccounts.Add(new CreditCardAccount(act));
                                break;
                            case MoneyAccountType.Loan:
                                _listLedgerAccounts.Add(new LoanAccount(act));
                                break;
                        }

                    }

                    _listAccts.Remove(act);
                }

                SaveMoneyAccounts();
            }


            if (_listCategories?.Any() == true)
            {
                var loopCategories = _listCategories.ToList();
                foreach(var cat in loopCategories)
                {
                    // If the category already exists, still need to remove the legacy
                    if(!LedgerAccountsList.Any(x => x.AccountType == MoneyAccountType.NotSet && x.CategoryId == cat.ID))
                    {
                        switch(cat.CategoryType)
                        {
                            case CategoryType.Expense:
                                PayableAccount ap = new PayableAccount(cat);
                                _listLedgerAccounts.Add(ap);
                                break;
                            case CategoryType.Income:
                                ReceivableAccount ar = new ReceivableAccount(cat);
                                _listLedgerAccounts.Add(ar);
                                break;
                            case CategoryType.UntrackedAdjustment:
                                PayableAccount pay = new PayableAccount(cat);
                                pay.Description = string.Format("AP {0}", pay.Description);
                                _listLedgerAccounts.Add(pay);

                                ReceivableAccount rec = new ReceivableAccount(cat);
                                rec.Description = string.Format("AR {0}", pay.Description);
                                _listLedgerAccounts.Add(rec);
                                break;
                        }
                        
                    }
                    _listCategories.Remove(cat);
                }
                SaveCategories();
            }
            SaveLedgerAccounts();

        }
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore CS0612 // Type or member is obsolete




        public void Copy(ITrackerConfig config)
        {
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CS0612 // Type or member is obsolete
            // Until the conversion is done, this will need to stayu
            _listAccts.Clear();
            foreach (var act in config.AccountsList)
            {
                this.AddMoneyAccount(act);
            }

            _listCategories.Clear();
            foreach (var cat in config.CategoryList)
            {
                this.AddCategory(cat);
            }
#pragma warning restore CS0612 // Type or member is obsolete
#pragma warning restore CS0618 // Type or member is obsolete

            _listLedgerAccounts.Clear();
            foreach(var gl in config.LedgerAccountsList)
            {
                this.AddLedgerAccount(gl);
            }
            
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

#pragma warning disable CS0618 // Type or member is obsolete
            // Will need to keep until conversion is done
            if (_listAccts is not null)
            {
                _listAccts.Clear();
                _listAccts = null;
            }

            if (_listCategories is not null)
            {
                _listCategories.Clear();
                _listCategories = null;
            }
#pragma warning restore CS0618 // Type or member is obsolete

            if(_listLedgerAccounts is not null)
            {
                _listLedgerAccounts.Clear();
                _listLedgerAccounts = null;
            }
        }
    }
}