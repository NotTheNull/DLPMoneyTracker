using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans
{
    public class GetBudgetTransactionBalanceForAccountUseCase(ITransactionRepository moneyRepository, ILedgerAccountRepository accountRepository) : IGetBudgetTransactionBalanceForAccountUseCase
    {
        private readonly ITransactionRepository moneyRepository = moneyRepository;
        private readonly ILedgerAccountRepository accountRepository = accountRepository;
        private readonly List<LedgerType> listBudgetAccountTypes = [LedgerType.LiabilityLoan, LedgerType.Payable, LedgerType.Receivable];

        /// <summary>
        /// This use case is intended for those accounts that are displayed on the Budget Analysis screen.
        /// It needs to handle Loan accounts differently
        /// </summary>
        /// <param name="accountUID"></param>
        /// <returns></returns>
        public decimal Execute(Guid accountUID)
        {
            var account = accountRepository.GetAccountByUID(accountUID);

            // If the account is associated with a Money Account or a Special Account then we need to return 0 as we don't Budget those accounts
            if (!listBudgetAccountTypes.Contains(account.JournalType)) return decimal.Zero;

            if (account.JournalType == LedgerType.LiabilityLoan)
            {
                MoneyRecordSearch search = new()
                {
                    Account = account,
                    DateRange = new Core.DateRange(DateTime.Today.Year, DateTime.Today.Month)
                };

                var records = moneyRepository.Search(search);
                if (records?.Any() != true) return decimal.Zero;

                decimal total = decimal.Zero;
                foreach (var money in records)
                {
                    // For loans, ignore Interest accruing and other adjustments as they are not part of the Monthly Budget
                    if (money.CreditAccount.JournalType == LedgerType.NotSet || money.DebitAccount.JournalType == LedgerType.NotSet) continue;

                    if (money.DebitAccountId == accountUID)
                    {
                        total += money.TransactionAmount;
                    }
                    else if (money.CreditAccountId == accountUID)
                    {
                        total -= money.TransactionAmount;
                    }
                }

                return total;
            }
            else
            {
                return moneyRepository.GetCurrentAccountBalance(accountUID);
            }
        }
    }
}