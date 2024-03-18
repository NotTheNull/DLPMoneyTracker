using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Plugins.JSON.Adapters;
using DLPMoneyTracker.Plugins.JSON.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.JSON.Repositories
{
    public class JSONBudgetPlanRepository : IBudgetPlanRepository, IJSONRepository
    {
        private int _year;
        private readonly ILedgerAccountRepository ledgerRepository;

        public JSONBudgetPlanRepository(ILedgerAccountRepository ledgerRepository)
        {
            _year = DateTime.Today.Year;
            this.ledgerRepository = ledgerRepository;

            this.LoadFromFile();
        }

        public List<IBudgetPlan> BudgetPlanList { get; set; } = new List<IBudgetPlan>();


        private string FolderPath { get { return AppSettings.DATA_FOLDER_PATH.Replace(AppSettings.YEAR_FOLDER_PLACEHOLDER, _year.ToString()); } }
        public string FilePath { get { return string.Concat(this.FolderPath, "JournalPlan.json"); } }


        public void LoadFromFile()
        {
            this.BudgetPlanList.Clear();
            if (!File.Exists(this.FilePath)) return;

            string json = File.ReadAllText(FilePath);
            if (string.IsNullOrWhiteSpace(json)) return;

            var dataList = (List<JournalPlanJSON>)JsonSerializer.Deserialize(json, typeof(List<JournalPlanJSON>));
            if (dataList?.Any() != true) return;

            BudgetPlanFactory factory = new BudgetPlanFactory(ledgerRepository);
            JSONScheduleRecurrenceAdapter adapter = new JSONScheduleRecurrenceAdapter();
            foreach(var data in dataList)
            {
                adapter.ImportJSON(data.RecurrenceJSON);
                BudgetPlanList.Add(factory.Build(data.PlanType.ToBudgetPlanType(), data.UID, data.Description, data.DebitAccountId, data.CreditAccountId, data.ExpectedAmount, adapter));
            }
        }

    }
}
