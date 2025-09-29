using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Plugins.SQL.Adapters;
using DLPMoneyTracker.Plugins.SQL.Data;

namespace DLPMoneyTracker.Plugins.SQL.Repositories
{
    public class SQLBudgetPlanRepository(IDLPConfig config, ILedgerAccountRepository accountRepository) : IBudgetPlanRepository
    {
        private readonly IDLPConfig config = config;
        private readonly ILedgerAccountRepository accountRepository = accountRepository;

        public void DeletePlan(Guid planUID)
        {
            using DataContext context = new(config);
            var existingPlan = context.BudgetPlans.FirstOrDefault(x => x.PlanUID == planUID);
            if (existingPlan is null) return;

            context.BudgetPlans.Remove(existingPlan);
            context.SaveChanges();
        }

        public List<IBudgetPlan> GetFullList()
        {
            List<IBudgetPlan> listPlansFinal = [];
            using (DataContext context = new(config))
            {
                var listPlans = context.BudgetPlans.ToList();
                if (listPlans?.Any() != true) return listPlansFinal;

                foreach (var src in listPlans)
                {
                    listPlansFinal.Add(this.SourceToPlan(src, context));
                }
            }

            return listPlansFinal;
        }

        private IBudgetPlan SourceToPlan(BudgetPlan src, DataContext context)
        {
            SQLSourceToBudgetPlanAdapter planAdapter = new(context, accountRepository);
            planAdapter.ImportSource(src);
            return BudgetPlanFactory.Build(planAdapter);
        }

        public List<IBudgetPlan> GetUpcomingPlansForAccount(Guid accountUID)
        {
            DateRange payPeriod = new(config.Period.CurrentPayPeriod, config.Period.NextPayPeriod);
            List<IBudgetPlan> listPlans = [];
            using (DataContext context = new(config))
            {
                var listPlansLoop = context.BudgetPlans
                    .Where(x => (x.Credit != null && x.Credit.AccountUID == accountUID) || (x.Debit != null && x.Debit.AccountUID == accountUID))
                    .ToList();
                foreach (var src in listPlansLoop)
                {
                    IBudgetPlan plan = this.SourceToPlan(src, context);
                    if(payPeriod.IsWithinRange(plan.NextOccurrence))
                    {
                        listPlans.Add(plan);
                    }
                }
            }

            return listPlans;
        }

        public void SavePlan(IBudgetPlan plan)
        {
            using DataContext context = new DataContext(config);
            SQLSourceToBudgetPlanAdapter adapter = new(context, accountRepository);
            adapter.Copy(plan);

            var existingPlan = context.BudgetPlans.FirstOrDefault(x => x.PlanUID == plan.UID);
            if (existingPlan is null)
            {
                existingPlan = new BudgetPlan();
                context.BudgetPlans.Add(existingPlan);
            }
            adapter.ExportSource(ref existingPlan);
            context.SaveChanges();
        }

        public List<IBudgetPlan> Search(BudgetPlanSearch search)
        {
            List<IBudgetPlan> listPlanFinal = [];
            using (DataContext context = new(config))
            {
                var listPlanQuery = context.BudgetPlans.Where(x => x.ExpectedAmount != decimal.Zero);
                if (search.AccountUID != Guid.Empty)
                {
                    listPlanQuery = listPlanQuery.Where(x => (x.Credit != null && x.Credit.AccountUID == search.AccountUID) || (x.Debit != null && x.Debit.AccountUID == search.AccountUID));
                }

                if (!string.IsNullOrWhiteSpace(search.FilterText))
                {
                    listPlanQuery = listPlanQuery.Where(x => x.Description.Contains(search.FilterText));
                }

                var listPlanLoop = listPlanQuery.ToList();
                if (listPlanLoop?.Any() != true) return listPlanFinal;

                foreach (var src in listPlanLoop)
                {
                    IBudgetPlan plan = SourceToPlan(src, context);

                    if (search.DateRange.IsWithinRange(plan.NextOccurrence))
                    {
                        listPlanFinal.Add(plan);
                    }
                }
            }

            return listPlanFinal;
        }

        public int GetRecordCount()
        {
            using DataContext context = new(config);
            return context.BudgetPlans.Count();
        }

        public List<IBudgetPlan> GetAllPlansForAccount(Guid accountUID)
        {
            List<IBudgetPlan> listPlanFinal = [];
            using (DataContext context = new(config))
            {
                var listPlanLoop = context.BudgetPlans.Where(x => (x.Credit != null && x.Credit.AccountUID == accountUID) || (x.Debit != null && x.Debit.AccountUID == accountUID)).ToList();
                if (listPlanLoop?.Any() != true) return listPlanFinal;

                foreach (var src in listPlanLoop)
                {
                    IBudgetPlan plan = SourceToPlan(src, context);
                    listPlanFinal.Add(plan);
                }
            }

            return listPlanFinal;
        }

        public List<IBudgetPlan> GetPlanListByType(BudgetPlanType planType)
        {
            List<IBudgetPlan> listPlanFinal = [];
            using (DataContext context = new(config))
            {
                var listPlanLoop = context.BudgetPlans.Where(x => x.PlanType == planType).ToList();
                if (listPlanLoop?.Any() != true) return listPlanFinal;

                foreach (var src in listPlanLoop)
                {
                    IBudgetPlan plan = SourceToPlan(src, context);
                    listPlanFinal.Add(plan);
                }
            }

            return listPlanFinal;
        }
    }
}