using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.Common;
using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.TransactionModels.BillPlan;
using DLPMoneyTracker.ReportViews.LedgerViews;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace DLPMoneyTracker.ReportViews.HistoricalViews
{
    public class MonthlyHistoricalRecord : BaseViewModel
    {
        private readonly ITrackerConfig _config;
        private readonly ILedger _ledger;
        private int _month, _year;

        public MonthlyHistoricalRecord(ITrackerConfig config, ILedger ledger)
        {
            _config = config;
            _ledger = ledger;
        }

        public Guid UID { get { return this.Category?.ID ?? Guid.Empty; } }

        private TransactionCategory _cat;

        public TransactionCategory Category
        {
            get { return _cat; }
        }

        public string CategoryName { get { return this.Category?.Name ?? string.Empty; } }

        private decimal _total;

        public decimal MonthlyTotal
        {
            get { return _total; }
        }



        #region Commands
        private RelayCommand _cmdShowDetail;
        public RelayCommand CommandShowDetail
        {
            get
            {
                return _cmdShowDetail ?? (_cmdShowDetail = new RelayCommand((o) =>
                {
                    DateRange monthRange = new DateRange(_year, _month);

                    LedgerDetailView uiAccountLedger = new LedgerDetailView(_ledger, _config);
                    uiAccountLedger.ShowCategoryDetail(this.Category, monthRange);
                    Window windowLedger = new Window()
                    {
                        Content = uiAccountLedger,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        Title = "Ledger Detail",
                        Width = 750,
                        Height = 500
                    };
                    windowLedger.Show();
                }));
            }
        }
        #endregion


        public void LoadCategory(TransactionCategory cat, int month, int year)
        {
            _cat = cat;
            _month = month;
            _year = year;
            _total = _ledger.GetCategoryTotal_Monthly(cat, year, month);
        }

        private void NotifyAll()
        {
            NotifyPropertyChanged(nameof(this.Category));
            NotifyPropertyChanged(nameof(this.CategoryName));
            NotifyPropertyChanged(nameof(this.MonthlyTotal));
        }
    }
}
