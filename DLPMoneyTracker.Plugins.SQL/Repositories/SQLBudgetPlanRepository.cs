using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.SQL.Adapters;
using DLPMoneyTracker.Plugins.SQL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.SQL.Repositories
{
    public class SQLBudgetPlanRepository : IBudgetPlanRepository
    {
        
        public void DeletePlan(Guid planUID)
        {
            using (DataContext context = new DataContext())
            {
                var existingPlan = context.BudgetPlans.FirstOrDefault(x => x.PlanUID == planUID);
                if (existingPlan is null) return;

                context.BudgetPlans.Remove(existingPlan);
                context.SaveChanges();
            }
        }

        public List<IBudgetPlan> GetFullList()
        {
            List<IBudgetPlan> listPlansFinal = new List<IBudgetPlan>();
            using(DataContext context = new DataContext())
            {
                var listPlans = context.BudgetPlans.ToList();
                if (listPlans?.Any() != true) return listPlansFinal;

                foreach(var src in listPlans)
                {
                    listPlansFinal.Add(this.SourceToPlan(src, context));
                }
            }

            return listPlansFinal;
        }

        private IBudgetPlan SourceToPlan(BudgetPlan src, DataContext context)
        {
            SQLSourceToBudgetPlanAdapter planAdapter = new SQLSourceToBudgetPlanAdapter(context);
            BudgetPlanFactory planFactory = new BudgetPlanFactory();

            planAdapter.ImportSource(src);
            return planFactory.Build(planAdapter);
        }

        public List<IBudgetPlan> GetUpcomingPlansForAccount(Guid accountUID)
        {
            List<IBudgetPlan> listPlans = new List<IBudgetPlan>();
            using (DataContext context = new DataContext())
            {
                var listPlansLoop = context.BudgetPlans.Where(x => x.Credit.AccountUID == accountUID || x.Debit.AccountUID == accountUID);
                foreach(var src in listPlansLoop)
                {
                    IBudgetPlan plan = this.SourceToPlan(src, context);
                    if(plan.NotificationDate <= DateTime.Today && plan.NextOccurrence.AddDays(5) >= DateTime.Today)
                    {
                        listPlans.Add(plan);
                    }
                }
            }

            return listPlans;
        }

        public void SavePlan(IBudgetPlan plan)
        {
            using (DataContext context = new DataContext())
            {
                SQLSourceToBudgetPlanAdapter adapter = new SQLSourceToBudgetPlanAdapter(context);
                var existingPlan = context.BudgetPlans.FirstOrDefault(x => x.PlanUID == plan.UID);
                if(existingPlan is null)
                {
                    existingPlan = new BudgetPlan();
                    context.BudgetPlans.Add(existingPlan);
                }
                adapter.ExportSource(ref existingPlan);
                context.SaveChanges();
            }
        }

        public List<IBudgetPlan> Search(BudgetPlanSearch search)
        {
            List<IBudgetPlan> listPlanFinal = new List<IBudgetPlan>();
            using(DataContext context = new DataContext())
            {
                var listPlanQuery = context.BudgetPlans.Where(x => x.ExpectedAmount != decimal.Zero);
                if(search.AccountUID != null && search.AccountUID != Guid.Empty)
                {
                    listPlanQuery = listPlanQuery.Where(x => x.Debit.AccountUID == search.AccountUID || x.Credit.AccountUID == search.AccountUID);
                }

                if(!string.IsNullOrWhiteSpace(search.FilterText))
                {
                    listPlanQuery = listPlanQuery.Where(x => x.Description.Contains(search.FilterText));
                }

                var listPlanLoop = listPlanQuery.ToList();
                if (listPlanLoop?.Any() != true) return listPlanFinal;

                foreach(var src in listPlanLoop)
                {
                    IBudgetPlan plan = SourceToPlan(src, context);

                    if(search.DateRange.IsWithinRange(plan.NextOccurrence))
                    {
                        listPlanFinal.Add(plan);
                    }
                }
            }

            return listPlanFinal;
        }

        public int GetRecordCount()
        {
            using(DataContext context = new DataContext())
            {
                return context.BudgetPlans.Count();
            }
        }

        public List<IBudgetPlan> GetAllPlansForAccount(Guid accountUID)
        {
            List<IBudgetPlan> listPlanFinal = new List<IBudgetPlan>();
            using (DataContext context = new DataContext())
            {
                var listPlanLoop = context.BudgetPlans.Where(x => x.Debit.AccountUID == accountUID || x.Credit.AccountUID == accountUID).ToList();
                if (listPlanLoop?.Any() != true) return listPlanFinal;

                foreach(var src in listPlanLoop)
                {
                    IBudgetPlan plan = SourceToPlan(src, context);
                    listPlanFinal.Add(plan);
                }
            }

            return listPlanFinal;
        }
    }
}
