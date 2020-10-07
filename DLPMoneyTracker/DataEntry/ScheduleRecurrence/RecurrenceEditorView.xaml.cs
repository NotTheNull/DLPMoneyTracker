using DLPMoneyTracker.Data.ScheduleRecurrence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DLPMoneyTracker.DataEntry.ScheduleRecurrence
{
    /// <summary>
    /// Interaction logic for RecurrenceEditor.xaml
    /// </summary>
    public partial class RecurrenceEditorView : Window
    {
        private RecurrenceEditorVM _viewModel;
        public RecurrenceEditorVM ViewModel { get { return _viewModel; } }

        public RecurrenceEditorView(RecurrenceEditorVM viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            _viewModel = viewModel;
            _viewModel.RecurrenceSelected += _viewModel_RecurrenceSelected;
        }

        public void LoadRecurrence(IScheduleRecurrence recurrence)
        {
            this.ViewModel.EditRecurrence(recurrence);
        }

        private void _viewModel_RecurrenceSelected(Data.ScheduleRecurrence.IScheduleRecurrence selected)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
