using DLPMoneyTracker.BusinessLogic.AdapterInterfaces;
using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Core.Models.ScheduleRecurrence;
using DLPMoneyTracker.Plugins.SQL.Data;

namespace DLPMoneyTracker.Plugins.SQL.Adapters
{
    public class SQLSourceToBudgetPlanAdapter(DataContext context, ILedgerAccountRepository accountRepository) : ISourceToBudgetPlanAdapter<BudgetPlan>
    {
        private readonly DataContext context = context;
        private readonly ILedgerAccountRepository accountRepository = accountRepository;

        public Guid UID { get; set; }

        public BudgetPlanType PlanType { get; set; }

        public string Description { get; set; } = string.Empty;

        public decimal ExpectedAmount { get; set; }

        public List<LedgerType> ValidDebitAccountTypes => throw new NotImplementedException();

        public IJournalAccount DebitAccount { get; set; } = SpecialAccount.InvalidAccount;

        public Guid DebitAccountId => this.DebitAccount?.Id ?? Guid.Empty;

        public string DebitAccountName => this.DebitAccount?.Description ?? string.Empty;

        public List<LedgerType> ValidCreditAccountTypes => throw new NotImplementedException();

        public IJournalAccount CreditAccount { get; set; } = SpecialAccount.InvalidAccount;

        public Guid CreditAccountId => this.CreditAccount?.Id ?? Guid.Empty;

        public string CreditAccountName => this.CreditAccount?.Description ?? string.Empty;

        public IScheduleRecurrence Recurrence { get; set; } = ScheduleRecurrenceFactory.Default();

        public DateTime NotificationDate => throw new NotImplementedException();

        public DateTime NextOccurrence => throw new NotImplementedException();

        public void Copy(IBudgetPlan plan)
        {
            ArgumentNullException.ThrowIfNull(plan);

            this.UID = plan.UID;
            this.PlanType = plan.PlanType;
            this.Description = plan.Description;
            this.DebitAccount = plan.DebitAccount;
            this.CreditAccount = plan.CreditAccount;
            this.ExpectedAmount = plan.ExpectedAmount;
            this.Recurrence = plan.Recurrence;
        }

        public void ExportSource(ref BudgetPlan plan)
        {
            ArgumentNullException.ThrowIfNull(plan);

            plan.PlanUID = this.UID;
            plan.PlanType = this.PlanType;
            plan.Description = this.Description;
            plan.ExpectedAmount = this.ExpectedAmount;
            plan.Frequency = this.Recurrence.Frequency;
            plan.StartDate = this.Recurrence.StartDate;

            plan.Debit = context.Accounts.FirstOrDefault(x => x.AccountUID == this.DebitAccountId);
            plan.Credit = context.Accounts.FirstOrDefault(x => x.AccountUID == this.CreditAccountId);
        }

        public void ImportSource(BudgetPlan plan)
        {
            ArgumentNullException.ThrowIfNull(plan);

            this.UID = plan.PlanUID;
            this.PlanType = plan.PlanType;
            this.Description = plan.Description;
            this.ExpectedAmount = plan.ExpectedAmount;
            this.Recurrence = ScheduleRecurrenceFactory.Build(plan.Frequency, plan.StartDate);

            this.DebitAccount = accountRepository.GetAccountByUID(plan.Debit?.AccountUID ?? Guid.Empty);
            this.CreditAccount = accountRepository.GetAccountByUID(plan.Credit?.AccountUID ?? Guid.Empty);
        }

        public bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}