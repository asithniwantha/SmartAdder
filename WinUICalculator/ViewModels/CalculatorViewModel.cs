using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace WinUICalculator.ViewModels
{
    public class CalculatorViewModel : ViewModelBase
    {
        private double _totalSum;

        public ObservableCollection<CellItemViewModel> Cells { get; } = new ObservableCollection<CellItemViewModel>();

        public double TotalSum
        {
            get => _totalSum;
            private set => SetProperty(ref _totalSum, value);
        }

        public CalculatorViewModel()
        {
            AddNewCell();
        }

        private void AddNewCell()
        {
            var newCell = new CellItemViewModel();
            newCell.OnTextChanged += Cell_OnTextChanged;
            Cells.Add(newCell);
        }

        private void Cell_OnTextChanged(object sender, EventArgs e)
        {
            CalculateSum();
            EnsureEmptyCellAtEnd();
        }

        private void CalculateSum()
        {
            TotalSum = Cells.Where(c => c.Value.HasValue).Sum(c => c.Value.Value);
        }

        private void EnsureEmptyCellAtEnd()
        {
            if (Cells.Count == 0 || !string.IsNullOrWhiteSpace(Cells.Last().Text))
            {
                AddNewCell();
            }
        }
    }
}
