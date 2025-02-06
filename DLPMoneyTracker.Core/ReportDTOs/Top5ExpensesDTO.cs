using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.ReportDTOs
{
    public class TopExpenseDTO
    {
        public DateRange Dates { get; set; } = new(DateTime.MinValue, DateTime.MaxValue);
        public decimal TotalExpenseBalance { get; set; } = default;

        public List<ExpenseReportRecord> DataSet { get; set; } = new();



    }

    public class ExpenseReportRecord
    {
        private readonly DateRange dates;

        public ExpenseReportRecord(DateRange dates)
        {
            this.dates = dates;
        }

        
        public IJournalAccount Account { get; set; }
        public decimal Balance { get; set; }
        public decimal ExpensePct { get; set; }

        public DateTime BegDate { get { return dates.Begin; } }
        public DateTime EndDate { get { return dates.End; } }
    }
}
