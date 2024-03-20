using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions
{
    public class SaveTransactionUseCase : ISaveTransactionUseCase
    {
        private readonly ITransactionRepository moneyRepository;
        private readonly NotificationSystem notifications;

        public SaveTransactionUseCase(ITransactionRepository moneyRepository, NotificationSystem notifications)
        {
            this.moneyRepository = moneyRepository;
            this.notifications = notifications;
        }

        public void Execute(IMoneyTransaction transaction)
        {
            moneyRepository.SaveTransaction(transaction);
            notifications.TriggerTransactionModified(transaction.DebitAccountId, transaction.CreditAccountId);
        }
    }
}
