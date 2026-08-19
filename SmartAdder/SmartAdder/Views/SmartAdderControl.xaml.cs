using Microsoft.UI.Xaml.Controls;
using SmartAdder.ViewModels;

namespace SmartAdder.Views
{
    public sealed partial class SmartAdderControl : UserControl
    {
        public SmartAdderViewModel ViewModel { get; } = new SmartAdderViewModel();

        public SmartAdderControl()
        {
            this.InitializeComponent();
        }
    }
}