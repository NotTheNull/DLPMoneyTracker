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
    public class GetJournalAccountListByTypesUseCase : IGetJournalAccountListByTypesUseCase
    {
        private readonly ILedgerAccountRepository accountRepository;

        public GetJournalAccountListByTypesUseCase(ILedgerAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }


        public List<IJournalAccount> Execute(List<LedgerType> listTypes)
        {
            ArgumentNullException.ThrowIfNull(listTypes);

            return accountRepository.GetAccountsBySearch(JournalAccountSearch.GetAccountsByType(listTypes));
        }

    }
}
