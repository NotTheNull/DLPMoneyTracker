using DLPMoneyTracker.Data.TransactionModels;
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

    public interface IMoneyPlanner
    {
        ReadOnlyCollection<IMoneyPlan> MoneyPlanList { get; }

        void AddMoneyPlan(IMoneyPlan record);
        void RemoveMoneyPlan(IMoneyPlan record);
        void ClearRecordList();
        void LoadFromFile();
        void SaveToFile();
        IEnumerable<IMoneyPlan> GetUpcomingMoneyPlansForAccount(string accountID);
    }

    public class MoneyPlanner : IMoneyPlanner
    {
        // TODO: Modify program to store the Budget Folder Path in a config file
        private const string MONEYPLAN_FOLDER_PATH = @"D:\Program Files\DLP Money Tracker\Data\";

        private string MoneyPlanFilePath { get { return string.Concat(MONEYPLAN_FOLDER_PATH, "MoneyPlan.json"); } }

        private ITrackerConfig _config;


        private List<IMoneyPlan> _listMoneyPlans = new List<IMoneyPlan>();
        public ReadOnlyCollection<IMoneyPlan> MoneyPlanList { get { return _listMoneyPlans.AsReadOnly(); } }



        public MoneyPlanner(ITrackerConfig config)
        {
            _config = config;
            if(!Directory.Exists(MONEYPLAN_FOLDER_PATH))
            {
                Directory.CreateDirectory(MONEYPLAN_FOLDER_PATH);
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
            if(!File.Exists(this.MoneyPlanFilePath)) return;

            string json = File.ReadAllText(MoneyPlanFilePath);
            if (string.IsNullOrWhiteSpace(json)) return;

            var dataList = (List<MoneyPlanRecordJSON>)JsonSerializer.Deserialize(json, typeof(List<MoneyPlanRecordJSON>));
            if(dataList.Any())
            {
                foreach(var record in dataList)
                {
                    _listMoneyPlans.Add(new MoneyPlanRecord()
                    {
                        UID = record.UID,
                        Description = record.Description,
                        Account = _config.GetAccount(record.AccountID),
                        Category = _config.GetCategory(record.CategoryID),
                        ExpectedAmount = record.ExpectedAmount,
                        RecurrenceJSON = record.RecurrenceJSON
                    });
                }
            }

        }

        public void SaveToFile()
        {
            string json = JsonSerializer.Serialize(_listMoneyPlans);
            File.WriteAllText(MoneyPlanFilePath, json);
        }

        public IEnumerable<IMoneyPlan> GetUpcomingMoneyPlansForAccount(string accountID)
        {
            if (!this.MoneyPlanList.Any(x => x.AccountID == accountID)) return null;

            List<IMoneyPlan> dataList = new List<IMoneyPlan>();
            foreach(var record in this.MoneyPlanList.Where(x => x.AccountID == accountID))
            {
                if(record is MoneyPlanRecord budget)
                {
                    if(budget.Recurrence.NotificationDate <= DateTime.Today && budget.Recurrence.NextOccurence >= DateTime.Today)
                    {
                        dataList.Add(budget);
                    }
                }
            }

            return dataList;
        }
    }
}
