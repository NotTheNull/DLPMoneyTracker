using DLPMoneyTracker.Core.ReportDTOs;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Reports.Interfaces;
public interface IGetIncomeStatementDataUseCase
{
    IncomeStatementDTO Execute(IncomeStatementRequest request);
}