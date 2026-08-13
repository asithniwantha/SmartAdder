using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartAdder.Models
{
    public partial class NumberCell : ObservableObject
    {
        // Source generator automatically creates the public 'InputValue' property
        // and handles PropertyChanged notifications.
        [ObservableProperty]
        private string _inputValue = string.Empty;

        // Safely parse the string to a double for the calculator
        public double Value
        {
            get
            {
                if (double.TryParse(InputValue, out double result))
                    return result;

                return 0;
            }
        }
    }
}