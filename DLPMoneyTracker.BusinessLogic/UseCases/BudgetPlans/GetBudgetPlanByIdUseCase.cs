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
    public class GetBudgetPlanByIdUseCase : IGetBudgetPlanByIdUseCase
    {
        private readonly IBudgetPlanRepository planRepo;

        public GetBudgetPlanByIdUseCase(IBudgetPlanRepository planRepo)
        {
            this.planRepo = planRepo;
        }

        public IBudgetPlan Execute(Guid uid)
        {
            return planRepo.GetPlan(uid);
        }
    }
}
