using DLPMoneyTracker.BusinessLogic.PluginInterfaces;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts
{
    public class GetNextUIDUseCase(ILedgerAccountRepository accountRepository) : IGetNextUIDUseCase
    {
        public Guid Execute()
        {
            return accountRepository.GetNextUID();
        }
    }
}