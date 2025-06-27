using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts
{
    public class GetPaymentAccountsUseCase(ILedgerAccountRepository accountRepository) : IGetPaymentAccountsUseCase
    {
        public List<IJournalAccount> Execute(bool includeDeleted = false)
        {
            return accountRepository.GetAccountsBySearch(JournalAccountSearch.GetPaymentAccounts(includeDeleted));
        }
    }
}