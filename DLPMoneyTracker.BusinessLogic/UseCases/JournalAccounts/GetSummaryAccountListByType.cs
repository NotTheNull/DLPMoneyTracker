using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts
{
    public class GetSummaryAccountListByType(ILedgerAccountRepository accountRepository) : IGetSummaryAccountListByType
    {
        private readonly List<LedgerType> VALID_TYPES = [LedgerType.Payable, LedgerType.Receivable];

        public List<IJournalAccount> Execute(LedgerType accountType)
        {
            if (!VALID_TYPES.Contains(accountType)) return [];

            return accountRepository.GetSummaryAccountListByType(accountType);
        }
    }
}