using Microsoft.UI.Xaml.Controls;
using SmartAdder.Services;
using SmartAdder.ViewModels;

namespace SmartAdder.Views
{
    public sealed partial class SmartAdderControl : UserControl
    {
        public SmartAdderViewModel ViewModel { get; }

        public SmartAdderControl()
        {
            this.InitializeComponent();

            // Composition root: wire up concrete service implementations for the ViewModel's abstractions.
            ViewModel = new SmartAdderViewModel(new DatabaseService(), new HistoryDialogService());
        }
    }
}
