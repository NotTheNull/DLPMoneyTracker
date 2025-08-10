using AutoMapper;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.Reports.IncomeStatement;

public class IncomeStatementVM(IMapper mapper) : BaseViewModel
{
    
    
	#region Properties

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


	public decimal TotalIncome => IncomeList.Sum(s => s.Total);

	public decimal TotalExpense => ExpenseList.Sum(s => s.Total);

	public decimal TotalProfit => (TotalIncome - TotalExpense);


	public ObservableCollection<IncomeStatementItemVM> IncomeList { get; } = [];
	public ObservableCollection<IncomeStatementItemVM> ExpenseList { get; } = [];

	#endregion


	#region Commands

	public RelayCommand CommandLoad => new((o) => LoadTransactions());


    #endregion
    private readonly TransactionType[] _incomeTypes = [TransactionType.Income];
    private readonly TransactionType[] _expenseTypes = [TransactionType.Expense, TransactionType.DebtPayment];

    
    private void LoadTransactions()
	{	
		// TODO: Finish



		NotifyPropertyChanged(nameof(TotalIncome));
		NotifyPropertyChanged(nameof(TotalExpense));
		NotifyPropertyChanged(nameof(TotalProfit));
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