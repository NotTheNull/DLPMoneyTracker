using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.ReportDTOs;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Reports.Interfaces
{
    public interface IGetTop5ExpensesUseCase
    {
        TopExpenseDTO Execute(DateRange dates, Guid moneyAccountId);
    }
}