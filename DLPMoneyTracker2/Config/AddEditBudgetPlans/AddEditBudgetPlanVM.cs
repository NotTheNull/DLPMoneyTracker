
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
        private readonly ScheduleRecurrenceFactory recurrenceFactory;
        private readonly BudgetPlanFactory budgetFactory;

        public AddEditBudgetPlanVM(
            IGetBudgetPlanListUseCase getPlanListUseCase, 
            IGetJournalAccountByUIDUseCase getAccountByUIDUseCase,
            IGetJournalAccountListByTypesUseCase getAccountsByTypesUseCase,
            IDeleteBudgetPlanUseCase deletePlanUseCase,
            ISaveBudgetPlanUseCase savePlanUseCase,
            BudgetPlanFactory budgetFactory,
            ScheduleRecurrenceFactory recurrenceFactory) : base()
        {
            this.getPlanListUseCase = getPlanListUseCase;
            this.getAccountByUIDUseCase = getAccountByUIDUseCase;
            this.getAccountsByTypesUseCase = getAccountsByTypesUseCase;
            this.deletePlanUseCase = deletePlanUseCase;
            this.savePlanUseCase = savePlanUseCase;
            this.budgetFactory = budgetFactory;
            this.recurrenceFactory = recurrenceFactory;


            this.Reload();
        }

        private ObservableCollection<IBudgetPlan> _listPlans = new ObservableCollection<IBudgetPlan>();
        public ObservableCollection<IBudgetPlan> PlanList { get { return _listPlans; } }

        public Guid BudgetPlanId { get; set; }

        private List<SpecialDropListItem<BudgetPlanType>> _listPlanTypes = new List<SpecialDropListItem<BudgetPlanType>>()
        {
            new SpecialDropListItem<BudgetPlanType>("Income", BudgetPlanType.Receivable),
            new SpecialDropListItem<BudgetPlanType>("Expense", BudgetPlanType.Payable),
            new SpecialDropListItem<BudgetPlanType>("Debt Payment", BudgetPlanType.DebtPayment),
            new SpecialDropListItem<BudgetPlanType>("Transfer", BudgetPlanType.Transfer)
        };

        public List<SpecialDropListItem<BudgetPlanType>> PlanTypes { get { return _listPlanTypes; } }

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

        private string _desc;

        public string Description
        {
            get { return _desc; }
            set
            {
                _desc = value;
                NotifyPropertyChanged(nameof(Description));
            }
        }

        private ObservableCollection<SpecialDropListItem<IJournalAccount>> _listValidDebits = new ObservableCollection<SpecialDropListItem<IJournalAccount>>();

        public ObservableCollection<SpecialDropListItem<IJournalAccount>> ValidDebitAccounts
        { get { return _listValidDebits; } }

        private IJournalAccount _debit;

        public IJournalAccount? SelectedDebitAccount
        {
            get { return _debit; }
            set
            {
                _debit = value;
                NotifyPropertyChanged(nameof(SelectedDebitAccount));
            }
        }

        private ObservableCollection<SpecialDropListItem<IJournalAccount>> _listValidCredits = new ObservableCollection<SpecialDropListItem<IJournalAccount>>();

        public ObservableCollection<SpecialDropListItem<IJournalAccount>> ValidCreditAccounts
        { get { return _listValidCredits; } }

        private IJournalAccount _credit;

        public IJournalAccount? SelectedCreditAccount
        {
            get { return _credit; }
            set
            {
                _credit = value;
                NotifyPropertyChanged(nameof(SelectedCreditAccount));
            }
        }

        private decimal _amount;

        public decimal Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                NotifyPropertyChanged(nameof(Amount));
            }
        }

        private IScheduleRecurrence _recurrence;

        public IScheduleRecurrence Recurrence
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

        private RelayCommand _cmdSaveChanges;

        public RelayCommand CommandSaveChanges
        {
            get
            {
                return _cmdSaveChanges ?? (_cmdSaveChanges = new RelayCommand((o) =>
                {
                    if (this.IsReadyForSave)
                    {
                        savePlanUseCase.Execute(budgetFactory.Build(this.SelectedPlanType, this.BudgetPlanId, this.Description, this.SelectedDebitAccount, this.SelectedCreditAccount, this.Amount, this.Recurrence));
                    }

                    this.Clear();
                    this.Reload();
                }));
            }
        }

        private RelayCommand _cmdNewRecord;

        public RelayCommand CommandAddNew
        {
            get
            {
                return _cmdNewRecord ?? (_cmdNewRecord = new RelayCommand((o) =>
                {
                    this.Clear();
                }));
            }
        }

        private RelayCommand _cmdEditRecord;

        public RelayCommand CommandEditRecord
        {
            get
            {
                return _cmdEditRecord ?? (_cmdEditRecord = new RelayCommand((plan) =>
                {
                    if (plan is IBudgetPlan jplan)
                    {
                        this.LoadJournalPlan(jplan);
                    }
                }));
            }
        }

        private RelayCommand _cmdDeleteRecord;
        

        public RelayCommand CommandDeleteRecord
        {
            get
            {
                return _cmdDeleteRecord ?? (_cmdDeleteRecord = new RelayCommand((plan) =>
                {
                    if (plan is IBudgetPlan jplan)
                    {
                        deletePlanUseCase.Execute(jplan.UID);

                        this.Clear();
                        this.Reload();
                    }
                }));
            }
        }

        #endregion Commands

        private void Reload()
        {
            this.PlanList.Clear();
            var listFull = getPlanListUseCase.Execute();
            if (listFull?.Any() != true) return;

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
            this.Recurrence = recurrenceFactory.Build(RecurrenceFrequency.Annual, DateTime.Today);
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