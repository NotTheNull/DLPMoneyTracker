using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts
{
    public class GetPaymentAccountsUseCase : IGetPaymentAccountsUseCase
    {
        private readonly ILedgerAccountRepository accountRepository;

        public GetPaymentAccountsUseCase(ILedgerAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }


        public List<IJournalAccount> Execute(bool includeDeleted = false)
        {
            return accountRepository.GetAccountsBySearch(JournalAccountSearch.GetPaymentAccounts(includeDeleted));
        }
    }
}
