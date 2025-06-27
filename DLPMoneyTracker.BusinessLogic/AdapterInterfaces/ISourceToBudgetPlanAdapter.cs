using DLPMoneyTracker.Core.Models.BudgetPlan;

namespace DLPMoneyTracker.BusinessLogic.AdapterInterfaces
{
    public interface ISourceToBudgetPlanAdapter<T> : IBudgetPlan, IDLPAdapter<T>
    {
    }
}