using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts
{
    public class GetJournalAccountByUIDUseCase(ILedgerAccountRepository accountRepository) : IGetJournalAccountByUIDUseCase
    {
        public IJournalAccount Execute(Guid uid)
        {
            return accountRepository.GetAccountByUID(uid);
        }
    }
}