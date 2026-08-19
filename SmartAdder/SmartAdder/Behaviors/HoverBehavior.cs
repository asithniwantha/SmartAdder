using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.Xaml.Interactivity;

namespace SmartAdder.Behaviors
{
    public class HoverBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty IsHoveringProperty =
            DependencyProperty.Register(nameof(IsHovering), typeof(bool), typeof(HoverBehavior), new PropertyMetadata(false));

        public bool IsHovering
        {
            get => (bool)GetValue(IsHoveringProperty);
            set => SetValue(IsHoveringProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PointerEntered += OnPointerEntered;
            AssociatedObject.PointerExited += OnPointerExited;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PointerEntered -= OnPointerEntered;
            AssociatedObject.PointerExited -= OnPointerExited;
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            IsHovering = true;
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            IsHovering = false;
        }
    }
}
