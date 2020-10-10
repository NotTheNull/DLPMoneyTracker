using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Linq;
using DLPMoneyTracker;
using System.Collections.ObjectModel;
using System.Transactions;

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

        void ClearCategoryList();
        void ClearMoneyAccountList();
    }

    public class TrackerConfig : ITrackerConfig
    {
        
        
        
        private string AccountListConfig { get { return string.Concat(AppConfigSettings.CONFIG_FOLDER_PATH, "MoneyAccounts.json"); } }

        private List<MoneyAccount> _listAccts = new List<MoneyAccount>();
        public ReadOnlyCollection<MoneyAccount> AccountsList { get { return _listAccts.OrderBy(o => o.ID).ToList().AsReadOnly(); } }


        private string CategoryListConfig { get { return string.Concat(AppConfigSettings.CONFIG_FOLDER_PATH, "Categories.json"); } }

        private List<TransactionCategory> _listCategories = new List<TransactionCategory>();
        public ReadOnlyCollection<TransactionCategory> CategoryList { get { return _listCategories.OrderBy(o => o.Name).ToList().AsReadOnly(); } }


        
        public TrackerConfig()
        {
            if (!Directory.Exists(AppConfigSettings.CONFIG_FOLDER_PATH))
            {
                Directory.CreateDirectory(AppConfigSettings.CONFIG_FOLDER_PATH);
            }

            this.LoadMoneyAccounts();
            this.LoadCategories();
        }
        ~TrackerConfig() { this.Dispose(); }


        public void LoadMoneyAccounts()
        {
            if (!(_listAccts is null) && _listAccts.Any()) _listAccts.Clear();

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

            if(File.Exists(CategoryListConfig))
            {
                string json = File.ReadAllText(CategoryListConfig);
                _listCategories = (List<TransactionCategory>)JsonSerializer.Deserialize(json, typeof(List<TransactionCategory>));
            }
            else
            {
                _listCategories = new List<TransactionCategory>();
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

        public void ClearCategoryList()
        {
            _listCategories.Clear();
        }

        public void ClearMoneyAccountList()
        {
            _listAccts.Clear();
        }






        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (!(_listAccts is null))
            {
                _listAccts.Clear();
                _listAccts = null;
            }

            if(!(_listCategories is null))
            {
                _listCategories.Clear();
                _listCategories = null;
            }
        }
    }
}
