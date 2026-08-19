using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Xaml.Interactivity;
using System;
using System.Text.RegularExpressions;
using Windows.System;
using Microsoft.UI.Input;

namespace SmartAdder.Behaviors
{
    public class NumericTextBoxBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
            AssociatedObject.TextChanging += OnTextChanging;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
            AssociatedObject.TextChanging -= OnTextChanging;
        }

        private void OnPreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            bool isPlus = e.Key == VirtualKey.Add;

            if (e.Key == (VirtualKey)187 || e.Key == (VirtualKey)107)
            {
                var shiftState = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift);
                if (shiftState.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down))
                {
                    isPlus = true;
                }
            }

            var options = new FindNextElementOptions
            {
                SearchRoot = AssociatedObject.XamlRoot?.Content
            };

            if (e.Key == VirtualKey.Enter || isPlus || e.Key == VirtualKey.Down)
            {
                e.Handled = true;

                var next = FocusManager.FindNextElement(FocusNavigationDirection.Down, options);
                if (next is Control control)
                {
                    var tb = FindInnerTextBox(next);
                    if (tb != null) tb.Focus(FocusState.Keyboard);
                    else control.Focus(FocusState.Keyboard);
                }
            }
            else if (e.Key == VirtualKey.Up)
            {
                e.Handled = true;

                var next = FocusManager.FindNextElement(FocusNavigationDirection.Up, options);
                if (next is Control control)
                {
                    var tb = FindInnerTextBox(next);
                    if (tb != null) tb.Focus(FocusState.Keyboard);
                    else control.Focus(FocusState.Keyboard);
                }
            }
        }

        private TextBox FindInnerTextBox(DependencyObject parent)
        {
            if (parent is TextBox tb) return tb;
            if (parent == null) return null;

            int count = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(parent, i);
                var result = FindInnerTextBox(child);
                if (result != null) return result;
            }
            return null;
        }

        private void OnTextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            string text = sender.Text;
            string newText = Regex.Replace(text, "[^0-9.]", "");

            int decimalCount = 0;
            string finalString = "";
            foreach (char c in newText)
            {
                if (c == '.')
                {
                    if (decimalCount == 0)
                    {
                        finalString += c;
                        decimalCount++;
                    }
                }
                else
                {
                    finalString += c;
                }
            }

            if (text != finalString)
            {
                int pos = sender.SelectionStart - (text.Length - finalString.Length);
                sender.Text = finalString;
                sender.SelectionStart = Math.Max(0, pos);
            }
        }
    }
}
