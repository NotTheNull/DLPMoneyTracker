using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DLPMoneyTracker.HTMLReports.MonthlyExpense
{
    public class MonthlyExpenseReportVM : BaseViewModel, IWebViewModel
    {
        private readonly string htmlRowTemplate;
        private readonly string htmlChartTemplate;
        private readonly string htmlReportTemplate;

        private ILedger _ledger;
        private ITrackerConfig _config;

        private string _html;
        public string HTMLSource { get { return _html; } }

        private int _month;

        public int SelectedMonth
        {
            get { return _month; }
            set
            {
                if (value < 1) _month = 1;
                else if (value > 12) _month = 12;
                else _month = value;
                NotifyPropertyChanged(nameof(this.SelectedMonth));
            }
        }

        private const string RESOURCE_ROW = "PART_MoneyChartRow.html";
        private const string RESOURCE_CHART = "PART_MoneyChart.html";
        private const string RESOURCE_REPORT = "MonthlyExpenseTemplate.html";

        public MonthlyExpenseReportVM(ILedger ledger, ITrackerConfig config) : base()
        {
            _ledger = ledger;
            _config = config;
            _month = DateTime.Today.Month;
            htmlRowTemplate = UICore.GetResourceText(RESOURCE_ROW);
            htmlChartTemplate = UICore.GetResourceText(RESOURCE_CHART);
            htmlReportTemplate = UICore.GetResourceText(RESOURCE_REPORT);
        }

        private const string ID_ROW_DESC = "{*DESCRIPTION*}";
        private const string ID_ROW_TOTAL = "{*TOTAL*}";
        private const string ID_CHART_DETAIL = "{*MONEY ROWS*}";
        private const string ID_CHART_TOTAL = "{*MONEY TOTAL*}";
        private const string ID_REPORT_MONTH = "{*MONTH*}";
        private const string ID_REPORT_YEAR = "{*YEAR*}";
        private const string ID_REPORT_EXPENSES = "{*EXPENSES*}";
        private const string ID_REPORT_INCOME = "{*INCOME*}";

        public void BuildHTML()
        {
            int year = DateTime.Today.Year;
            int month = _month;
            string htmlExpense = BuildHTML_Expense(month);
            string htmlIncome = BuildHTML_Income(month);
            _html = htmlReportTemplate.Replace(ID_REPORT_MONTH, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)).Replace(ID_REPORT_YEAR, year.ToString()).Replace(ID_REPORT_EXPENSES, htmlExpense).Replace(ID_REPORT_INCOME, htmlIncome);
            NotifyPropertyChanged(nameof(this.HTMLSource));
        }

        private string BuildHTML_Expense(int month)
        {
            StringBuilder htmlExpenses = new StringBuilder();
            var listExpenseTypes = _config.CategoryList.Where(x => x.CategoryType == Data.ConfigModels.CategoryType.Expense && !x.ExcludeFromBudget).OrderBy(o => o.Name).ToList();
            decimal monthlyTotalExpense = decimal.Zero;
            if (listExpenseTypes?.Any() == true)
            {
                foreach (var exp in listExpenseTypes)
                {
                    decimal expenseTotal = _ledger.GetCategoryTotal_Monthly(exp, month);
                    monthlyTotalExpense += expenseTotal;
                    string row = htmlRowTemplate.Replace(ID_ROW_DESC, exp.Name).Replace(ID_ROW_TOTAL, expenseTotal.ToString("c"));
                    htmlExpenses.AppendLine(row);
                }
            }
            return htmlChartTemplate.Replace(ID_CHART_DETAIL, htmlExpenses.ToString()).Replace(ID_CHART_TOTAL, monthlyTotalExpense.ToString("c"));
        }

        private string BuildHTML_Income(int month)
        {
            StringBuilder htmlIncome = new StringBuilder();
            var listIncomeTypes = _config.CategoryList.Where(x => x.CategoryType == Data.ConfigModels.CategoryType.Income && !x.ExcludeFromBudget).OrderBy(o => o.Name).ToList();
            decimal monthlyTotalIncome = decimal.Zero;
            if (listIncomeTypes?.Any() == true)
            {
                foreach (var inc in listIncomeTypes)
                {
                    decimal incomeTotal = _ledger.GetCategoryTotal_Monthly(inc, month);
                    monthlyTotalIncome += incomeTotal;
                    string row = htmlRowTemplate.Replace(ID_ROW_DESC, inc.Name).Replace(ID_ROW_TOTAL, incomeTotal.ToString("c"));
                    htmlIncome.AppendLine(row);
                }
            }
            return htmlChartTemplate.Replace(ID_CHART_DETAIL, htmlIncome.ToString()).Replace(ID_CHART_TOTAL, monthlyTotalIncome.ToString("c"));
        }
    }
}