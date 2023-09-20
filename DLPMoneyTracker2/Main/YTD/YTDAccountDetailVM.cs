using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker2.Core;
using DLPMoneyTracker2.Main.TransactionList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.Main.YTD
{
    public class YTDAccountDetailVM : BaseViewModel
    {
        private readonly int _year; // Here in case we use to display history
        private readonly ITrackerConfig _config;
        private readonly IJournal _journal;
        private readonly IJournalAccount _account;

        public YTDAccountDetailVM(int year, IJournalAccount account, ITrackerConfig config, IJournal journal)
        {
            _year = year;
            _account = account;
            _config = config;
            _journal = journal;
        }


        public string AccountName { get { return _account.Description; } }
        public decimal YearTotal { get { return _journal.GetAccountBalance(_account.Id, false); } }
        public decimal JanuaryTotal { get { return _journal.GetAccountBalance_Month(_account.Id, false, _year, 1); } }
        public decimal FebruaryTotal { get { return _journal.GetAccountBalance_Month(_account.Id, false, _year, 2); } }
        public decimal MarchTotal { get { return _journal.GetAccountBalance_Month(_account.Id, false, _year, 3); } }
        public decimal AprilTotal { get { return _journal.GetAccountBalance_Month(_account.Id, false, _year, 4); } }
        public decimal MayTotal { get { return _journal.GetAccountBalance_Month(_account.Id, false, _year, 5); } }
        public decimal JuneTotal { get { return _journal.GetAccountBalance_Month(_account.Id, false, _year, 6); } }
        public decimal JulyTotal { get { return _journal.GetAccountBalance_Month(_account.Id, false, _year, 7); } }
        public decimal AugustTotal { get { return _journal.GetAccountBalance_Month(_account.Id, false, _year, 8); } }
        public decimal SeptemberTotal { get { return _journal.GetAccountBalance_Month(_account.Id, false, _year, 9); } }
        public decimal OctoberTotal { get { return _journal.GetAccountBalance_Month(_account.Id, false, _year, 10); } }
        public decimal NovemberTotal { get { return _journal.GetAccountBalance_Month(_account.Id, false, _year, 11); } }
        public decimal DecemberTotal { get { return _journal.GetAccountBalance_Month(_account.Id, false, _year, 12); } }


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
                        FilterDates = new DLPMoneyTracker.Data.Common.DateRange(start, end),
                        AreFilterControlsVisible = false
                    };
                    AccountTransactionDetail window = new AccountTransactionDetail(filter);
                    window.Show();

                }));
            }
        }
        #endregion


    }
}
