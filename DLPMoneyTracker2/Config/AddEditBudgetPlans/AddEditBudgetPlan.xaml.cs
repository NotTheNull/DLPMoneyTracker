using Microsoft.Extensions.DependencyInjection;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
