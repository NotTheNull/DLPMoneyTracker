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
    public class SaveBudgetPlanUseCase : ISaveBudgetPlanUseCase
    {
        private readonly IBudgetPlanRepository budgetRepository;

        public SaveBudgetPlanUseCase(IBudgetPlanRepository budgetRepository)
        {
            this.budgetRepository = budgetRepository;
        }

        public void Execute(IBudgetPlan plan)
        {
            budgetRepository.SavePlan(plan);
        }
    }
}
