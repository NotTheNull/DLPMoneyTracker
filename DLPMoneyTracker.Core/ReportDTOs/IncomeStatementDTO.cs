using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.ReportDTOs;
public class IncomeStatementDTO
{
    public List<IncomeStatementItemDTO> IncomeList { get; set; } = [];
    public List<IncomeStatementItemDTO> ExpenseList { get; set; } = [];
}

public class IncomeStatementItemDTO
{
    public string AccountName { get; set; } = string.Empty;
    public decimal Total { get; set; } = decimal.Zero;
}
