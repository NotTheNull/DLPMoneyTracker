using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Reports.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Core.ReportDTOs;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Reports;
public class GetBudgetAnalysisDataUseCase(
    ILedgerAccountRepository accountRepository,
    ITransactionRepository moneyRepository) : IGetBudgetAnalysisDataUseCase
{
    public List<BudgetAnalysisDTO> Execute(DateRange transactionDateRange)
    {
        List<BudgetAnalysisDTO> listDTO = [];

        JournalAccountSearch search = new()
        {
            IncludeDeleted = true,
            JournalTypes = [Core.Models.LedgerAccounts.LedgerType.Payable, Core.Models.LedgerAccounts.LedgerType.Receivable]
        };

        var listAccounts = accountRepository.GetAccountsBySearch(search);
        var listBudgetAccounts = listAccounts.Where(x => ((INominalAccount)x).BudgetType != BudgetTrackingType.DO_NOT_TRACK).ToList();

        foreach (var account in listBudgetAccounts)
        {
            MoneyRecordSearch moneySearch = new()
            {
                Account = account,
                DateRange = transactionDateRange
            };
            var listTransactions = moneyRepository.Search(moneySearch);
            if (listTransactions?.Any() != true) continue;

            foreach (var t in listTransactions)
            {
                listDTO.Add(new BudgetAnalysisDTO
                {
                    AccountName = account.Description,
                    IncomeOrExpense = account.JournalType == LedgerType.Receivable ? "Income" : "Expense",
                    TransactionDate = t.TransactionDate,
                    TransactionDescription = t.Description,
                    TransactionAmount = t.TransactionAmount
                });
            }
        }

        return listDTO;
    }
}