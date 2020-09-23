using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.TransactionModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace DLPMoneyTracker.Data
{
    public interface ILedger
    {
        ReadOnlyCollection<IMoneyRecord> TransactionList { get; }



        void AddTransaction(IMoneyRecord trans);
        void RemoveTransaction(IMoneyRecord trans);

        decimal GetAccountBalance(MoneyAccount act);
        decimal GetCategoryTotal(TransactionCategory cat);

        void SaveToFile();
        void LoadFromFile();
    }


    public class Ledger : ILedger
    {
        private const string LEDGER_FOLDER_PATH = @"D:\Program Files\DLP Money Tracker\Data\";
        private string LedgerFilePath { get { return string.Concat(LEDGER_FOLDER_PATH, "Ledger.json"); } }

        private ITrackerConfig _config;


        private List<IMoneyRecord> _listTransactions = new List<IMoneyRecord>();
        public ReadOnlyCollection<IMoneyRecord> TransactionList { get { return _listTransactions.AsReadOnly(); } }








        public Ledger(ITrackerConfig config)
        {
            _config = config;
            if (!Directory.Exists(LEDGER_FOLDER_PATH))
            {
                Directory.CreateDirectory(LEDGER_FOLDER_PATH);
            }

            this.LoadFromFile();
        }


        public void AddTransaction(IMoneyRecord trans)
        {
            _listTransactions.Add(trans);
            this.SaveToFile();
        }
        public void RemoveTransaction(IMoneyRecord trans)
        {
            if (_listTransactions.Contains(trans))
            {
                _listTransactions.Remove(trans);
                this.SaveToFile();
            }
        }


        public decimal GetAccountBalance(MoneyAccount act)
        {
            if (_listTransactions.Any(x => x.AccountID == act.ID))
            {
                return _listTransactions.Where(x => x.AccountID == act.ID).Sum(s => s.TransAmount);
            }

            return decimal.Zero;
        }

        public decimal GetCategoryTotal(TransactionCategory cat)
        {
            if (_listTransactions.Any(x => x.CategoryUID == cat.ID))
            {
                return _listTransactions.Where(x => x.CategoryUID == cat.ID).Sum(s => s.TransAmount);
            }

            return decimal.Zero;
        }

        public void SaveToFile()
        {
            string json = JsonSerializer.Serialize(_listTransactions);
            File.WriteAllText(LedgerFilePath, json);
        }

        public void LoadFromFile()
        {
            if (_listTransactions is null) _listTransactions = new List<IMoneyRecord>();
            _listTransactions.Clear();
            if (File.Exists(LedgerFilePath))
            {
                string json = File.ReadAllText(LedgerFilePath);
                var dataList = (List<IMoneyRecord>)JsonSerializer.Deserialize(json, typeof(List<IMoneyRecord>));

                foreach (var trans in dataList)
                {
                    _listTransactions.Add(new MoneyRecord()
                    {
                        Description = trans.Description,
                        TransDate = trans.TransDate,
                        TransAmount = trans.TransAmount,
                        Account = _config.AccountsList.FirstOrDefault(x => x.ID == trans.AccountID),
                        Category = _config.CategoryList.FirstOrDefault(x => x.ID == trans.CategoryUID)
                    });
                }
            }
        }
    }
}
