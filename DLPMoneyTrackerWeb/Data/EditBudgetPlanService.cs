using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using DLPMoneyTracker.Data.TransactionModels.JournalPlan;
using Microsoft.AspNetCore.Components.WebView.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTrackerWeb.Data
{
    public interface IEditBudgetPlanService
    {
        IEnumerable<IJournalPlan> GetBudgetList();
        IEnumerable<IJournalAccount> GetJournalAccounts(IList<JournalAccountType> validAccountTypes);

        EditBudgetPlanVM GetViewModel();
        void SaveBudgetPlan(EditBudgetPlanVM vm);
        void DeleteBudgetPlan(Guid idPlan);
    }

    public class EditBudgetPlanService : IEditBudgetPlanService
    {
        private readonly ITrackerConfig _config;
        private readonly IJournalPlanner _planner;

        public EditBudgetPlanService(ITrackerConfig config, IJournalPlanner planner)
        {
            _config = config;
            _planner = planner;
        }





        public IEnumerable<IJournalPlan> GetBudgetList()
        {
            return _planner.JournalPlanList;
        }

        public IEnumerable<IJournalAccount> GetJournalAccounts(IList<JournalAccountType> validAccountTypes)
        {
            return _config.GetJournalAccountList(new JournalAccountSearch(validAccountTypes));
        }

        public EditBudgetPlanVM GetViewModel()
        {
            return new EditBudgetPlanVM(_config);
        }


        public void SaveBudgetPlan(EditBudgetPlanVM vm)
        {

            var newPlan = JournalPlanFactory.Build(_config, vm.PlanType, vm.Description, vm.CreditAccount, vm.DebitAccount, vm.ExpectedAmount, vm.Recurrence);
        }

        public void DeleteBudgetPlan(Guid idPlan)
        {
            var plan = _planner.JournalPlanList.FirstOrDefault(x => x.UID == idPlan);
            _planner.RemovePlan(plan);
        }
    }

    public class EditBudgetPlanVM : IJournalPlan
    {
        private readonly ITrackerConfig _config;
        private IJournalPlan _plan;

        public EditBudgetPlanVM(ITrackerConfig config) 
        {
            _config = config;
        }
        public EditBudgetPlanVM(ITrackerConfig config, IJournalPlan plan)
        {
            _config = config;
            this.Copy(plan);
        }


        public Guid UID { get; set; }
        public JournalPlanType PlanType 
        {
            get { return _plan?.PlanType ?? JournalPlanType.NotSet; }
            set
            {
                switch(value)
                {
                    case JournalPlanType.Payable:
                        _plan = new PayablePlan();
                        break;
                    case JournalPlanType.Receivable:
                        _plan = new ReceivablePlan();
                        break;
                    case JournalPlanType.DebtPayment:
                        _plan = new DebtPaymentPlan();
                        break;
                    case JournalPlanType.Transfer:
                        _plan = new TransferPlan();
                        break;
                    default:
                        _plan = null;
                        break;

                }
            }
        }




        public List<JournalAccountType> ValidDebitAccountTypes { get { return _plan?.ValidDebitAccountTypes ?? new List<JournalAccountType>(); }  }
        public IJournalAccount DebitAccount { get; set; }
        public Guid DebitAccountId 
        {
            get { return DebitAccount?.Id ?? Guid.Empty; } 
            set
            {
                if(value == Guid.Empty)
                {
                    this.DebitAccount = null;
                }
                else
                {
                    this.DebitAccount = _config.GetJournalAccount(value); 
                }
            }
        }
        public string DebitAccountName { get { return DebitAccount?.Description ?? string.Empty; } }


        public List<JournalAccountType> ValidCreditAccountTypes { get { return _plan?.ValidCreditAccountTypes ?? new List<JournalAccountType>(); } }
        public IJournalAccount CreditAccount { get; set; }
        public Guid CreditAccountId 
        { 
            get { return CreditAccount?.Id ?? Guid.Empty; } 
            set
            {
                if(value == Guid.Empty)
                {
                    this.CreditAccount = null;
                }
                else
                {
                    this.CreditAccount = _config.GetJournalAccount(value);
                }

            }
        }
        public string CreditAccountName { get { return CreditAccount?.Description ?? string.Empty; } }


        public int PriorityOrder { get; set; }

        public string Description { get; set; }


        public IScheduleRecurrence Recurrence { get; set; }

        public string RecurrenceJSON { get { return Recurrence.GetFileData(); } }

        public RecurrenceFrequency Frequency { get { return Recurrence.Frequency; } }

        public decimal ExpectedAmount { get; set; }

        public DateTime NotificationDate { get { return Recurrence.NotificationDate; } }

        public DateTime NextOccurrence { get { return Recurrence.NextOccurence; } }



        public void Copy(IJournalPlan plan)
        {
            this.UID = plan.UID;
            this.PlanType = plan.PlanType;
            this.Description = plan.Description;
            this.PriorityOrder = plan.PriorityOrder;
            this.ExpectedAmount = plan.ExpectedAmount;

            this.Recurrence = ScheduleRecurrenceFactory.Build(plan.RecurrenceJSON);

            this.CreditAccount = _config.GetJournalAccount(plan.CreditAccountId);
            this.DebitAccount = _config.GetJournalAccount(plan.DebitAccountId);

        }


        public bool IsValid()
        {
            throw new NotImplementedException();
        }
    }

}
