using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts
{
    public class GetNominalAccountsUseCase(ILedgerAccountRepository accountRepository) : IGetNominalAccountsUseCase
    {
        public List<IJournalAccount> Execute(bool includeDeleted)
        {
            return accountRepository.GetAccountsBySearch(JournalAccountSearch.GetNominalAccounts(includeDeleted));
        }
    }
}