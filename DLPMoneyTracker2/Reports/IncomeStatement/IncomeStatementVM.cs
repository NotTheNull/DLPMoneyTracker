using AutoMapper;
using DLPMoneyTracker.BusinessLogic.UseCases.Reports.Interfaces;
using DLPMoneyTracker.Core.ReportDTOs;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace DLPMoneyTracker2.Reports.IncomeStatement;

public class IncomeStatementVM(IMapper mapper, IGetIncomeStatementDataUseCase actionGetIncomeStatement) : BaseViewModel
{    
    
	#region Properties

	private readonly List<SpecialDropListItem<int>> _listMonths =
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
	public List<SpecialDropListItem<int>> MonthsList { get { return _listMonths; } }


	private int _year = DateTime.Today.Year;

	public int SelectedYear
	{
		get { return _year; }
		set
		{
			_year = value;
			NotifyPropertyChanged(nameof(SelectedYear));
		}
	}


	private int _month = DateTime.Today.Month;

	public int SelectedMonth
	{
		get { return _month; }
		set
		{
			_month = value;
			NotifyPropertyChanged(nameof(SelectedMonth));
		}
	}


	public decimal TotalIncome => IncomeList.Sum(s => s.Total) * -1;

	public decimal TotalExpense => ExpenseList.Sum(s => s.Total);

	public decimal TotalProfit => (TotalIncome - TotalExpense);

	public SolidColorBrush ProfitColor => new(TotalProfit < decimal.Zero ? Colors.Red : Colors.Black);


	public ObservableCollection<IncomeStatementItemVM> IncomeList { get; } = [];
	public ObservableCollection<IncomeStatementItemVM> ExpenseList { get; } = [];

	#endregion


	#region Commands

	public RelayCommand CommandLoad => new((o) => LoadTransactions());


    #endregion
    

    
    private void LoadTransactions()
	{
		var results = actionGetIncomeStatement.Execute(new() { Year = this.SelectedYear, Month = this.SelectedMonth });

		this.FillList(this.IncomeList, results.IncomeList);
		this.FillList(this.ExpenseList, results.ExpenseList);
		
        NotifyPropertyChanged(nameof(TotalIncome));
		NotifyPropertyChanged(nameof(TotalExpense));
		NotifyPropertyChanged(nameof(TotalProfit));
		NotifyPropertyChanged(nameof(ProfitColor));
	}

	private void FillList(Collection<IncomeStatementItemVM> fillThisList, IEnumerable<IncomeStatementItemDTO> dtos)
	{
		fillThisList.Clear();
		foreach(var d in dtos)
		{
			var vm = mapper.Map<IncomeStatementItemVM>(d);
			if (vm is null) continue;

			fillThisList.Add(vm);
		}
	}
}

public class IncomeStatementItemVM : BaseViewModel
{
	private string _name = string.Empty;
	public string AccountName
	{
		get { return _name; }
		set 
		{
			_name = value;
			NotifyPropertyChanged(nameof(AccountName));
		}
	}

	private decimal _total;

	public decimal Total
	{
		get { return _total; }
		set 
		{ 
			_total = value;
			NotifyPropertyChanged(nameof(Total));
		}
	}

}