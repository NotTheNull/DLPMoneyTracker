using DLPMoneyTracker.Data.TransactionModels.BillPlan;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DLPMoneyTracker.Data
{
    public interface IMoneyPlanner : IJSONFileMaker
    {
        ReadOnlyCollection<IMoneyPlan> MoneyPlanList { get; }

        void AddMoneyPlan(IMoneyPlan record);

        void RemoveMoneyPlan(IMoneyPlan record);

        void ClearRecordList();

        IEnumerable<IMoneyPlan> GetUpcomingMoneyPlansForAccount(string accountID);

        void Copy(IMoneyPlanner planner);
    }

    public class MoneyPlanner : IMoneyPlanner
    {
        
        private string FolderPath { get { return AppConfigSettings.DATA_FOLDER_PATH.Replace(AppConfigSettings.YEAR_FOLDER_PLACEHOLDER, _year.ToString()); } }
        public string FilePath { get { return string.Concat(this.FolderPath, "MoneyPlan.json"); } }

        private ITrackerConfig _config;
        private int _year;

        private List<IMoneyPlan> _listMoneyPlans = new List<IMoneyPlan>();
        public ReadOnlyCollection<IMoneyPlan> MoneyPlanList { get { return _listMoneyPlans.AsReadOnly(); } }

        public MoneyPlanner(ITrackerConfig config) : this(config, DateTime.Today.Year) { }
        public MoneyPlanner(ITrackerConfig config, int year)
        {
            _year = year;
            _config = config;
            if (!Directory.Exists(this.FolderPath))
            {
                Directory.CreateDirectory(this.FolderPath);
            }
            this.LoadFromFile();
        }

        public void AddMoneyPlan(IMoneyPlan record)
        {
            if (_listMoneyPlans.Any(x => x.UID == record.UID)) return;
            _listMoneyPlans.Add(record);
        }

        public void RemoveMoneyPlan(IMoneyPlan record)
        {
            if (!_listMoneyPlans.Any(x => x.UID == record.UID)) return;
            _listMoneyPlans.Remove(record);
        }

        public void ClearRecordList()
        {
            _listMoneyPlans.Clear();
        }

        public void LoadFromFile()
        {
            if (_listMoneyPlans is null) _listMoneyPlans = new List<IMoneyPlan>();
            _listMoneyPlans.Clear();
            if (!File.Exists(this.FilePath)) return;

            string json = File.ReadAllText(FilePath);
            if (string.IsNullOrWhiteSpace(json)) return;

            var dataList = (List<MoneyPlanRecordJSON>)JsonSerializer.Deserialize(json, typeof(List<MoneyPlanRecordJSON>));
            if (dataList is null || !dataList.Any()) return;

            foreach (var record in dataList)
            {
                _listMoneyPlans.Add(MoneyPlanFactory.Build(_config, record));
            }
        }

        public void SaveToFile()
        {
            string json = JsonSerializer.Serialize(_listMoneyPlans);
            File.WriteAllText(FilePath, json);
        }

        public IEnumerable<IMoneyPlan> GetUpcomingMoneyPlansForAccount(string accountID)
        {
            if (!this.MoneyPlanList.Any(x => x.AccountID == accountID)) return null;

            List<IMoneyPlan> dataList = new List<IMoneyPlan>();
            foreach (var record in this.MoneyPlanList.Where(x => x.AccountID == accountID))
            {
                // Adding three days for Next Occurrence check to account for weekends & holidays that might delay the bill posting
                if (record.NotificationDate <= DateTime.Today && record.NextOccurrence.AddDays(3) >= DateTime.Today)
                {
                    dataList.Add(record);
                }
            }

            return dataList;
        }

        public void Copy(IMoneyPlanner planner)
        {
            this.ClearRecordList();
            foreach(var plan in this.MoneyPlanList)
            {
                this.AddMoneyPlan(plan);
            }
        }
    }
}