﻿using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.TransactionModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DLPMoneyTracker.Data
{
    public delegate void LedgerModifiedHandler();

    public interface ILedger : IJSONFileMaker
    {
        event LedgerModifiedHandler LedgerModified;

        ReadOnlyCollection<IMoneyRecord> TransactionList { get; }

        void AddTransaction(IMoneyRecord trans);

        void RemoveTransaction(IMoneyRecord trans);

        decimal GetInitialBalance(MoneyAccount act);

        decimal GetAccountBalance(MoneyAccount act);

        decimal ApplyTransactionToBalance(MoneyAccount act, decimal startValue, TransactionCategory category, decimal transAmount);

        decimal GetCategoryTotal(TransactionCategory cat);

        decimal GetCategoryMonthlyTotal(TransactionCategory cat);
    }

    public class Ledger : ILedger
    {
        public event LedgerModifiedHandler LedgerModified;

        public string FilePath { get { return string.Concat(AppConfigSettings.DATA_FOLDER_PATH, "Ledger.json"); } }

        private ITrackerConfig _config;

        private List<IMoneyRecord> _listTransactions = new List<IMoneyRecord>();

        public ReadOnlyCollection<IMoneyRecord> TransactionList { get { return _listTransactions.AsReadOnly(); } }

        public Ledger(ITrackerConfig config)
        {
            _config = config;
            if (!Directory.Exists(AppConfigSettings.DATA_FOLDER_PATH))
            {
                Directory.CreateDirectory(AppConfigSettings.DATA_FOLDER_PATH);
            }

            this.LoadFromFile();
        }

        public void AddTransaction(IMoneyRecord trans)
        {
            if (trans.CategoryUID == Guid.Empty)
            {
                // This is the UID of the Initial Account Balance category.  There should only be ONE per Money Account
                var initialRecord = _listTransactions.FirstOrDefault(x => x.CategoryUID == Guid.Empty && x.AccountID == trans.AccountID);
                if (initialRecord is null)
                {
                    _listTransactions.Add(trans);
                }
                else if (initialRecord is MoneyRecord record)
                {
                    record.TransAmount = trans.TransAmount;
                }
            }
            else
            {
                _listTransactions.Add(trans);
            }
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

        public decimal GetInitialBalance(MoneyAccount act)
        {
            decimal initialBalance = decimal.Zero;
            if (_listTransactions.Any(x => x.AccountID == act.ID && x.CategoryUID == TransactionCategory.InitialBalance.ID))
            {
                var record = _listTransactions.FirstOrDefault(x => x.AccountID == act.ID && x.CategoryUID == TransactionCategory.InitialBalance.ID);
                initialBalance = record.TransAmount;
            }

            return initialBalance;
        }

        public decimal GetAccountBalance(MoneyAccount act)
        {
            decimal balance = decimal.Zero;
            if (_listTransactions.Any(x => x.AccountID == act.ID))
            {
                foreach (var trans in _listTransactions.Where(x => x.AccountID == act.ID))
                {
                    if (trans is MoneyRecord record)
                    {
                        balance = this.ApplyTransactionToBalance(act, balance, record.Category, record.TransAmount);
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("Type {0} is not supported in Account Balance Calc", trans.GetType()));
                    }
                }
            }

            return balance;
        }

        public decimal ApplyTransactionToBalance(MoneyAccount act, decimal startValue, TransactionCategory category, decimal transAmount)
        {
            decimal balance = startValue;

            if (category.ID == TransactionCategory.InitialBalance.ID)
            {
                balance += transAmount;
            }
            else
            {
                switch (category.CategoryType)
                {
                    case CategoryType.UntrackedAdjustment:
                        balance += transAmount;
                        break;

                    case CategoryType.Expense:
                        if (act.AccountType == MoneyAccountType.Checking || act.AccountType == MoneyAccountType.Savings)
                        {
                            balance -= transAmount;
                        }
                        else if (act.AccountType == MoneyAccountType.CreditCard)
                        {
                            balance += transAmount;
                        }
                        // Loans cannot have expenses added to them
                        break;

                    case CategoryType.Income:
                        if (act.AccountType == MoneyAccountType.Checking || act.AccountType == MoneyAccountType.Savings)
                        {
                            balance += transAmount;
                        }
                        // No other account types can have income reported
                        break;

                    case CategoryType.Payment:
                        // No matter the account type, it's a reduction
                        balance -= transAmount;
                        break;
                }
            }

            return balance;
        }

        public decimal GetCategoryTotal(TransactionCategory cat)
        {
            if (_listTransactions.Any(x => x.CategoryUID == cat.ID))
            {
                return _listTransactions.Where(x => x.CategoryUID == cat.ID).Sum(s => s.TransAmount);
            }

            return decimal.Zero;
        }

        public decimal GetCategoryMonthlyTotal(TransactionCategory cat)
        {
            bool isCurrentMonth(IMoneyRecord record)
            {
                return record.CategoryUID == cat.ID
                    && record.TransDate.Month == DateTime.Today.Month;
            };

            if (_listTransactions.Any(isCurrentMonth))
            {
                return _listTransactions.Where(isCurrentMonth).Sum(s => s.TransAmount);
            }

            return decimal.Zero;
        }

        public void SaveToFile()
        {
            string json = JsonSerializer.Serialize(_listTransactions);
            File.WriteAllText(FilePath, json);
            LedgerModified?.Invoke();
        }

        public void LoadFromFile()
        {
            if (_listTransactions is null) _listTransactions = new List<IMoneyRecord>();
            _listTransactions.Clear();
            if (!File.Exists(FilePath)) return;

            string json = File.ReadAllText(FilePath);
            if (string.IsNullOrWhiteSpace(json)) return;

            var dataList = (List<MoneyRecordJSON>)JsonSerializer.Deserialize(json, typeof(List<MoneyRecordJSON>));
            if (dataList is null || !dataList.Any()) return;

            foreach (var trans in dataList)
            {
                MoneyRecord record = new MoneyRecord()
                {
                    TransDate = trans.TransDate,
                    Account = _config.GetAccount(trans.AccountID),
                    Category = _config.GetCategory(trans.CategoryUID),
                    Description = trans.Description,
                    TransAmount = trans.TransAmount
                };

                _listTransactions.Add(record);
            }
        }
    }
}