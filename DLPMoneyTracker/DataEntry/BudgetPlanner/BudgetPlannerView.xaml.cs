using DLPMoneyTracker.DataEntry.ScheduleRecurrence;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DLPMoneyTracker.DataEntry.BudgetPlanner
{
    /// <summary>
    /// Interaction logic for BudgetPlannerView.xaml
    /// </summary>
    public partial class BudgetPlannerView : Window
    {
        private BudgetPlannerVM _viewModel;

        public BudgetPlannerView(BudgetPlannerVM viewModel)
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
            RecurrenceEditorView uiEditRecurrence = UICore.DependencyHost.GetService<RecurrenceEditorView>();
            uiEditRecurrence.LoadRecurrence(_viewModel.Recurrence);
            uiEditRecurrence.ViewModel.RecurrenceSelected += ViewModel_RecurrenceSelected;
            uiEditRecurrence.Show();
        }

        private void ViewModel_RecurrenceSelected(Data.ScheduleRecurrence.IScheduleRecurrence selected)
        {
            _viewModel.Recurrence = selected;
        }
    }
}
