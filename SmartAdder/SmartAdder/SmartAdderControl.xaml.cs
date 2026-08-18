using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SmartAdder.ViewModels;
using System;
using System.Text.RegularExpressions;
using Windows.System;
using Microsoft.UI.Input;

namespace SmartAdder.Views
{
    public sealed partial class SmartAdderControl : UserControl
    {
        public SmartAdderViewModel ViewModel { get; } = new SmartAdderViewModel();

        public SmartAdderControl()
        {
            this.InitializeComponent();
        }

        private void RootGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ViewModel.IsHovering = true;
        }

        private void RootGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ViewModel.IsHovering = false;
        }

        private void CellListView_GotFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.IsListFocused = true;
        }

        private void CellListView_LostFocus(object sender, RoutedEventArgs e)
        {
            // Delay checking focus to allow the new element to receive focus
            DispatcherQueue.TryEnqueue(() =>
            {
                var focusedElement = FocusManager.GetFocusedElement(this.XamlRoot);
                if (focusedElement is DependencyObject depObj)
                {
                    // Check if the newly focused element is a child of the CellListView
                    bool isFocusInList = false;
                    DependencyObject current = depObj;
                    while (current != null)
                    {
                        if (current == CellListView)
                        {
                            isFocusInList = true;
                            break;
                        }
                        current = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetParent(current);
                    }

                    if (!isFocusInList)
                    {
                        ViewModel.IsListFocused = false;
                    }
                }
                else
                {
                    ViewModel.IsListFocused = false;
                }
            });
        }

        private void TextBox_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            // + key can come from numpad (Add) or keyboard (187 or Shift+=)
            bool isPlus = e.Key == VirtualKey.Add;

            // Check if Shift is held down (for =/+ key)
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
                SearchRoot = this.XamlRoot?.Content
            };

            if (e.Key == VirtualKey.Delete)
            {
                e.Handled = true;

                if (ViewModel.Cells.Count == 1)
                {
                    if (sender is TextBox textBox)
                    {
                        textBox.Text = string.Empty;
                        if (textBox.DataContext is SmartAdder.Models.NumberCell cell)
                        {
                            cell.InputValue = string.Empty;
                        }
                    }
                }
                else
                {
                    if (sender is TextBox textBox && textBox.DataContext is SmartAdder.Models.NumberCell cell)
                    {
                        ViewModel.RemoveCell(cell);

                        // Wait for layout to update before trying to focus the last element
                        _ = DispatcherQueue.TryEnqueue(() =>
                        {
                            if (ViewModel.Cells.Count > 0)
                            {
                                var lastCell = ViewModel.Cells[ViewModel.Cells.Count - 1];
                                CellListView.ScrollIntoView(lastCell);
                                CellListView.UpdateLayout();

                                var container = CellListView.ContainerFromItem(lastCell) as ListViewItem;
                                if (container != null)
                                {
                                    var innerTb = FindInnerTextBox(container);
                                    if (innerTb != null)
                                    {
                                        innerTb.Focus(FocusState.Keyboard);
                                    }
                                }
                            }
                        });
                    }
                }
            }
            else if (e.Key == VirtualKey.Enter || isPlus || e.Key == VirtualKey.Down)
            {
                e.Handled = true; // Prevent character from being entered or default action

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
                e.Handled = true; // Prevent default action

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

        private void TextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            // Prevent entering any characters except numbers and decimal point
            string text = sender.Text;
            string newText = Regex.Replace(text, "[^0-9.]", "");

            // Optionally, prevent multiple decimal points
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
