using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.Reports.IncomeStatement;

public class IncomeStatementVM : BaseViewModel
{


	public readonly List<SpecialDropListItem<int>> MonthList =
    [
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
	];


	private int _year;

	public int SelectedYear
	{
		get { return _year; }
		set 
		{ 
			_year = value;
			NotifyPropertyChanged(nameof(SelectedYear));
		}
	}


	private int _month;

	public int SelectedMonth
	{
		get { return _month; }
		set 
		{
			_month = value;
			NotifyPropertyChanged(nameof(SelectedMonth));
		}
	}



}
