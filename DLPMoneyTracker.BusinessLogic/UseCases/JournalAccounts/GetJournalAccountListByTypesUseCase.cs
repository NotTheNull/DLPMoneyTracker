using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts
{
    public class GetJournalAccountListByTypesUseCase(ILedgerAccountRepository accountRepository) : IGetJournalAccountListByTypesUseCase
    {
        public List<IJournalAccount> Execute(List<LedgerType> listTypes)
        {
            ArgumentNullException.ThrowIfNull(listTypes);

            return accountRepository.GetAccountsBySearch(JournalAccountSearch.GetAccountsByType(listTypes));
        }
    }
}