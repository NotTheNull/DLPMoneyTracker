using Microsoft.Toolkit.Wpf.UI.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace DLPMoneyTracker.Core.Behaviors
{
    public class WebViewHTMLStringBehavior
    {
        public static readonly DependencyProperty HTMLProperty = DependencyProperty.RegisterAttached("HTML", typeof(string), typeof(WebViewHTMLStringBehavior), new FrameworkPropertyMetadata(OnHTMLChanged));

        [AttachedPropertyBrowsableForType(typeof(WebView))]
        public static string GetHTML(WebView d)
        {
            return (string)d.GetValue(HTMLProperty);
        }

        public static void SetHTML(WebView d, string value)
        {
            d.SetValue(HTMLProperty, value);
        }


        static void OnHTMLChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is null) return;
            if(d is WebView view)
            {
                view.NavigateToString(e.NewValue as string);
            }
        }
    }
}
