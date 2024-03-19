using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans
{
    public class GetBudgetPlanListByDateRangeUseCase : IGetBudgetPlanListByDateRangeUseCase
    {
        private readonly IBudgetPlanRepository budgetRepository;

        public GetBudgetPlanListByDateRangeUseCase(IBudgetPlanRepository budgetRepository)
        {
            this.budgetRepository = budgetRepository;
        }

        public List<IBudgetPlan> Execute(DateRange dateRange)
        {
            BudgetPlanSearch search = new BudgetPlanSearch
            {
                DateRange = dateRange
            };

            return budgetRepository.Search(search);
        }
    }
}
