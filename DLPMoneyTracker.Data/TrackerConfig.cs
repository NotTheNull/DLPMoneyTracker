using DLPMoneyTracker.Data.ConfigModels;
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
        ReadOnlyCollection<MoneyAccount> AccountsList { get; }
        ReadOnlyCollection<TransactionCategory> CategoryList { get; }

        void LoadMoneyAccounts();

        void SaveMoneyAccounts();

        void LoadCategories();

        void SaveCategories();

        TransactionCategory GetCategory(Guid uid);

        MoneyAccount GetAccount(string id);

        void AddCategory(TransactionCategory cat);

        void AddMoneyAccount(MoneyAccount act);

        void RemoveCategory(TransactionCategory cat);

        void RemoveMoneyAccount(MoneyAccount act);

        void Copy(ITrackerConfig config);
    }

    public class TrackerConfig : ITrackerConfig
    {
        private int _year;
        private string FolderPath { get { return AppConfigSettings.CONFIG_FOLDER_PATH.Replace(AppConfigSettings.YEAR_FOLDER_PLACEHOLDER, _year.ToString()); } }

        private string AccountListConfig { get { return string.Concat(this.FolderPath, "MoneyAccounts.json"); } }

        private List<MoneyAccount> _listAccts = new List<MoneyAccount>();
        public ReadOnlyCollection<MoneyAccount> AccountsList { get { return _listAccts.OrderBy(o => o.ID).ToList().AsReadOnly(); } }

        private string CategoryListConfig { get { return string.Concat(this.FolderPath, "Categories.json"); } }

        private List<TransactionCategory> _listCategories = new List<TransactionCategory>();
        public ReadOnlyCollection<TransactionCategory> CategoryList { get { return _listCategories.OrderBy(o => o.Name).ToList().AsReadOnly(); } }

        public TrackerConfig() : this(DateTime.Today.Year)
        {
        }

        public TrackerConfig(int year)
        {
            _year = year;

            if (!Directory.Exists(this.FolderPath))
            {
                Directory.CreateDirectory(this.FolderPath);
            }

            this.LoadMoneyAccounts();
            this.LoadCategories();
        }

        ~TrackerConfig()
        {
            this.Dispose();
        }

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

        public void SaveMoneyAccounts()
        {
            string json = JsonSerializer.Serialize(_listAccts, typeof(List<MoneyAccount>));
            File.WriteAllText(AccountListConfig, json);
        }

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

        public void SaveCategories()
        {
            string json = JsonSerializer.Serialize(_listCategories.Where(x => x.ID != Guid.Empty).ToList(), typeof(List<TransactionCategory>));
            File.WriteAllText(CategoryListConfig, json);
        }

        public TransactionCategory GetCategory(Guid uid)
        {
            if (uid == TransactionCategory.InitialBalance.ID) return TransactionCategory.InitialBalance;
            else if (uid == TransactionCategory.DebtPayment.ID) return TransactionCategory.DebtPayment;
            else if (uid == TransactionCategory.TransferFrom.ID) return TransactionCategory.TransferFrom;
            else if (uid == TransactionCategory.TransferTo.ID) return TransactionCategory.TransferTo;
            else return _listCategories.FirstOrDefault(x => x.ID == uid);
        }

        public MoneyAccount GetAccount(string id)
        {
            return _listAccts.FirstOrDefault(x => x.ID == id);
        }

        public void AddCategory(TransactionCategory cat)
        {
            if (cat is null) return;
            if (cat.ID == Guid.Empty) return;
            if (_listCategories.Any(x => x.ID == cat.ID)) return;
            _listCategories.Add(cat);
        }

        public void AddMoneyAccount(MoneyAccount act)
        {
            if (act is null) return;
            if (string.IsNullOrWhiteSpace(act.ID)) return;
            if (_listAccts.Any(x => x.ID == act.ID)) return;
            _listAccts.Add(act);
        }

        public void RemoveCategory(TransactionCategory cat)
        {
            if (cat is null) return;
            if (cat.ID == Guid.Empty) return;
            if (!_listCategories.Any(x => x.ID == cat.ID)) return;
            _listCategories.Remove(cat);
        }

        public void RemoveMoneyAccount(MoneyAccount act)
        {
            if (act is null) return;
            if (string.IsNullOrWhiteSpace(act.ID)) return;
            if (!_listAccts.Any(x => x.ID == act.ID)) return;
            _listAccts.Remove(act);
        }

        public void Copy(ITrackerConfig config)
        {
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
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (!(_listAccts is null))
            {
                _listAccts.Clear();
                _listAccts = null;
            }

            if (!(_listCategories is null))
            {
                _listCategories.Clear();
                _listCategories = null;
            }
        }
    }
}