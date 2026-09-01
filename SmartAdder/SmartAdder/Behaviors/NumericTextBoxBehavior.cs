using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Xaml.Interactivity;
using System;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Windows.System;
using Microsoft.UI.Input;
using Microsoft.UI.Dispatching;

namespace SmartAdder.Behaviors
{
    public class NumericTextBoxBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register(
                nameof(DeleteCommand),
                typeof(ICommand),
                typeof(NumericTextBoxBehavior),
                new PropertyMetadata(null));

        public ICommand? DeleteCommand
        {
            get => (ICommand?)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }

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
            else if (e.Key == VirtualKey.Delete)
            {
                e.Handled = true;

                var command = DeleteCommand;
                var cell = AssociatedObject.DataContext;

                if (command != null && command.CanExecute(cell))
                {
                    var listView = FindAncestor<ListView>(AssociatedObject);
                    var dispatcherQueue = AssociatedObject.DispatcherQueue;

                    command.Execute(cell);

                    if (listView != null)
                    {
                        dispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () => FocusLastCell(listView));
                    }
                }
            }
        }

        private static void FocusLastCell(ListView listView)
        {
            if (listView.Items.Count == 0) return;

            var lastItem = listView.Items[listView.Items.Count - 1];
            var container = listView.ContainerFromItem(lastItem);
            var tb = container != null ? FindInnerTextBox(container) : null;
            tb?.Focus(FocusState.Keyboard);
        }

        private static T? FindAncestor<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetParent(child);
            while (parent != null && parent is not T)
            {
                parent = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetParent(parent);
            }
            return parent as T;
        }

        private static TextBox? FindInnerTextBox(DependencyObject parent)
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
