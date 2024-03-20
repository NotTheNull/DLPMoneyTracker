

using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.Core;
using DLPMoneyTracker2.Main.TransactionList;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Windows.Controls;

namespace DLPMoneyTracker2.Main.YTD
{
    public class YTDAccountDetailVM : BaseViewModel
    {
        private readonly IGetJournalAccountYTDUseCase getYTDUseCase;
        private readonly IGetJournalAccountBalanceByMonthUseCase getMTDUseCase;
        private int _year; // Here in case we use to display history
        private IJournalAccount _account;

        public YTDAccountDetailVM(
            IGetJournalAccountYTDUseCase getYTDUseCase,
            IGetJournalAccountBalanceByMonthUseCase getMTDUseCase)
        {
            this.getYTDUseCase = getYTDUseCase;
            this.getMTDUseCase = getMTDUseCase;
        }

        public void LoadData(IJournalAccount account, int year)
        {
            _year = year;
            _account = account;

            this.YearTotal = getYTDUseCase.Execute(account.Id, year);
            this.JanuaryTotal = getMTDUseCase.Execute(account.Id, year, 1);
            this.FebruaryTotal = getMTDUseCase.Execute(account.Id, year, 2);
            this.MarchTotal = getMTDUseCase.Execute(account.Id, year, 3);
            this.AprilTotal = getMTDUseCase.Execute(account.Id, year, 4);
            this.MayTotal = getMTDUseCase.Execute(account.Id, year, 5);
            this.JuneTotal = getMTDUseCase.Execute(account.Id, year, 6);
            this.JulyTotal = getMTDUseCase.Execute(account.Id, year, 7);
            this.AugustTotal = getMTDUseCase.Execute(account.Id, year, 8);
            this.SeptemberTotal = getMTDUseCase.Execute(account.Id, year, 9);
            this.OctoberTotal = getMTDUseCase.Execute(account.Id, year, 10);
            this.NovemberTotal = getMTDUseCase.Execute(account.Id, year, 11);
            this.DecemberTotal = getMTDUseCase.Execute(account.Id, year, 12);

            this.NotifyAll();
        }

        private void NotifyAll()
        {
            NotifyPropertyChanged(nameof(this.AccountName));
            NotifyPropertyChanged(nameof(this.YearTotal));
            NotifyPropertyChanged(nameof(this.JanuaryTotal));
            NotifyPropertyChanged(nameof(this.FebruaryTotal));
            NotifyPropertyChanged(nameof(this.MarchTotal));
            NotifyPropertyChanged(nameof(this.AprilTotal));
            NotifyPropertyChanged(nameof(this.MayTotal));
            NotifyPropertyChanged(nameof(this.JuneTotal));
            NotifyPropertyChanged(nameof(this.JulyTotal));
            NotifyPropertyChanged(nameof(this.AugustTotal));
            NotifyPropertyChanged(nameof(this.SeptemberTotal));
            NotifyPropertyChanged(nameof(this.OctoberTotal));
            NotifyPropertyChanged(nameof(this.NovemberTotal));
            NotifyPropertyChanged(nameof(this.DecemberTotal));
        }

        public string AccountName { get { return _account.Description; } }
        public decimal YearTotal { get; set; }
        public decimal JanuaryTotal { get; set; }
        public decimal FebruaryTotal { get; set; }
        public decimal MarchTotal { get; set; }
        public decimal AprilTotal { get; set; }
        public decimal MayTotal { get; set; }
        public decimal JuneTotal { get; set; }
        public decimal JulyTotal { get; set; }
        public decimal AugustTotal { get; set; }
        public decimal SeptemberTotal { get; set; }
        public decimal OctoberTotal { get; set; }
        public decimal NovemberTotal { get; set; }
        public decimal DecemberTotal { get; set; }

        #region Commands

        private RelayCommand _cmdTransactions;


        public RelayCommand CommandShowDetail
        {
            get
            {
                return _cmdTransactions ?? (_cmdTransactions = new RelayCommand((o) =>
                {
                    DateTime start = new DateTime(DateTime.Today.Year, 1, 1);
                    DateTime end = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
                    TransDetailFilter filter = new TransDetailFilter()
                    {
                        Account = _account,
                        FilterDates = new DateRange(start, end),
                        AreFilterControlsVisible = false
                    };
                    AccountTransactionDetail window = new AccountTransactionDetail(filter);
                    window.Show();
                }));
            }
        }

        #endregion Commands
    }
}