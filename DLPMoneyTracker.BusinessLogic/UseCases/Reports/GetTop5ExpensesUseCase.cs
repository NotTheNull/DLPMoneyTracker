using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
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
    public class GetTop5ExpensesUseCase
    {
        private readonly ITransactionRepository repoTransactions;

        public GetTop5ExpensesUseCase(ITransactionRepository repoTransactions)
        {
            this.repoTransactions = repoTransactions;
        }

        public TopExpenseDTO Execute(DateRange dates)
        {
            AccountBalanceSearch search = new();
            search.AccountTypes = [LedgerType.Payable];
            search.Dates = dates;


            var result = repoTransactions.GetAccountBalancesBySearch(search);
            if (result?.Any() != true) return null;

            TopExpenseDTO dto = new();
            dto.Dates = dates;
            dto.TotalExpenseBalance = result.Sum(s => s.Value);
            decimal otherExpenseTotal = result.OrderByDescending(o => o.Value).Skip(5).Sum(s => s.Value);
            foreach(var r in result.OrderByDescending(o => o.Value).Take(5))
            {
                ExpenseReportRecord record = new(dates)
                {
                    Account = r.Key,
                    Balance = r.Value,
                    ExpensePct = (r.Value / dto.TotalExpenseBalance) * 100
                };
                dto.DataSet.Add(record);
            }
            dto.DataSet.Add(new ExpenseReportRecord(dates)
            {
                Account = null,
                Balance = otherExpenseTotal,
                ExpensePct = (otherExpenseTotal / dto.TotalExpenseBalance) * 100
            });


            return dto;
        }
        
    }
}
