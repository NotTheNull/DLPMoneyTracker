using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts
{
    public class SaveJournalAccountUseCase(ILedgerAccountRepository accountRepository) : ISaveJournalAccountUseCase
    {
        public void Execute(IJournalAccount account)
        {
            if (account is null) return;
            if (account.JournalType == LedgerType.NotSet) throw new InvalidOperationException("Journal type is NOT set");
            if (account.Id == Guid.Empty) throw new InvalidOperationException("GUID is not set");

            accountRepository.SaveJournalAccount(account);
        }
    }
}