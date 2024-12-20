﻿using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
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
        private readonly ILedgerAccountRepository accountRepository;
        private readonly IDLPConfig config;

        public JSONBudgetPlanRepository(ILedgerAccountRepository ledgerRepository, IDLPConfig config)
        {
            _year = DateTime.Today.Year;
            this.accountRepository = ledgerRepository;
            this.config = config;
            this.LoadFromFile();
        }

        public List<IBudgetPlan> BudgetPlanList { get; set; } = new List<IBudgetPlan>();

        public string FilePath { get { return Path.Combine(config.JSONFilePath, "Data", "JournalPlan.json"); } }


        public void LoadFromFile()
        {
            this.BudgetPlanList.Clear();
            string json = string.Empty;
            if (File.Exists(this.FilePath))
            {
                json = File.ReadAllText(FilePath);
            }
            else
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(json)) return;

            var dataList = (List<JournalPlanJSON>)JsonSerializer.Deserialize(json, typeof(List<JournalPlanJSON>));
            if (dataList?.Any() != true) return;

            BudgetPlanFactory budgetFactory = new BudgetPlanFactory();
            ScheduleRecurrenceFactory recurrenceFactory = new ScheduleRecurrenceFactory();
            JSONScheduleRecurrenceAdapter adapter = new JSONScheduleRecurrenceAdapter();
            foreach(var data in dataList)
            {
                adapter.ImportJSON(data.RecurrenceJSON);
                var recurrence = recurrenceFactory.Build(adapter);
                IJournalAccount debit = accountRepository.GetAccountByUID(data.DebitAccountId);
                IJournalAccount credit = accountRepository.GetAccountByUID(data.CreditAccountId);
                var newPlan = budgetFactory.Build(data.PlanType, data.UID, data.Description, debit, credit, data.ExpectedAmount, recurrence);
                BudgetPlanList.Add(newPlan);
            }
        }

        public void SaveToFile()
        {
            if (this.BudgetPlanList.Any() != true) return;

            List<JournalPlanJSON> listJSONPlans = new List<JournalPlanJSON>();
            foreach(var plan in this.BudgetPlanList)
            {
                JournalPlanJSON jsonPlan = new JournalPlanJSON();
                jsonPlan.Copy(plan);
                listJSONPlans.Add(jsonPlan);
            }

            string json = JsonSerializer.Serialize<List<JournalPlanJSON>>(listJSONPlans);
            File.WriteAllText(this.FilePath, json);
        }


        public List<IBudgetPlan> Search(BudgetPlanSearch search)
        {
            var listPlans = this.BudgetPlanList.Where(x => search.DateRange.IsWithinRange(x.NextOccurrence));

            if(search.AccountUID != null && search.AccountUID != Guid.Empty)
            {
                listPlans = listPlans.Where(x => x.DebitAccountId == search.AccountUID || x.CreditAccountId == search.AccountUID);
            }

            if(!string.IsNullOrWhiteSpace(search.FilterText))
            {
                listPlans = listPlans.Where(x => x.Description.Contains(search.FilterText));
            }

            return listPlans.ToList();
        }

        public List<IBudgetPlan> GetUpcomingPlansForAccount(Guid accountUID)
        {
            if (!this.BudgetPlanList.Any(x => x.CreditAccountId == accountUID || x.DebitAccountId == accountUID)) return null;

            List<IBudgetPlan> listPlans = new List<IBudgetPlan>();
            foreach(var record in this.BudgetPlanList.Where(x => x.CreditAccountId == accountUID || x.DebitAccountId == accountUID))
            {
                // Adding five days for Next Occurrence check to account for weekends & holidays that might delay the bill posting
                if (record.NotificationDate <= DateTime.Today && record.NextOccurrence.AddDays(5) >= DateTime.Today)
                {
                    listPlans.Add(record);
                }
            }

            return listPlans;
        }

        public List<IBudgetPlan> GetFullList()
        {
            return this.BudgetPlanList.ToList();
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
            if(existingPlan is null)
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
            return this.BudgetPlanList.Where(x => x.DebitAccountId == accountUID || x.CreditAccountId == accountUID).ToList();
        }

        public List<IBudgetPlan> GetPlanListByType(BudgetPlanType planType)
        {
            return this.BudgetPlanList.Where(x => x.PlanType == planType).ToList();
        }
    }
}
