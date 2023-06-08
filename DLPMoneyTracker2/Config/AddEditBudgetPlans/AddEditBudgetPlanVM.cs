﻿using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using DLPMoneyTracker.Data.TransactionModels.JournalPlan;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace DLPMoneyTracker2.Config.AddEditBudgetPlans
{
    public class AddEditBudgetPlanVM : BaseViewModel
    {
        private readonly ITrackerConfig _config;
        private readonly IJournalPlanner _planner;

        public AddEditBudgetPlanVM(ITrackerConfig config, IJournalPlanner planner) : base()
        {
            _config = config;
            _planner = planner;
        }


        private ObservableCollection<IJournalPlan> _listPlans = new ObservableCollection<IJournalPlan>();
        public ObservableCollection<IJournalPlan> PlanList { get { return _listPlans; } }





        public Guid BudgetPlanId { get; set; }

        private List<SpecialDropListItem<JournalPlanType>> _listPlanTypes = new List<SpecialDropListItem<JournalPlanType>>()
        {
            new SpecialDropListItem<JournalPlanType>("Income", JournalPlanType.Receivable),
            new SpecialDropListItem<JournalPlanType>("Expense", JournalPlanType.Payable),
            new SpecialDropListItem<JournalPlanType>("Debt Payment", JournalPlanType.DebtPayment),
            new SpecialDropListItem<JournalPlanType>("Transfer", JournalPlanType.Transfer)
        };


        public List<SpecialDropListItem<JournalPlanType>> PlanTypes { get { return _listPlanTypes; } }

        private JournalPlanType _planType;

        public JournalPlanType SelectedPlanType
        {
            get { return _planType; }
            set
            {
                _planType = value;
                this.LoadValidAccounts();
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
        public ObservableCollection<SpecialDropListItem<IJournalAccount>> ValidDebitAccounts { get { return _listValidDebits; } }

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
        public ObservableCollection<SpecialDropListItem<IJournalAccount>> ValidCreditAccounts { get { return _listValidCredits; } }


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













        #region Commands


        private RelayCommand _cmdSaveChanges;
        public RelayCommand CommandSaveChanges
        {
            get
            {
                return _cmdSaveChanges ?? (_cmdSaveChanges = new RelayCommand((o) =>
                {
                    var plan = _planner.JournalPlanList.FirstOrDefault(x => x.UID == this.BudgetPlanId);
                    if (plan is null)
                    {
                        plan = JournalPlanFactory.Build(_config, this.SelectedPlanType, this.Description, this.SelectedCreditAccount, this.SelectedDebitAccount, this.Amount, this.Recurrence);
                    }
                    _planner.AddPlan(plan);
                    _planner.SaveToFile();
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
                    if (plan is IJournalPlan jplan)
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
                    if (plan is IJournalPlan jplan)
                    {
                        _planner.RemovePlan(jplan);
                    }
                }));
            }
        }

        #endregion





        private void Clear()
        {
            this.BudgetPlanId = Guid.Empty;
            this.Amount = decimal.Zero;
            this.Description = string.Empty;
            this.SelectedCreditAccount = null;
            this.SelectedDebitAccount = null;
            this.SelectedPlanType = JournalPlanType.Payable;
            this.Recurrence = ScheduleRecurrenceFactory.Build(RecurrenceFrequency.Annual, DateTime.Today);
        }


        private void LoadValidAccounts()
        {

        }


        private void LoadJournalPlan(IJournalPlan plan)
        {
            this.BudgetPlanId = plan.UID;
            this.SelectedPlanType = plan.PlanType;
            this.SelectedCreditAccount = _config.LedgerAccountsList.FirstOrDefault(x => x.Id == plan.CreditAccountId);
            this.SelectedDebitAccount = _config.LedgerAccountsList.FirstOrDefault(x => x.Id == plan.DebitAccountId);
            this.Description = plan.Description;
            this.Amount = plan.ExpectedAmount;

            if (string.IsNullOrWhiteSpace(plan.RecurrenceJSON))
            {
                this.Recurrence = ScheduleRecurrenceFactory.Build(RecurrenceFrequency.Annual, DateTime.Today);
            }
            else
            {
                this.Recurrence = JsonSerializer.Deserialize<IScheduleRecurrence>(plan.RecurrenceJSON);
            }
        }


    }

}
