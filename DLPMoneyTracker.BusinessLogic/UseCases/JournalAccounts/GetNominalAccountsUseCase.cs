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
    public class GetNominalAccountsUseCase : IGetNominalAccountsUseCase
    {
        private readonly ILedgerAccountRepository accountRepository;

        public GetNominalAccountsUseCase(ILedgerAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public List<IJournalAccount> Execute(bool includeDeleted)
        {
            return accountRepository.GetAccountsBySearch(JournalAccountSearch.GetNominalAccounts(includeDeleted));
        }
    }
}
