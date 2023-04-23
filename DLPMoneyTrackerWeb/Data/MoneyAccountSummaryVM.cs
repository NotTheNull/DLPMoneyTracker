using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLPMoneyTracker.Data.TransactionModels.BillPlan;

namespace DLPMoneyTrackerWeb.Data
{
    internal class MoneyAccountSummaryVM
    {
        private readonly ITrackerConfig _config;
        private readonly IMoneyPlanner _budget;
        private readonly ILedger _ledger;
        private MoneyAccount _act;

        public MoneyAccountSummaryVM(ITrackerConfig config, IMoneyPlanner budget, ILedger ledger)
        {
            _config = config;
            _budget = budget;
            _ledger = ledger;
        }


        public string AccountID { get { return _act?.ID ?? string.Empty; } }
        public string AccountDesc { get { return _act?.Description ?? string.Empty; } }

        public MoneyAccountType AccountType { get { return _act?.AccountType ?? MoneyAccountType.NotSet; } }

        public decimal CurrentBalance { get; set; }
        public decimal FutureBalance { get; set; }



        private List<IMoneyPlan> _listBudgets = new List<IMoneyPlan>();
        public IReadOnlyList<IMoneyPlan> UpcomingMoneyEvents { get { return _listBudgets.AsReadOnly(); } }




        public void LoadAccount(string actId)
        {
            _act = _config.GetAccount(actId);
            this.Refresh();
        }

        public void Refresh()
        {
            this.CurrentBalance = _ledger.GetAccountBalance(_act);
            this.LoadMoneyEvents();
            this.UpdateFutureBalance();
        }

        private void LoadMoneyEvents()
        {
            _listBudgets.Clear();

            var listMoney = _budget.GetUpcomingMoneyPlansForAccount(this.AccountID);
            if (listMoney?.Any() != true) return;

            foreach(var budget in listMoney.OrderBy(o => o.NotificationDate).ThenBy(o => o.PriorityOrder))
            {
                // Check to see if there's a ledger record that matches; if so, skip the money plan so we're not misled
                if (budget.CategoryName.Contains("Transfer"))
                {
                    // Transfers should be handled especially
                    if (_ledger.TransactionList.Any(x =>
                        (
                            x.CategoryUID == TransactionCategory.TransferTo.ID
                            || x.CategoryUID == TransactionCategory.TransferFrom.ID
                        )
                        && x.AccountID == budget.AccountID
                        && x.TransDate >= budget.NotificationDate))
                    {
                        continue;
                    }
                }
                else if (budget.CategoryName.Contains("Debt"))
                {
                    // Debt payments also have to be handled especially
                    if (_ledger.TransactionList.Any(x => x.CategoryUID == TransactionCategory.DebtPayment.ID && x.AccountID == budget.AccountID && x.TransDate > budget.NotificationDate))
                    {
                        continue;
                    }
                }
                else
                {
                    if (_ledger.TransactionList.Any(x => x.CategoryUID == budget.CategoryID && x.Description == budget.Description && x.TransDate >= budget.NotificationDate))
                    {
                        continue;
                    }
                }

                _listBudgets.Add(budget);
            }
        }

        private void UpdateFutureBalance()
        {
            decimal bal = this.CurrentBalance;
            decimal budgetTotal = _listBudgets.Sum(s => s.ExpectedAmount);
            this.FutureBalance = bal - budgetTotal;

        }
    }
}
