using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Reports.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Core.ReportDTOs;
using System.Numerics;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Reports;

public class GetIncomeStatementDataUseCase(ILedgerAccountRepository repoAccount, ITransactionRepository repoTransaction) : IGetIncomeStatementDataUseCase
{
    //private readonly List<LedgerType> _incomeTypes = [LedgerType.Receivable];
    //private readonly List<LedgerType> _expenseTypes = [LedgerType.Payable, LedgerType.LiabilityLoan];

    public IncomeStatementDTO Execute(IncomeStatementRequest request)
    {
        IncomeStatementDTO dto = new();
        var incomeAccounts = repoAccount.GetAccountsBySearch(JournalAccountSearch.GetAccountsByType([LedgerType.Receivable]));

        foreach (var account in incomeAccounts)
        {
            decimal total = repoTransaction.GetAccountBalanceByMonth(account.Id, request.Year, request.Month);
            if (total == decimal.Zero) continue;

            dto.IncomeList.Add(new() { AccountName = account.Description, Total = total });
        }

        var expenseAccounts = repoAccount.GetAccountsBySearch(JournalAccountSearch.GetAccountsByType([LedgerType.Payable]));
        foreach (var account in expenseAccounts)
        {
            decimal total = repoTransaction.GetAccountBalanceByMonth(account.Id, request.Year, request.Month);
            if (total == decimal.Zero) continue;

            dto.ExpenseList.Add(new() { AccountName = account.Description, Total = total });
        }

        var loanAccounts = repoAccount.GetAccountsBySearch(JournalAccountSearch.GetAccountsByType([LedgerType.LiabilityLoan]));
        foreach(var account in loanAccounts)
        {
            var transactions = repoTransaction.Search(new() { Account = account, DateRange = DateRange.GetMonthRange(request.Year, request.Month) });
            if (transactions?.Any() != true) continue;

            decimal total = transactions.Sum(s => s.TransactionAmount);
            dto.ExpenseList.Add(new() { AccountName = account.Description, Total = total });
        }

        return dto;
    }
}

public class IncomeStatementRequest
{
    public int Year { get; set; }
    public int Month { get; set; }
}