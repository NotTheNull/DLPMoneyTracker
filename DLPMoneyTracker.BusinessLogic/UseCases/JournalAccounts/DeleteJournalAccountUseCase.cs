using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts
{
    public class DeleteJournalAccountUseCase(ILedgerAccountRepository accountRepository) : IDeleteJournalAccountUseCase
    {
        public void Execute(Guid accountUID)
        {
            var account = accountRepository.GetAccountByUID(accountUID);
            if (account is null) return;
            if (account.DateClosedUTC.HasValue) return;

            account.DateClosedUTC = DateTime.UtcNow;
            accountRepository.SaveJournalAccount(account);
        }
    }
}