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
    public partial class RecurrenceEditorView : UserControl
    {
        private RecurrenceEditorVM _viewModel;
        public RecurrenceEditorVM ViewModel { get { return _viewModel; } }

        public RecurrenceEditorView(RecurrenceEditorVM viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            _viewModel = viewModel;
        }

        
    }
}
