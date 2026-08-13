using System;

namespace WinUICalculator.ViewModels
{
    public class CellItemViewModel : ViewModelBase
    {
        private string _text = "";
        private double? _value = null;

        public string Text
        {
            get => _text;
            set
            {
                if (SetProperty(ref _text, value))
                {
                    UpdateValue();
                    OnTextChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public double? Value => _value;

        public event EventHandler OnTextChanged;

        private void UpdateValue()
        {
            if (string.IsNullOrWhiteSpace(_text))
            {
                _value = null;
            }
            else if (double.TryParse(_text, out double result))
            {
                _value = result;
            }
            else
            {
                _value = 0; // Or keep it null depending on how errors should be handled
            }
            OnPropertyChanged(nameof(Value));
        }
    }
}
