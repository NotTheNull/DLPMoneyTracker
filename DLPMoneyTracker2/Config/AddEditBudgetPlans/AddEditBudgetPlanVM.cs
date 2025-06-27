
using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Core.Models.ScheduleRecurrence;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DLPMoneyTracker2.Config.AddEditBudgetPlans
{
    public class AddEditBudgetPlanVM : BaseViewModel
    {

        private readonly IGetBudgetPlanListUseCase getPlanListUseCase;
        private readonly IGetJournalAccountByUIDUseCase getAccountByUIDUseCase;
        private readonly IGetJournalAccountListByTypesUseCase getAccountsByTypesUseCase;
        private readonly IDeleteBudgetPlanUseCase deletePlanUseCase;
        private readonly ISaveBudgetPlanUseCase savePlanUseCase;

        public AddEditBudgetPlanVM(
            IGetBudgetPlanListUseCase getPlanListUseCase, 
            IGetJournalAccountByUIDUseCase getAccountByUIDUseCase,
            IGetJournalAccountListByTypesUseCase getAccountsByTypesUseCase,
            IDeleteBudgetPlanUseCase deletePlanUseCase,
            ISaveBudgetPlanUseCase savePlanUseCase) : base()
        {
            this.getPlanListUseCase = getPlanListUseCase;
            this.getAccountByUIDUseCase = getAccountByUIDUseCase;
            this.getAccountsByTypesUseCase = getAccountsByTypesUseCase;
            this.deletePlanUseCase = deletePlanUseCase;
            this.savePlanUseCase = savePlanUseCase;

            this.Reload();
        }

        private readonly ObservableCollection<IBudgetPlan> _listPlans = [];
        public ObservableCollection<IBudgetPlan> PlanList => _listPlans;

        public Guid BudgetPlanId { get; set; }

        private readonly List<SpecialDropListItem<BudgetPlanType>> _listPlanTypes =
        [
            new SpecialDropListItem<BudgetPlanType>("Income", BudgetPlanType.Receivable),
            new SpecialDropListItem<BudgetPlanType>("Expense", BudgetPlanType.Payable),
            new SpecialDropListItem<BudgetPlanType>("Debt Payment", BudgetPlanType.DebtPayment),
            new SpecialDropListItem<BudgetPlanType>("Transfer", BudgetPlanType.Transfer)
        ];

        public List<SpecialDropListItem<BudgetPlanType>> PlanTypes => _listPlanTypes;

        private BudgetPlanType _planType;

        public BudgetPlanType SelectedPlanType
        {
            get { return _planType; }
            set
            {
                _planType = value;
                this.LoadValidJournalAccounts();
                NotifyPropertyChanged(nameof(SelectedPlanType));
            }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                NotifyPropertyChanged(nameof(Description));
            }
        }

        private readonly ObservableCollection<SpecialDropListItem<IJournalAccount>> _listValidDebits = [];
        public ObservableCollection<SpecialDropListItem<IJournalAccount>> ValidDebitAccounts => _listValidDebits;

        
        private IJournalAccount? _debit = null;
        public IJournalAccount? SelectedDebitAccount
        {
            get { return _debit; }
            set
            {
                _debit = value;
                NotifyPropertyChanged(nameof(SelectedDebitAccount));
            }
        }

        private readonly ObservableCollection<SpecialDropListItem<IJournalAccount>> _listValidCredits = [];
        public ObservableCollection<SpecialDropListItem<IJournalAccount>> ValidCreditAccounts => _listValidCredits;

        private IJournalAccount? _credit = null;
        public IJournalAccount? SelectedCreditAccount
        {
            get { return _credit; }
            set
            {
                _credit = value;
                NotifyPropertyChanged(nameof(SelectedCreditAccount));
            }
        }

        private decimal _amount = decimal.Zero;
        public decimal Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                NotifyPropertyChanged(nameof(Amount));
            }
        }

        private IScheduleRecurrence? _recurrence;
        public IScheduleRecurrence? Recurrence
        {
            get { return _recurrence; }
            set
            {
                _recurrence = value;
                NotifyPropertyChanged(nameof(Recurrence));
            }
        }

        private bool IsReadyForSave
        {
            get
            {
                if (this.SelectedCreditAccount is null) return false;
                if (this.SelectedDebitAccount is null) return false;
                if (this.SelectedPlanType == BudgetPlanType.NotSet) return false;
                if (string.IsNullOrWhiteSpace(this.Description)) return false;
                if (this.Amount == decimal.Zero) return false;
                if (this.Recurrence is null) return false;

                return true;
            }
        }

        #region Commands

        public RelayCommand CommandSaveChanges =>
            new((o) =>
            {
                if (this.IsReadyForSave)
                {
#pragma warning disable CS8604 // Possible null reference argument: It's being checked in "IsReadyForSave"; too bad the compiler can't see it
                    savePlanUseCase.Execute(BudgetPlanFactory.Build(this.SelectedPlanType, this.BudgetPlanId, this.Description, this.SelectedDebitAccount, this.SelectedCreditAccount, this.Amount, this.Recurrence));
#pragma warning restore CS8604 // Possible null reference argument.
                }

                this.Clear();
                this.Reload();
            });

        public RelayCommand CommandAddNew => 
            new((o) =>
            {
                this.Clear();
            });

        public RelayCommand CommandEditRecord => 
            new((plan) =>
            {
                if (plan is IBudgetPlan journalPlan)
                {
                    this.LoadJournalPlan(journalPlan);
                }
            });

        public RelayCommand CommandDeleteRecord => 
            new((plan) =>
            {
                if (plan is IBudgetPlan journalPlan)
                {
                    deletePlanUseCase.Execute(journalPlan.UID);

                    this.Clear();
                    this.Reload();
                }
            });

        #endregion Commands

        private void Reload()
        {
            this.PlanList.Clear();
            var listFull = getPlanListUseCase.Execute();
            if (listFull is null) return;
            if (listFull.Count == 0) return;

            foreach (var p in listFull.OrderBy(o => o.Description))
            {
                this.PlanList.Add(p);
            }
        }

        private void Clear()
        {
            this.BudgetPlanId = Guid.Empty;
            this.Amount = decimal.Zero;
            this.Description = string.Empty;
            this.SelectedCreditAccount = null;
            this.SelectedDebitAccount = null;
            this.SelectedPlanType = BudgetPlanType.Payable;
            this.Recurrence = ScheduleRecurrenceFactory.Build(RecurrenceFrequency.Annual, DateTime.Today);
        }

        /// <summary>
        /// Fills the Valid Account Lists depending on the Selected Plan Type
        /// </summary>
        private void LoadValidJournalAccounts()
        {
            IBudgetPlan plan;
            this.ValidCreditAccounts.Clear();
            this.ValidDebitAccounts.Clear();

            switch (this.SelectedPlanType)
            {
                case BudgetPlanType.Receivable:
                    plan = new ReceivablePlan();
                    break;

                case BudgetPlanType.Payable:
                    plan = new PayablePlan();
                    break;

                case BudgetPlanType.Transfer:
                    plan = new TransferPlan();
                    break;

                case BudgetPlanType.DebtPayment:
                    plan = new DebtPaymentPlan();
                    break;

                default:
                    return;
            }

            var listCreditAccounts = getAccountsByTypesUseCase.Execute(plan.ValidCreditAccountTypes);
            foreach (var act in listCreditAccounts.OrderBy(o => o.Description))
            {
                this.ValidCreditAccounts.Add(new SpecialDropListItem<IJournalAccount>(act.Description, act));
            }

            var listDebitAccounts = getAccountsByTypesUseCase.Execute(plan.ValidDebitAccountTypes);
            foreach (var act in listDebitAccounts.OrderBy(o => o.Description))
            {
                this.ValidDebitAccounts.Add(new SpecialDropListItem<IJournalAccount>(act.Description, act));
            }
        }

        private void LoadJournalPlan(IBudgetPlan plan)
        {
            this.BudgetPlanId = plan.UID;
            this.SelectedPlanType = plan.PlanType;
            this.SelectedCreditAccount = getAccountByUIDUseCase.Execute(plan.CreditAccountId);
            this.SelectedDebitAccount = getAccountByUIDUseCase.Execute(plan.DebitAccountId);
            this.Description = plan.Description;
            this.Amount = plan.ExpectedAmount;
            this.Recurrence = plan.Recurrence;
            
        }
    }
}