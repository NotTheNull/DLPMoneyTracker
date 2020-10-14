using DLPMoneyTracker.Data.TransactionModels.Budget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DLPMoneyTracker.Data
{
    public interface IBudgetTracker : IJSONFileMaker
    {
        ReadOnlyCollection<IBudget> BudgetList { get; }

        void AddBudget(IBudget record);

        void RemoveBudget(IBudget record);

        void ClearBudget();

        decimal GetBudgetAmount(Guid categoryId);
    }

    public class BudgetTracker : IBudgetTracker
    {
        private ITrackerConfig _config;

        public string FilePath { get { return string.Concat(AppConfigSettings.DATA_FOLDER_PATH, "Budget.json"); } }

        private List<IBudget> _listBudgets = new List<IBudget>();
        public ReadOnlyCollection<IBudget> BudgetList { get { return _listBudgets.AsReadOnly(); } }

        public BudgetTracker(ITrackerConfig config)
        {
            _config = config;

            if (!Directory.Exists(AppConfigSettings.DATA_FOLDER_PATH))
            {
                Directory.CreateDirectory(AppConfigSettings.DATA_FOLDER_PATH);
            }

            this.LoadFromFile();
        }

        public void AddBudget(IBudget record)
        {
            IBudget existing = _listBudgets.FirstOrDefault(x => x.CategoryId == record.CategoryId);
            if (existing is null)
            {
                _listBudgets.Add(record);
            }
            else
            {
                existing.BudgetAmount = record.BudgetAmount;
            }
        }

        public void RemoveBudget(IBudget record)
        {
            if (!_listBudgets.Contains(record)) return;
            _listBudgets.Remove(record);
        }

        public void ClearBudget()
        {
            _listBudgets.Clear();
        }

        public decimal GetBudgetAmount(Guid categoryId)
        {
            return _listBudgets.FirstOrDefault(x => x.CategoryId == categoryId)?.BudgetAmount ?? decimal.Zero;
        }

        public void LoadFromFile()
        {
            _listBudgets.Clear();
            if (!File.Exists(FilePath)) return;

            string json = File.ReadAllText(FilePath);
            if (string.IsNullOrWhiteSpace(json)) return;

            var dataList = (List<BudgetJSON>)JsonSerializer.Deserialize(json, typeof(List<BudgetJSON>));
            if (dataList is null || !dataList.Any()) return;

            foreach (var record in dataList)
            {
                MonthlyBudget budget = new MonthlyBudget()
                {
                    BudgetAmount = record.BudgetAmount,
                    Category = _config.GetCategory(record.CategoryId)
                };
                if (budget.Category.ExcludeFromBudget) continue;

                _listBudgets.Add(budget);
            }
        }

        public void SaveToFile()
        {
            string json = JsonSerializer.Serialize(_listBudgets);
            File.WriteAllText(FilePath, json);
        }
    }
}