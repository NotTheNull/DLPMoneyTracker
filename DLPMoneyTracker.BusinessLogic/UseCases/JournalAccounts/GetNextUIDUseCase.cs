


using DLPMoneyTracker.BusinessLogic.PluginInterfaces;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts
{
    public class GetNextUIDUseCase : IGetNextUIDUseCase
    {
        private readonly ILedgerAccountRepository accountRepository;

        public GetNextUIDUseCase(ILedgerAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public Guid Execute()
        {
            return accountRepository.GetNextUID();
        }

    }

}