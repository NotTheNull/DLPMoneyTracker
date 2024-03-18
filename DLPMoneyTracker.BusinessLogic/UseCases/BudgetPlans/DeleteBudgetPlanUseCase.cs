using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans
{
    public class DeleteBudgetPlanUseCase : IDeleteBudgetPlanUseCase
    {
        private readonly IBudgetPlanRepository budgetRepository;

        public DeleteBudgetPlanUseCase(IBudgetPlanRepository budgetRepository)
        {
            this.budgetRepository = budgetRepository;
        }

        public void Execute(Guid planUID)
        {
            budgetRepository.DeletePlan(planUID);
        }
    }
}
