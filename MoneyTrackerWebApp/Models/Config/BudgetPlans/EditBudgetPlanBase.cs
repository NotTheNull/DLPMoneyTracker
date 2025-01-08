using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Core.Models.ScheduleRecurrence;
using Microsoft.AspNetCore.Components;
using MoneyTrackerWebApp.Components.Pages.Config.MoneyAccounts;
using MoneyTrackerWebApp.Models.Core;
using MoneyTrackerWebApp.Services;

namespace MoneyTrackerWebApp.Models.Config.BudgetPlans
{
    public class EditBudgetPlanBase : ComponentBase
    {
        [Parameter]
        public Guid? PlanUID { get; set; }

        [Inject]
        public IGetBudgetPlanByIdUseCase ActionGetPlan { get; set; }

        [Inject]
        public IGetJournalAccountByUIDUseCase ActionGetAccountByUID { get; set; }

        [Inject]
        public IGetJournalAccountListByTypesUseCase ActionGetAccountListByTypes { get; set; }

        [Inject]
        public ISaveBudgetPlanUseCase ActionSavePlan { get; set; }

        [Inject]
        public StorageService<IBudgetPlan> Storage { get; set; }

        [Inject]
        public INavigationHistoryService Navigation { get; set; }

        [Inject]
        public ILogger<EditBudgetPlanBase> Logger { get; set; }

        [Inject]
        public BudgetPlanFactory PlanFactory { get; set; }

        [Inject]
        public ScheduleRecurrenceFactory RecurrenceFactory { get; set; }


        protected readonly BudgetPlanType[] listPlanTypes = [BudgetPlanType.Payable, BudgetPlanType.Receivable, BudgetPlanType.DebtPayment, BudgetPlanType.Transfer];
        protected readonly RecurrenceFrequency[] listFrequency = [RecurrenceFrequency.Monthly, RecurrenceFrequency.SemiAnnual, RecurrenceFrequency.Annual];
        protected List<IJournalAccount> listDebitAccounts = new List<IJournalAccount>();
        protected List<IJournalAccount> listCreditAccounts = new List<IJournalAccount>();

        protected EditBudgetPlanVM BudgetPlan { get; set; } = new EditBudgetPlanVM();

        private readonly string URL_PLANLIST = "/config/budgetplans";

        protected override void OnParametersSet()
        {
            Logger.LogInformation("Setting parameters");
            if (PlanUID is null || PlanUID == Guid.Empty) return;

            Logger.LogInformation($"Loading plan with UID {PlanUID}");
            var p = ActionGetPlan.Execute(PlanUID.Value);
            Storage.Data = p;
            BudgetPlan.Copy(p);
        }

        protected override void OnInitialized()
        {
            Logger.LogInformation("Initializing");
            if (Storage.Data == null) return;

            Logger.LogInformation("Copying plan from storage");
            BudgetPlan.Copy(Storage.Data);
        }

        #region Form Events
        protected void OnPlanTypeChanged(ChangeEventArgs e)
        {
            BudgetPlan.PlanType = (BudgetPlanType)e.Value;
            this.LoadValidJournalAccounts();
        }

        protected void OnDescriptionChanged(ChangeEventArgs e)
        {
            BudgetPlan.Description = e.Value.ToString();
        }

        protected void OnDebitAccountChanged(ChangeEventArgs e)
        {
            Guid uid = Guid.Parse(e.Value.ToString());
            BudgetPlan.DebitAccount = ActionGetAccountByUID.Execute(uid);
        }

        protected void OnCreditAccountChanged(ChangeEventArgs e)
        {
            Guid uid = Guid.Parse(e.Value.ToString());
            BudgetPlan.CreditAccount = ActionGetAccountByUID.Execute(uid);
        }

        protected void OnExpectedAmountChanged(ChangeEventArgs e)
        {
            if(decimal.TryParse(e.Value.ToString(), out decimal newVal))
            {
                BudgetPlan.ExpectedAmount = newVal;
            }
        }

        protected void OnFrequencyChanged(ChangeEventArgs e)
        {
            BudgetPlan.Frequency = (RecurrenceFrequency)e.Value;
        }

        protected void OnStartDateChanged(ChangeEventArgs e)
        {
            if(DateTime.TryParse(e.Value.ToString(), out DateTime newDate)) {
                BudgetPlan.StartDate = newDate;
            }
        }
        #endregion


        private void LoadValidJournalAccounts()
        {
            listDebitAccounts.Clear();
            var debits = ActionGetAccountListByTypes.Execute(BudgetPlan.ValidDebitAccountTypes);
            if(debits?.Any() == true)
            {
                listDebitAccounts.AddRange(debits);
            }

            listCreditAccounts.Clear();
            var credits = ActionGetAccountListByTypes.Execute(BudgetPlan.ValidCreditAccountTypes);
            if(credits?.Any() == true)
            {
                listCreditAccounts.AddRange(credits);
            }
        }




        protected void SaveChanges()
        {
            BudgetPlan.Recurrence = RecurrenceFactory.Build(BudgetPlan.Frequency, BudgetPlan.StartDate);

            var p = PlanFactory.Build(BudgetPlan);
            ActionSavePlan.Execute(p);
            ReturnToList();
        }

        protected void Reset()
        {
            if(Storage.Data != null)
            {
                BudgetPlan.Copy(Storage.Data);
            } else
            {
                ReturnToList();
            }
        }

        protected void ReturnToList()
        {
            Storage.Data = null;
            Navigation.NavigateBack(URL_PLANLIST);
        }

    }
}
