using DLPMoneyTracker.Core;
using DLPMoneyTracker2.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.Reports
{
    public class BudgetAnalysisParametersVM : BaseViewModel
    {




		private List<SpecialDropListItem<int>> _listMonths = new List<SpecialDropListItem<int>>()
		{
			new SpecialDropListItem<int>("January", 1),
			new SpecialDropListItem<int>("February", 2),
			new SpecialDropListItem<int>("March", 3),
			new SpecialDropListItem<int>("April", 4),
			new SpecialDropListItem<int>("May", 5),
			new SpecialDropListItem<int>("June", 6),
			new SpecialDropListItem<int>("July", 7),
			new SpecialDropListItem<int>("August", 8),
			new SpecialDropListItem<int>("September", 9),
			new SpecialDropListItem<int>("October", 10),
			new SpecialDropListItem<int>("November", 11),
			new SpecialDropListItem<int>("December", 12)
		};
		public List<SpecialDropListItem<int>> Months { get { return _listMonths; } }



		private int _year;

		public int SelectedYear
		{
			get { return _year; }
			set { _year = value; }
		}


		private int _month;

		public int SelectedMonth
		{
			get { return _month; }
			set { _month = value; }
		}



		private RelayCommand _cmdPrint;
		public RelayCommand CommandPrint
		{
			get
			{
				return _cmdPrint ?? (_cmdPrint = new RelayCommand((o) =>
				{
					DateRange range = new DateRange(this.SelectedYear, this.SelectedMonth);
					ReportBudgetAnalysis report = UICore.DependencyHost.GetRequiredService<ReportBudgetAnalysis>();
					report.Print(range);
				}));
			}
		}



	}
}
