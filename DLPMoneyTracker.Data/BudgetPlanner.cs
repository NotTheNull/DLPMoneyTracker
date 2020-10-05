﻿using DLPMoneyTracker.Data.TransactionModels;
using DLPMoneyTracker.Data.TransactionModels.BillPlan;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace DLPMoneyTracker.Data
{

    public interface IBudgetPlanner
    {
        ReadOnlyCollection<IBudgetRecord> BudgetRecordList { get; }

        void AddBudget(IBudgetRecord record);
        void RemoveBudget(IBudgetRecord record);
        void ClearBudget();
        void LoadFromFile();
        void SaveToFile();
    }

    public class BudgetPlanner : IBudgetPlanner
    {
        private const string BUDGET_FOLDER_PATH = @"D:\Program Files\DLP Money Tracker\Data\";

        private string BudgetFilePath { get { return string.Concat(BUDGET_FOLDER_PATH, "Budget.json"); } }

        private ITrackerConfig _config;


        private List<IBudgetRecord> _listBudgets = new List<IBudgetRecord>();
        public ReadOnlyCollection<IBudgetRecord> BudgetRecordList { get { return _listBudgets.AsReadOnly(); } }



        public BudgetPlanner(ITrackerConfig config)
        {
            _config = config;
            if(!Directory.Exists(BUDGET_FOLDER_PATH))
            {
                Directory.CreateDirectory(BUDGET_FOLDER_PATH);
            }
            this.LoadFromFile();
        }


        public void AddBudget(IBudgetRecord record)
        {
            if (_listBudgets.Any(x => x.UID == record.UID)) return;
            _listBudgets.Add(record);
        }

        public void RemoveBudget(IBudgetRecord record)
        {
            if (!_listBudgets.Any(x => x.UID == record.UID)) return;
            _listBudgets.Remove(record);
        }

        public void ClearBudget()
        {
            _listBudgets.Clear();
        }


        public void LoadFromFile()
        {
            if (_listBudgets is null) _listBudgets = new List<IBudgetRecord>();
            _listBudgets.Clear();
            if(!File.Exists(this.BudgetFilePath)) return;

            string json = File.ReadAllText(BudgetFilePath);
            if (string.IsNullOrWhiteSpace(json)) return;

            var dataList = (List<BudgetRecordJSON>)JsonSerializer.Deserialize(json, typeof(List<BudgetRecordJSON>));
            if(dataList.Any())
            {
                foreach(var record in dataList)
                {
                    _listBudgets.Add(new BudgetRecord()
                    {
                        UID = record.UID,
                        BillDescription = record.BillDescription,
                        Category = _config.GetCategory(record.CategoryID),
                        ExpectedAmount = record.ExpectedAmount,
                        RecurrenceJSON = record.RecurrenceJSON
                    });
                }
            }

        }

        public void SaveToFile()
        {
            string json = JsonSerializer.Serialize(_listBudgets);
            File.WriteAllText(BudgetFilePath, json);
        }
    }
}