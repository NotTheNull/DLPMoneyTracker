using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Linq;
using DLPMoneyTracker;

namespace DLPMoneyTracker.Data
{
    public interface ITrackerConfig : IDisposable
    {
        List<MoneyAccount> AccountsList { get; }
        List<TransactionCategory> CategoryList { get; }

        void LoadMoneyAccounts();
        void SaveMoneyAccounts();

        void LoadCategories();
        void SaveCategories();

        TransactionCategory GetCategory(Guid uid);
        MoneyAccount GetAccount(string id);
    }

    public class TrackerConfig : ITrackerConfig
    {
        public const string CONFIG_PATH = @"D:\Program Files\DLP Money Tracker\Config\";
        
        
        private string AccountListConfig { get { return string.Concat(CONFIG_PATH, "MoneyAccounts.json"); } }

        private List<MoneyAccount> _listAccts = new List<MoneyAccount>();
        public List<MoneyAccount> AccountsList { get { return _listAccts; } }


        private string CategoryListConfig { get { return string.Concat(CONFIG_PATH, "Categories.json"); } }

        private List<TransactionCategory> _listCategories = new List<TransactionCategory>();
        public List<TransactionCategory> CategoryList { get { return _listCategories; } }


        
        public TrackerConfig()
        {
            if (!Directory.Exists(CONFIG_PATH))
            {
                Directory.CreateDirectory(CONFIG_PATH);
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
            string json = JsonSerializer.Serialize(this.AccountsList, typeof(List<MoneyAccount>));
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
            string json = JsonSerializer.Serialize(this.CategoryList.Where(x => x.ID != Guid.Empty).ToList(), typeof(List<TransactionCategory>));
            File.WriteAllText(CategoryListConfig, json);
        }



        public TransactionCategory GetCategory(Guid uid)
        {
            if (uid == Guid.Empty) return TransactionCategory.InitialBalance;

            return _listCategories.FirstOrDefault(x => x.ID == uid);
        }

        public MoneyAccount GetAccount(string id)
        {
            return _listAccts.FirstOrDefault(x => x.ID == id);
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
