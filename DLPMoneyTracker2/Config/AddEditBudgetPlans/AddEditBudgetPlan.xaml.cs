using DLPMoneyTracker.Data.ScheduleRecurrence;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DLPMoneyTracker2.Config.AddEditBudgetPlans
{
    /// <summary>
    /// Interaction logic for AddEditBudgetPlan.xaml
    /// </summary>
    public partial class AddEditBudgetPlan : Window
    {
        private AddEditBudgetPlanVM _viewModel;

        public AddEditBudgetPlan(AddEditBudgetPlanVM viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            _viewModel = viewModel;
        }

        private void btnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnEditRecurrence_Click(object sender, RoutedEventArgs e)
        {
            RecurrenceEditor uiEditRecurrence = UICore.DependencyHost.GetService<RecurrenceEditor>();
            uiEditRecurrence.LoadRecurrence(_viewModel.Recurrence);
            uiEditRecurrence.ViewModel.RecurrenceSelected += ViewModel_RecurrenceSelected;
            uiEditRecurrence.Show();
        }

        private void ViewModel_RecurrenceSelected(IScheduleRecurrence selected)
        {
            _viewModel.Recurrence = selected;
        }
    }
}