using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace SmartAdder.Behaviors
{
    public class FocusWithinBehavior : Behavior<ListView>
    {
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.Register(nameof(IsFocused), typeof(bool), typeof(FocusWithinBehavior), new PropertyMetadata(false));

        public bool IsFocused
        {
            get => (bool)GetValue(IsFocusedProperty);
            set => SetValue(IsFocusedProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.GotFocus += OnGotFocus;
            AssociatedObject.LostFocus += OnLostFocus;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.GotFocus -= OnGotFocus;
            AssociatedObject.LostFocus -= OnLostFocus;
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            IsFocused = true;
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            // Delay checking focus to allow the new element to receive focus
            AssociatedObject.DispatcherQueue.TryEnqueue(() =>
            {
                var focusedElement = Microsoft.UI.Xaml.Input.FocusManager.GetFocusedElement(AssociatedObject.XamlRoot);
                if (focusedElement is DependencyObject depObj)
                {
                    bool isFocusInList = false;
                    DependencyObject current = depObj;
                    while (current != null)
                    {
                        if (current == AssociatedObject)
                        {
                            isFocusInList = true;
                            break;
                        }
                        current = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetParent(current);
                    }

                    if (!isFocusInList)
                    {
                        IsFocused = false;
                    }
                }
                else
                {
                    IsFocused = false;
                }
            });
        }
    }
}
