using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans
{
    public class GetBudgetTransactionBalanceForAccountUseCase : IGetBudgetTransactionBalanceForAccountUseCase
    {
        private readonly ITransactionRepository moneyRepository;
        private readonly ILedgerAccountRepository accountRepository;
        private readonly List<LedgerType> listBudgetAccountTypes = new List<LedgerType>() { LedgerType.LiabilityLoan, LedgerType.Payable, LedgerType.Receivable };

        public GetBudgetTransactionBalanceForAccountUseCase(ITransactionRepository moneyRepository, ILedgerAccountRepository accountRepository)
        {
            this.moneyRepository = moneyRepository;
            this.accountRepository = accountRepository;
        }




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
                MoneyRecordSearch search = new MoneyRecordSearch()
                {
                    Account = account,
                    DateRange = new Core.DateRange(DateTime.Today.Year, DateTime.Today.Month)
                };

                var records = moneyRepository.Search(search);
                if (records?.Any() != true) return decimal.Zero;

                decimal total = decimal.Zero;
                foreach (var money in records)
                {
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
