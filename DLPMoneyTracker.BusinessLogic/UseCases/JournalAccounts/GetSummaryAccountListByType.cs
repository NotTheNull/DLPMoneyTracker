


using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts
{

    public class GetSummaryAccountListByType : IGetSummaryAccountListByType
    {
        private readonly ILedgerAccountRepository accountRepository;

        public GetSummaryAccountListByType(ILedgerAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        private readonly List<LedgerType> VALID_TYPES = new List<LedgerType>() { LedgerType.Payable, LedgerType.Receivable };
        public List<IJournalAccount> Execute(LedgerType accountType)
        {
            if (!VALID_TYPES.Contains(accountType)) return null;

            return accountRepository.GetSummaryAccountListByType(accountType);
        }
    }
}