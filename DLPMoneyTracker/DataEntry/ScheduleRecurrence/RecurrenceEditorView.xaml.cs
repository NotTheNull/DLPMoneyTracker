using DLPMoneyTracker.Data.ScheduleRecurrence;
using System.Windows;

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