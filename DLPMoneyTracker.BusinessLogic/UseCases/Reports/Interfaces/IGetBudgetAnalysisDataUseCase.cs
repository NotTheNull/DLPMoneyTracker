using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.ReportDTOs;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Reports.Interfaces
{
    public interface IGetBudgetAnalysisDataUseCase
    {
        List<BudgetAnalysisDTO> Execute(DateRange transactionDateRange);
    }
}