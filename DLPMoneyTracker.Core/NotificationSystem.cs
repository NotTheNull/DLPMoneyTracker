using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core
{
    /// <summary>
    /// Purpose is to host the events that the use cases will trigger.
    /// MUST BE A SINGLETON
    /// </summary>
    public delegate void SendMessageHandler(object msg);
    public class NotificationSystem : IDisposable
    {
        public NotificationSystem() { }
        ~NotificationSystem()
        {
            this.Dispose();
        }
    
        public delegate void TransactionsModifiedHandler(Guid debitAccountId, Guid creditAccountId);
        public event TransactionsModifiedHandler TransactionsModified;

        public delegate void BankDateChangedHandler(Guid moneyAccountUID);
        public event BankDateChangedHandler BankDateChanged;

        public delegate void BankReconciliationChangedHandler(Guid bankAccountUID);
        public event BankReconciliationChangedHandler BankReconciliationChanged;

        public delegate void BudgetAmountChangedHandler(Guid accountUID);
        public event BudgetAmountChangedHandler BudgetAmountChanged;

        
        // This will allow me to custom design ANY event
        public Dictionary<string, SendMessageHandler> MessageEvents { get; set; } = new Dictionary<string, SendMessageHandler>();


        public void TriggerTransactionModified(Guid debitAccountUID, Guid creditAccountUID) { TransactionsModified?.Invoke(debitAccountUID, creditAccountUID); }
        public void TriggerBankDateChanged(Guid moneyAccountUID) { BankDateChanged?.Invoke(moneyAccountUID); }
        public void TriggerBankReconciliationChanged(Guid bankAccountUID) { BankReconciliationChanged?.Invoke(bankAccountUID); }
        public void TriggerBudgetAmountChanged(Guid accountUID) { BudgetAmountChanged?.Invoke(accountUID); }


        public void SendMessage(string messageName, object messageContent)
        {
            //if (!MessageEvents.ContainsKey(messageName)) throw new InvalidMessageException($"Message [{messageName}] not found");

            MessageEvents[messageName]?.Invoke(messageContent);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if(MessageEvents?.Any() == true)
            {
                MessageEvents.Clear();
            }
            MessageEvents = null;
        }
    }

    public class InvalidMessageException : System.Exception
    {
        public InvalidMessageException() : base() { }

        public InvalidMessageException(string msg) : base(msg) { }

        public InvalidMessageException(string msg, Exception inner) : base(msg, inner) { }
    }

}
