using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTrackerWeb.Data
{
    internal class EditMoneyAccountService
    {
        private readonly ITrackerConfig _config;
        public EditMoneyAccountService(ITrackerConfig config)
        {
            _config = config;
        }

        public IReadOnlyList<MoneyAccount> MoneyAccounts { get { return _config.AccountsList; } }

        public MoneyAccount GetAccount(string id)
        {
            return this.MoneyAccounts.FirstOrDefault(x => x.ID == id);
        }

        public void SaveAccount(MoneyAccount account)
        {
            if(!MoneyAccounts.Contains(account))
            {
                _config.AddMoneyAccount(account);
            }
            _config.SaveMoneyAccounts();
        }

        public void ReloadAccounts()
        {
            _config.LoadMoneyAccounts();
        }

        public void DeleteAccount(string id) 
        {
            var account = this.GetAccount(id);
            if (account is null) return;

            _config.RemoveMoneyAccount(account);
        }
    }
}
