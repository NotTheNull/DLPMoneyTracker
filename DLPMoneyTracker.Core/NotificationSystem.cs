namespace DLPMoneyTracker.Core
{
    /// <summary>
    /// Purpose is to host the events that the use cases will trigger.
    /// MUST BE A SINGLETON
    /// </summary>
    public class NotificationSystem
    {
        public delegate void TransactionsModifiedHandler(Guid debitAccountId, Guid creditAccountId);

        public event TransactionsModifiedHandler? TransactionsModified;

        public delegate void BankDateChangedHandler(Guid moneyAccountUID);

        public event BankDateChangedHandler? BankDateChanged;

        public delegate void BankReconciliationChangedHandler(Guid bankAccountUID);

        public event BankReconciliationChangedHandler? BankReconciliationChanged;

        public delegate void BudgetAmountChangedHandler(Guid accountUID);

        public event BudgetAmountChangedHandler? BudgetAmountChanged;

        public void TriggerTransactionModified(Guid debitAccountUID, Guid creditAccountUID)
        { TransactionsModified?.Invoke(debitAccountUID, creditAccountUID); }

        public void TriggerBankDateChanged(Guid moneyAccountUID)
        { BankDateChanged?.Invoke(moneyAccountUID); }

        public void TriggerBankReconciliationChanged(Guid bankAccountUID)
        { BankReconciliationChanged?.Invoke(bankAccountUID); }

        public void TriggerBudgetAmountChanged(Guid accountUID)
        { BudgetAmountChanged?.Invoke(accountUID); }
    }
}