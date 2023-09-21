using DLPMoneyTracker.Data.Common;
using DLPMoneyTracker.Data.TransactionModels.JournalPlan;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DLPMoneyTracker.Data
{
    public interface IJournalPlanner
    {
        string FilePath { get; }
        ReadOnlyCollection<IJournalPlan> JournalPlanList { get; }

        void AddPlan(IJournalPlan journalPlan);

        void RemovePlan(IJournalPlan journalPlan);

        void ClearRecordList();

        void LoadFromFile();

        void SaveToFile();

        IEnumerable<IJournalPlan> GetUpcomingPlansForAccount(Guid accountId);

        IEnumerable<IJournalPlan> GetPlansForDateRange(DateRange range);

        void Copy(IJournalPlanner journalPlanner);

        //#pragma warning disable CS0618 // Type or member is obsolete
        //        void Convert(IMoneyPlanner moneyPlanner);
        //#pragma warning restore CS0618 // Type or member is obsolete
    }

    public class JournalPlanner : IJournalPlanner
    {
        private readonly ITrackerConfig _config;
        private int _year;

        public JournalPlanner(ITrackerConfig config) : this(config, DateTime.Today.Year)
        {
        }

        public JournalPlanner(ITrackerConfig config, int year)
        {
            _config = config;
            _year = year;
            if (!Directory.Exists(this.FolderPath))
            {
                Directory.CreateDirectory(this.FolderPath);
            }
            this.LoadFromFile();
        }

        private string FolderPath
        { get { return AppConfigSettings.DATA_FOLDER_PATH.Replace(AppConfigSettings.YEAR_FOLDER_PLACEHOLDER, _year.ToString()); } }
        public string FilePath
        { get { return string.Concat(this.FolderPath, "JournalPlan.json"); } }

        private List<IJournalPlan> _planList = new List<IJournalPlan>();
        public ReadOnlyCollection<IJournalPlan> JournalPlanList
        { get { return _planList.AsReadOnly(); } }

        public void AddPlan(IJournalPlan journalPlan)
        {
            var existing = this.JournalPlanList.FirstOrDefault(x => x.UID == journalPlan.UID);
            if (existing != null)
            {
                _planList.Remove(existing);
            }
            _planList.Add(journalPlan);
        }

        public void RemovePlan(IJournalPlan journalPlan)
        {
            if (!_planList.Any(x => x.UID == journalPlan.UID)) return;
            _planList.Remove(journalPlan);
        }

        public void ClearRecordList()
        {
            _planList.Clear();
        }

        public void LoadFromFile()
        {
            _planList.Clear();
            if (!File.Exists(this.FilePath)) return;

            string json = File.ReadAllText(FilePath);
            if (string.IsNullOrWhiteSpace(json)) return;

            var dataList = (List<JournalPlanJSON>)JsonSerializer.Deserialize(json, typeof(List<JournalPlanJSON>));
            if (dataList is null || !dataList.Any()) return;

            foreach (var data in dataList)
            {
                var plan = JournalPlanFactory.Build(_config, data);
                if (plan is null) continue;
                _planList.Add(plan);
            }
        }

        public void SaveToFile()
        {
            string json = JsonSerializer.Serialize(_planList);
            File.WriteAllText(FilePath, json);
        }

        public IEnumerable<IJournalPlan> GetUpcomingPlansForAccount(Guid accountId)
        {
            if (!this.JournalPlanList.Any(x => x.CreditAccountId == accountId || x.DebitAccountId == accountId)) return null;

            List<IJournalPlan> listPlans = new List<IJournalPlan>();
            foreach (var record in this.JournalPlanList.Where(x => x.CreditAccountId == accountId || x.DebitAccountId == accountId))
            {
                // Adding three days for Next Occurrence check to account for weekends & holidays that might delay the bill posting
                if (record.NotificationDate <= DateTime.Today && record.NextOccurrence.AddDays(3) >= DateTime.Today)
                {
                    listPlans.Add(record);
                }
            }

            return listPlans;
        }

        public IEnumerable<IJournalPlan> GetPlansForDateRange(DateRange range)
        {
            if (range is null) throw new ArgumentNullException("Date Range");
            if (range.Begin < new DateTime(DateTime.Today.Year, 1, 1)) return null;
            if (range.End > new DateTime(DateTime.Today.Year, 12, 31)) return null;

            return this.JournalPlanList.Where(x => range.IsWithinRange(x.NextOccurrence)).ToList();
        }

        public void Copy(IJournalPlanner journalPlanner)
        {
            this.ClearRecordList();
            foreach (var plan in journalPlanner.JournalPlanList)
            {
                this.AddPlan(plan);
            }
        }

        //#pragma warning disable CS0618 // Type or member is obsolete
        //        public void Convert(IMoneyPlanner moneyPlanner)
        //        {
        //            return;
        //            //if (moneyPlanner?.MoneyPlanList?.Any() != true) return;

        //            //_planList.Clear();
        //            //foreach(var m in moneyPlanner.MoneyPlanList)
        //            //{
        //            //    var plan = JournalPlanFactory.Build(_config, m);
        //            //    if (plan is null) continue;
        //            //    _planList.Add(plan);
        //            //}
        //            //this.SaveToFile();

        //            // Hold off on deleting
        //            //moneyPlanner.ClearRecordList();
        //            //moneyPlanner.SaveToFile();

        //        }
        //#pragma warning restore CS0618 // Type or member is obsolete
    }
}