using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans
{
    public class GetBudgetPlanListUseCase : IGetBudgetPlanListUseCase
    {
        private readonly IBudgetPlanRepository budgetRepository;

        public GetBudgetPlanListUseCase(IBudgetPlanRepository budgetRepository)
        {
            this.budgetRepository = budgetRepository;
        }

        public List<IBudgetPlan> Execute()
        {
            return budgetRepository.GetFullList();
        }
    }
}
