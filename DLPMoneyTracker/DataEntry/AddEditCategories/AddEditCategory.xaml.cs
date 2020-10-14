using System.Windows;

namespace DLPMoneyTracker.DataEntry.AddEditCategories
{
    /// <summary>
    /// Interaction logic for AddEditCategory.xaml
    /// </summary>
    public partial class AddEditCategory : Window
    {
        public AddEditCategory(AddEditCategoryVM viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void btnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}