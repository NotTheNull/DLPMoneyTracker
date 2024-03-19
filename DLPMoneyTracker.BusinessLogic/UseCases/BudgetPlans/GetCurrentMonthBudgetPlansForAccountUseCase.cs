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
    public class GetCurrentMonthBudgetPlansForAccountUseCase : IGetCurrentMonthBudgetPlansForAccountUseCase
    {
        private readonly IBudgetPlanRepository budgetRepository;

        public GetCurrentMonthBudgetPlansForAccountUseCase(IBudgetPlanRepository budgetRepository)
        {
            this.budgetRepository = budgetRepository;
        }

        public List<IBudgetPlan> Execute(Guid accountUID)
        {
            BudgetPlanSearch search = new BudgetPlanSearch
            {
                AccountUID = accountUID,
                DateRange = new Core.DateRange(DateTime.Today.Year, DateTime.Today.Month)
            };

            return budgetRepository.Search(search);
        }
    }
}
