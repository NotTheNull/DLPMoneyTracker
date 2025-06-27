using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts
{
    public class GetMoneyAccountsUseCase(ILedgerAccountRepository accountRepository) : IGetMoneyAccountsUseCase
    {
        public List<IJournalAccount> Execute(bool includeDeleted)
        {
            return accountRepository.GetAccountsBySearch(JournalAccountSearch.GetMoneyAccounts(includeDeleted));
        }
    }
}