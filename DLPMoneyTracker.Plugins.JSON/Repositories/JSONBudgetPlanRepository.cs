using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.JSON.Adapters;
using DLPMoneyTracker.Plugins.JSON.Models;
using System.Text.Json;

namespace DLPMoneyTracker.Plugins.JSON.Repositories
{
    public class JSONBudgetPlanRepository : IBudgetPlanRepository, IJSONRepository
    {
        private readonly int _year;
        private readonly ILedgerAccountRepository accountRepository;
        private readonly IDLPConfig config;

        public JSONBudgetPlanRepository(ILedgerAccountRepository ledgerRepository, IDLPConfig config)
        {
            _year = DateTime.Today.Year;
            this.accountRepository = ledgerRepository;
            this.config = config;
            this.LoadFromFile();
        }

        public List<IBudgetPlan> BudgetPlanList { get; set; } = [];

        public string FilePath => Path.Combine(config.JSONFilePath, "Data", "JournalPlan.json");

        public void LoadFromFile()
        {
            this.BudgetPlanList.Clear();
            if (!File.Exists(this.FilePath)) return;

            string json = File.ReadAllText(FilePath);
            if (string.IsNullOrWhiteSpace(json)) return;

            var dataList = (List<JournalPlanJSON>?)JsonSerializer.Deserialize(json, typeof(List<JournalPlanJSON>));
            if (dataList?.Any() != true) return;

            JSONScheduleRecurrenceAdapter adapter = new();
            foreach (var data in dataList)
            {
                adapter.ImportJSON(data.RecurrenceJSON);
                var recurrence = ScheduleRecurrenceFactory.Build(adapter);
                IJournalAccount debit = accountRepository.GetAccountByUID(data.DebitAccountId);
                IJournalAccount credit = accountRepository.GetAccountByUID(data.CreditAccountId);
                var newPlan = BudgetPlanFactory.Build(data.PlanType, data.UID, data.Description, debit, credit, data.ExpectedAmount, recurrence);
                BudgetPlanList.Add(newPlan);
            }
        }

        public void SaveToFile()
        {
            if (this.BudgetPlanList.Any() != true) return;

            List<JournalPlanJSON> listJSONPlans = [];
            foreach (var plan in this.BudgetPlanList)
            {
                JournalPlanJSON jsonPlan = new();
                jsonPlan.Copy(plan);
                listJSONPlans.Add(jsonPlan);
            }

            string json = JsonSerializer.Serialize<List<JournalPlanJSON>>(listJSONPlans);
            File.WriteAllText(this.FilePath, json);
        }

        public List<IBudgetPlan> Search(BudgetPlanSearch search)
        {
            ArgumentNullException.ThrowIfNull(search);
            var listPlans = this.BudgetPlanList.Where(x => search.DateRange.IsWithinRange(x.NextOccurrence));

            if (search.AccountUID != Guid.Empty)
            {
                listPlans = listPlans.Where(x => x.DebitAccountId == search.AccountUID || x.CreditAccountId == search.AccountUID);
            }

            if (!string.IsNullOrWhiteSpace(search.FilterText))
            {
                listPlans = listPlans.Where(x => x.Description.Contains(search.FilterText));
            }

            return [.. listPlans];
        }

        public List<IBudgetPlan> GetUpcomingPlansForAccount(Guid accountUID)
        {
            if (!this.BudgetPlanList.Any(x => x.CreditAccountId == accountUID || x.DebitAccountId == accountUID)) return [];

            DateRange payperiod = new(config.Period.CurrentPayPeriod, config.Period.NextPayPeriod);

            List<IBudgetPlan> listPlans = [];
            foreach (var record in this.BudgetPlanList.Where(x => x.CreditAccountId == accountUID || x.DebitAccountId == accountUID))
            {
                if(payperiod.IsWithinRange(record.NextOccurrence))
                {
                    listPlans.Add(record);
                }
            }

            return listPlans;
        }

        public List<IBudgetPlan> GetFullList()
        {
            return [.. this.BudgetPlanList];
        }

        public void DeletePlan(Guid planUID)
        {
            var existingPlan = this.BudgetPlanList.FirstOrDefault(x => x.UID == planUID);
            if (existingPlan is null) return;

            this.BudgetPlanList.Remove(existingPlan);
            this.SaveToFile();
        }

        public void SavePlan(IBudgetPlan plan)
        {
            ArgumentNullException.ThrowIfNull(plan);
            if (plan.PlanType == BudgetPlanType.NotSet) throw new InvalidOperationException("Budget Plans MUST have a set type");
            if (plan.UID == Guid.Empty) throw new InvalidOperationException("Plan UID cannot be EMPTY");

            var existingPlan = this.BudgetPlanList.FirstOrDefault(x => x.UID == plan.UID);
            if (existingPlan is null)
            {
                this.BudgetPlanList.Add(plan);
            }
            else
            {
                existingPlan.Copy(plan);
            }

            this.SaveToFile();
        }

        public int GetRecordCount()
        {
            return this.BudgetPlanList.Count;
        }

        public List<IBudgetPlan> GetAllPlansForAccount(Guid accountUID)
        {
            return [.. this.BudgetPlanList.Where(x => x.DebitAccountId == accountUID || x.CreditAccountId == accountUID)];
        }

        public List<IBudgetPlan> GetPlanListByType(BudgetPlanType planType)
        {
            return [.. this.BudgetPlanList.Where(x => x.PlanType == planType)];
        }
    }
}