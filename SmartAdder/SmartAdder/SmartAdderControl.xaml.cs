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

            if (e.Key == VirtualKey.Enter || isPlus)
            {
                e.Handled = true; // Prevent character from being entered

                // Move focus to next element
                FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
            }
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
