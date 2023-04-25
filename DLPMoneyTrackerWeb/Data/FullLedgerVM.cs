using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.TransactionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTrackerWeb.Data
{
    internal class LedgerFilter
    {


        public string SearchText { get; set; }


        private DateTime _start;

        public DateTime StartDate
        {
            get { return _start; }
            set
            {
                _start = value.Date;
            }
        }



        private DateTime _end;
        public DateTime EndDate
        {
            get { return _end; }
            set
            {
                _end = value.Date.AddDays(1).AddMilliseconds(-1);
            }
        }

        public LedgerFilter()
        {
            StartDate = new DateTime(DateTime.Today.Year, 1, 1, 0, 0, 0);
            EndDate = new DateTime(DateTime.Today.Year, 12, 31).AddDays(1).AddMinutes(-1);
        }
    }

    internal class FullLedgerVM
    {
        private readonly ITrackerConfig _config;
        private readonly ILedger _ledger;

        public FullLedgerVM(ITrackerConfig config, ILedger ledger)
        {
            _config = config;
            _ledger = ledger;
        }

        private List<IMoneyRecord> _listTrans = new List<IMoneyRecord>();
        public IReadOnlyList<IMoneyRecord> Transactions { get { return _listTrans.AsReadOnly(); } }




        public void Refresh(LedgerFilter filter)
        {
            _listTrans.Clear();

            if (_ledger?.TransactionList?.Any() == true)
            {
                var filteredList = _ledger.TransactionList.Where(x => x.TransDate >= filter.StartDate && x.TransDate <= filter.EndDate);

                if (!string.IsNullOrWhiteSpace(filter.SearchText))
                {
                    filteredList = filteredList.Where(x => x.Description.Contains(filter.SearchText));
                }

                if (filteredList?.Any() == true) _listTrans.AddRange(filteredList);
            }
        }


        public TransactionCategory GetCategory(Guid catId)
        {
            return _config.GetCategory(catId);
        }

        public MoneyAccount GetAccount(string uid)
        {
            return _config.GetAccount(uid);
        }
    }
}
