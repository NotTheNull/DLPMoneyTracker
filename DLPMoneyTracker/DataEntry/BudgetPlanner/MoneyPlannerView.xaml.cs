using DLPMoneyTracker.DataEntry.ScheduleRecurrence;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DLPMoneyTracker.DataEntry.BudgetPlanner
{
    // TODO: Noticed a problem with going back and forth between editing an expense and editing an income.  Does not seem to load the right radio button option on occasions

    /// <summary>
    /// Interaction logic for BudgetPlannerView.xaml
    /// </summary>
    public partial class MoneyPlannerView : Window
    {
        private MoneyPlannerVM _viewModel;

        public MoneyPlannerView(MoneyPlannerVM viewModel)
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