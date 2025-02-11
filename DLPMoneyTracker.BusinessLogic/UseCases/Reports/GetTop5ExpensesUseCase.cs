using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Reports.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Core.ReportDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Reports
{
    public class GetTop5ExpensesUseCase : IGetTop5ExpensesUseCase
    {
        private readonly ITransactionRepository repoTransactions;

        public GetTop5ExpensesUseCase(ITransactionRepository repoTransactions)
        {
            this.repoTransactions = repoTransactions;
        }

        public TopExpenseDTO Execute(DateRange dates, Guid moneyAccountId)
        {
            AccountBalanceSearch search = new();
            search.MoneyAccountId = moneyAccountId;
            search.AccountTypes = [LedgerType.Payable];
            search.Dates = dates;


            var result = repoTransactions.GetAccountBalancesBySearch(search);
            if (result?.Any() != true) return null;

            TopExpenseDTO dto = new();
            dto.Dates = dates;
            dto.TotalExpenseBalance = result.Sum(s => s.Item2);
            decimal otherExpenseTotal = result.OrderByDescending(o => o.Item2).Skip(5).Sum(s => s.Item2);
            foreach (var r in result.OrderByDescending(o => o.Item2).Take(5))
            {
                ExpenseReportRecord record = new(dates)
                {
                    Account = r.Item1,
                    Balance = r.Item2,
                    ExpensePct = (r.Item2 / dto.TotalExpenseBalance) * 100
                };
                dto.DataSet.Add(record);
            }
            dto.DataSet.Add(new ExpenseReportRecord(dates)
            {
                Account = new SpecialAccount() { Description = "Remaining Account" },
                Balance = otherExpenseTotal,
                ExpensePct = (otherExpenseTotal / dto.TotalExpenseBalance) * 100
            });


            return dto;
        }

    }
}
