using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using DLPMoneyTracker.Data.TransactionModels;

namespace DLPMoneyTrackerWeb.Data
{
    internal class MoneyAccountDetailVM
    {
        private readonly ITrackerConfig _config;
        private readonly IMoneyPlanner _budget;
        private readonly ILedger _ledger;
        private MoneyAccount _act;

        public MoneyAccountDetailVM(ITrackerConfig config, IMoneyPlanner budget, ILedger ledger)
        {
            _config = config;
            _budget = budget;
            _ledger = ledger;
        }

        public string AccountID { get { return _act?.ID ?? string.Empty; } }
        public string AccountDesc { get { return _act?.Description ?? string.Empty; } }

        public MoneyAccountType AccountType { get { return _act?.AccountType ?? MoneyAccountType.NotSet; } }


        private List<IMoneyRecord> _listTrans = new List<IMoneyRecord>();
        public IReadOnlyList<IMoneyRecord> Transactions { get { return _listTrans.AsReadOnly(); } }


        public void LoadAccount(string actId)
        {
            _act = _config.GetAccount(actId);
            this.Refresh();
        }

        public void Refresh()
        {
            _listTrans.Clear();
            var records = _ledger.TransactionList.Where(x => x.AccountID == this.AccountID).ToList();

            if(records?.Any() == true)
            {
                _listTrans.AddRange(records);
            }
        }


        public string GetCategoryName(Guid catId)
        {
            return _config.GetCategory(catId)?.Name ?? "*MISSING*";
        }

    }
}
