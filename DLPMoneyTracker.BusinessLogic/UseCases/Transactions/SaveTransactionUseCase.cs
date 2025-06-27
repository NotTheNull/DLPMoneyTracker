using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions
{
    public class SaveTransactionUseCase(ITransactionRepository moneyRepository, NotificationSystem notifications) : ISaveTransactionUseCase
    {
        public void Execute(IMoneyTransaction transaction)
        {
            if (transaction.DebitAccount == SpecialAccount.InvalidAccount || transaction.CreditAccount == SpecialAccount.InvalidAccount)
                throw new InvalidOperationException("You must use a valid account");

            moneyRepository.SaveTransaction(transaction);
            notifications.TriggerTransactionModified(transaction.DebitAccountId, transaction.CreditAccountId);
        }
    }
}