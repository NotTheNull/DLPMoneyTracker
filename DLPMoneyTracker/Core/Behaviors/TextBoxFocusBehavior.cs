using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace DLPMoneyTracker.Core.Behaviors
{
    public class TextBoxFocusBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.GotFocus += AssociatedObject_GotFocus;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.GotFocus -= AssociatedObject_GotFocus;
            base.OnDetaching();
        }

        private void AssociatedObject_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (AssociatedObject is null) return;
            AssociatedObject.SelectAll();
        }
    }
}