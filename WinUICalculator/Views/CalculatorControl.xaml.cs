using Microsoft.UI.Xaml.Controls;
using WinUICalculator.ViewModels;

namespace WinUICalculator.Views
{
    public sealed partial class CalculatorControl : UserControl
    {
        public CalculatorViewModel ViewModel { get; }

        public CalculatorControl()
        {
            this.ViewModel = new CalculatorViewModel();
            this.InitializeComponent();
        }
    }
}
